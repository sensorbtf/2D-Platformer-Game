using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    // Singletone instance
    public static Player Instance { get; private set; }

    [Header("Player parameters")]
    [SerializeField] private int  health = 3;
    [SerializeField] private int  damage = 1;
    [SerializeField] private float  speed = 5;
    [SerializeField] private float  jumpForce = 30;
    [SerializeField] private float  wallSlidingSpeed = 3;
    [SerializeField] private float  timeBetweenAttacks = 0.4f; 
    [SerializeField] private float  attackRange = 0.5f;
    [SerializeField] private float xWallForce = 5;
    [SerializeField] private float yWallForce = 20;
    [SerializeField] private float wallJumpTime = 0.1f;

    private Rigidbody2D rB2D;
    private Collider2D playerCollider;
    private Animator anim;
    private float input;

    // Hero movement checkers
    private bool doDash = false;
    private bool doJump = false;
    private bool doJumpDown = false;
    private bool doJumpFromWall = false;

    [Header("Checkers")]
    [SerializeField] private Transform platformTouchingValidator;
    [SerializeField] private Transform wallTouchingValidator;
    [SerializeField] private Transform attackValidator;
    [SerializeField] private float radiousChecker;
    [SerializeField] private LayerMask whatIsPlatform;
    [SerializeField] private LayerMask whatAreWallsAndCeiling;
    [SerializeField] private LayerMask whatAreEnemies;
    [SerializeField] private GameObject Blood;
    [SerializeField] private Collider2D mapCollider;

    // Hero Dashing

    private bool canDash = true;
    private bool isDashing = false;
    private float dashingPower = 100;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    // Hero state Checkers

    private bool  isFacingRight = true;
    private bool  isGrounded;
    private bool  isTouchingWall;
    private bool  isSlidingWall;
    private float nextAttackTime;
    private bool  isJumpingFromWall;

    // Sound
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip runningSound;
    [SerializeField] private AudioClip getDamagedSound;
    [SerializeField] private AudioClip jumpedDownSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;

    // Properties for most used fields
    public Rigidbody2D RB2D { get => rB2D; set => rB2D = value; }
    public float Speed
    {
        get => speed;
        set { speed = value < 0 ?  speed = 0 : speed = value; }
    }
    public float JumpForce
    {
        get => jumpForce;
        set { jumpForce = value < 0 ?  jumpForce = 0 : jumpForce = value; }
    }
    public float WallSlidingSpeed
    {
        get => wallSlidingSpeed;
        set { wallSlidingSpeed = value < 0 ?  wallSlidingSpeed = 0 : wallSlidingSpeed = value; }
    }
    public float TimeBetweenAtacks
    {
        get => timeBetweenAttacks;
        set { timeBetweenAttacks = value < 0 ? timeBetweenAttacks = 0 : timeBetweenAttacks = value; }
    }
    public float AttackRange
    {
        get => attackRange;
        set { attackRange = value < 0 ? attackRange = 0 : attackRange = value; }
    }
    public int Damage
    {
        get => damage;
        set { damage = value < 0 ?  damage = 0 : damage = value; }
    }
    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (value <= 0)
            {
                rB2D.constraints = RigidbodyConstraints2D.FreezePosition;
                StartCoroutine(Die());
            }
        }
    }
    private void Start()
    {
        rB2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void Update()
    {
        if (isDashing == true)
            return;

        input = Input.GetAxisRaw("Horizontal");

        PlayerPositionChecker();
        PlayerInput();
        HeroState();
        HeroStateAnimations();
    }
    private void FixedUpdate()
    {
        RB2D.velocity = new Vector2(input * speed, RB2D.velocity.y);

        if (doDash == true)
        {
            Dash();
        }
        if (doJump == true)
        {
            Jump();
        }
        if (doJumpDown == true)
        {
            StartCoroutine(JumpOff());
        }
        if (isSlidingWall)
        {
            SlideOnWall();
        }
        if (doJumpFromWall == true)
        {
            JumpFromWall();
        }
    }
    private void HeroState()
    {
        if (input > 0 && isFacingRight == false)
        {
            FlipHeroSprite();
        }
        else if (input < 0 && isFacingRight == true)
        {
            FlipHeroSprite();
        }

        if (isTouchingWall == true && isGrounded == false && input != 0)
        {
            isSlidingWall = true;
        }
        else
        {
            isSlidingWall = false;
        }
    }

    private void HeroStateAnimations()
    {
        if (input != 0 && isGrounded == true)
        {
            anim.SetBool("isRunning", true);
            if (input != 0 && !SoundManager.Instance.PlayerWalkingSource.isPlaying)
            {
                SoundManager.Instance.PlayWalkingEffect(runningSound);
                SoundManager.Instance.PlayerWalkingSource.Play();
            }
            else if (input == 0 || isGrounded == false)
                SoundManager.Instance.PlayerWalkingSource.Stop();
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        if (isGrounded == true)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }
        else
        {
            anim.SetBool("isJumping", true);
            if (RB2D.velocity.y < -0.1)
            {
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", true);
            }
            else
            {
                anim.SetBool("isFalling", false);
            }
        }
    }
    void PlayerPositionChecker()
    {
        isGrounded = Physics2D.OverlapCircle(platformTouchingValidator.position, radiousChecker, whatIsPlatform);
        isTouchingWall = Physics2D.OverlapCircle(wallTouchingValidator.position, radiousChecker, whatAreWallsAndCeiling);
    }
    void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded == true)
        {
            doJump = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded == true)
        {
            doJumpDown = true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && isSlidingWall == true)
        {
            isJumpingFromWall = true;
            Invoke(nameof(SetWallJumpingToFalse), wallJumpTime);

            SoundManager.Instance.PlayPlayerEffects(jumpSound);
        }
        if (isJumpingFromWall == true)
        {
            doJumpFromWall = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash == true && isGrounded == false)
        {
            doDash = true;
        }

        if (Time.time > nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
                SoundManager.Instance.PlayPlayerEffects(attackSound);
                anim.SetTrigger("Attacking");
                nextAttackTime = Time.time + TimeBetweenAtacks;
            }
        }
    }
    void Jump()
    {
        RB2D.velocity = Vector2.up * JumpForce;
        SoundManager.Instance.PlayPlayerEffects(jumpSound);
        doJump = false;
    }
    void Attack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackValidator.position, AttackRange, whatAreEnemies);
        foreach (Collider2D enemies in enemiesToDamage)
        {
            enemies.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
    void SlideOnWall()
    {
        RB2D.velocity = new Vector2(RB2D.velocity.x, Mathf.Clamp(RB2D.velocity.y, -WallSlidingSpeed, float.MaxValue));
    }
    void JumpFromWall()
    {
        RB2D.velocity = new Vector2(xWallForce * -input, yWallForce);
        doJumpFromWall = false;
    }
    void Dash()
    {
        if (wallTouchingValidator.position.x > platformTouchingValidator.position.x == true)
        {
            StartCoroutine(Dash(1));
        }
        else
        {
            StartCoroutine(Dash(-1));
        }
    }
    void FlipHeroSprite()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        isFacingRight =! isFacingRight;
    }
    void SetWallJumpingToFalse()
    {
         isJumpingFromWall = false;
    }
    public void TakeDamage(int damage)
    {
        anim.SetTrigger("GettingDamage");
        Health -= damage;
        StartCoroutine(TemporaryGodmode());
        Instantiate(Blood, transform.position, Quaternion.identity);

        SoundManager.Instance.PlayPlayerEffects(getDamagedSound);
    }
    IEnumerator Die()
    {
        SoundManager.Instance.MuteDespiteMusic();
        SoundManager.Instance.PlayMusic(deathSound);
        anim.SetTrigger("Dying");
        yield return new WaitForSeconds(0.80f);
        Destroy(gameObject);

        Time.timeScale = 0;
    }
    IEnumerator JumpOff()
    {
        SoundManager.Instance.PlayPlayerEffects(jumpedDownSound);

        Physics2D.IgnoreCollision(playerCollider, mapCollider, true);
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreCollision(playerCollider, mapCollider, false);
        doJumpDown = false;
    }
    IEnumerator TemporaryGodmode()
    {
        int PlayerLayer = LayerMask.NameToLayer("Player");
        int EnemyLayer = LayerMask.NameToLayer("Enemy");

        SoundManager.Instance.MusicSource.pitch = 1.5f;

        anim.SetTrigger("GodModeOn");
        Physics2D.IgnoreLayerCollision(PlayerLayer, EnemyLayer, true);
        yield return new WaitForSeconds(1.5f);
        Physics2D.IgnoreLayerCollision(PlayerLayer, EnemyLayer, false);

        SoundManager.Instance.MusicSource.pitch = 1f;
    }
    IEnumerator Dash(float Direction)
    {
        anim.SetTrigger("Dashing");
        isDashing = true;
        canDash = false;
        doDash = false;

        RB2D.velocity = new Vector2(RB2D.velocity.x, 0f);
        RB2D.AddForce(new Vector2(dashingPower * Direction, 0f), ForceMode2D.Impulse);
        float originalGravity = RB2D.gravityScale;
        RB2D.gravityScale = 0;
        yield return new WaitForSeconds(dashingTime);
        RB2D.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    // Showing attack range of Player
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackValidator.position, AttackRange);
    }
}
