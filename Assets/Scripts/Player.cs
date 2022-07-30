using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour, ICharacters, IPlayer
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

    // Layers
    private int playerLayerIndex;
    private int enemyLayerIndex;
    private int projectileLayerIndex;

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

    private bool  canDash = true;
    private bool  isDashing = false;
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
    private bool  isImmune = false;

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
    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (value <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }
    public int Damage
    {
        get => damage;
        set { damage = value < 0 ? damage = 0 : damage = value; }
    }
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
    public bool IsImmune
    {
        get => isImmune;
        set { isImmune = value; }
    }
    private void Start()
    {
        rB2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();

        playerLayerIndex = LayerMask.NameToLayer("Player");
        enemyLayerIndex = LayerMask.NameToLayer("Enemy");
        projectileLayerIndex = LayerMask.NameToLayer("Projectiles");
}
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    private void Update()
    {
        Debug.Log("TT" + isImmune);
        if (isDashing )
            return;
        if (Health <= 0)
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

        if (doDash)
            Dash();

        if (doJump)
            Jump();

        if (doJumpDown)
            StartCoroutine(JumpOff());

        if (isSlidingWall)
            SlideOnWall();

        if (doJumpFromWall)
            JumpFromWall();
    }
    private void HeroState()
    {
        if (input > 0 && !isFacingRight)
            FlipHeroSprite();
        else if (input < 0 && isFacingRight)
            FlipHeroSprite();

        if (isTouchingWall  && !isGrounded  && input != 0)
            isSlidingWall = true;
        else
            isSlidingWall = false;
    }

    private void HeroStateAnimations()
    {
        if (input != 0 && isGrounded )
        {
            anim.SetBool("isRunning", true);
            if (input != 0 && !SoundManager.Instance.PlayerWalkingSource.isPlaying)
            {
                SoundManager.Instance.PlayWalkingEffect(runningSound);
                SoundManager.Instance.PlayerWalkingSource.Play();
            }
            else if (input == 0 || !isGrounded)
                SoundManager.Instance.PlayerWalkingSource.Stop();
        }
        else
            anim.SetBool("isRunning", false);


        if (isGrounded)
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
                anim.SetBool("isFalling", false);
        }
    }
    private void PlayerPositionChecker()
    {
        isGrounded = Physics2D.OverlapCircle(platformTouchingValidator.position, radiousChecker, whatIsPlatform);
        isTouchingWall = Physics2D.OverlapCircle(wallTouchingValidator.position, radiousChecker, whatAreWallsAndCeiling);
    }
    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            doJump = true;

        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded)
            doJumpDown = true;

        if (Input.GetKeyDown(KeyCode.UpArrow) && isSlidingWall)
        {
            isJumpingFromWall = true;
            Invoke(nameof(SetWallJumpingToFalse), wallJumpTime);

            SoundManager.Instance.PlayPlayerEffects(jumpSound);
        }

        if (isJumpingFromWall )
            doJumpFromWall = true;

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isGrounded)
            doDash = true;

        if (Time.time > nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isImmune)
            {
                StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
                SoundManager.Instance.PlayPlayerEffects(attackSound);
                anim.SetTrigger("Attacking");
                nextAttackTime = Time.time + TimeBetweenAtacks;
            }
        }
    }
    private void Jump()
    {
        SoundManager.Instance.PlayPlayerEffects(jumpSound);

        RB2D.velocity = Vector2.up * JumpForce;
        doJump = false;
    }
    private void Attack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackValidator.position, AttackRange, whatAreEnemies);

        foreach (Collider2D enemies in enemiesToDamage)
            enemies.GetComponent<Enemy>().TakeDamage(damage);
    }
    private void SlideOnWall()
    {
        RB2D.velocity = new Vector2(RB2D.velocity.x, Mathf.Clamp(RB2D.velocity.y, -WallSlidingSpeed, float.MaxValue));
    }
    private void JumpFromWall()
    {
        RB2D.velocity = new Vector2(xWallForce * -input, yWallForce);
        doJumpFromWall = false;
    }
    private void Dash()
    {
        if (wallTouchingValidator.position.x > platformTouchingValidator.position.x)
            StartCoroutine(Dash(1));
        else
            StartCoroutine(Dash(-1));
    }
    private void FlipHeroSprite()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        isFacingRight =! isFacingRight;
    }
    private void SetWallJumpingToFalse()
    {
         isJumpingFromWall = false;
    }
    public void TakeDamage(int damage)
    {
        if (isImmune)
            return;

        SoundManager.Instance.PlayPlayerEffects(getDamagedSound);

        Instantiate(Blood, transform.position, Quaternion.identity);

        if (Health > 0)
            StartCoroutine(TemporaryGodmode());

        anim.SetTrigger("GettingDamage");
        Health -= damage;
    }
    private IEnumerator Die()
    {
        SoundManager.Instance.MuteDespiteMusic();
        SoundManager.Instance.PlayMusic(deathSound);

        isImmune = false;
        rB2D.constraints = RigidbodyConstraints2D.FreezePosition;
        anim.SetTrigger("Dying");
        yield return new WaitForSeconds(0.80f);
        Destroy(gameObject);
        StopIgnoringCollisions();

        Time.timeScale = 0;
    }
    private IEnumerator JumpOff()
    {
        SoundManager.Instance.PlayPlayerEffects(jumpedDownSound);

        Physics2D.IgnoreCollision(playerCollider, mapCollider, true);
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreCollision(playerCollider, mapCollider, false);
        doJumpDown = false;
    }
    private IEnumerator TemporaryGodmode()
    {
        SoundManager.Instance.MusicSource.pitch = 1.5f;

        isImmune = true;
        anim.SetTrigger("GodModeOn");
        StartIgnoringCollisions();
        yield return new WaitForSeconds(1.5f);
        StopIgnoringCollisions();
        isImmune = false;

        SoundManager.Instance.MusicSource.pitch = 1f;
    }
    private IEnumerator Dash(float Direction)
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
    private void StartIgnoringCollisions()
    {
        Physics2D.IgnoreLayerCollision(playerLayerIndex, enemyLayerIndex, true);
        Physics2D.IgnoreLayerCollision(playerLayerIndex, projectileLayerIndex, true);
    }
    private void StopIgnoringCollisions()
    {
        Physics2D.IgnoreLayerCollision(playerLayerIndex, enemyLayerIndex, false);
        Physics2D.IgnoreLayerCollision(playerLayerIndex, projectileLayerIndex, false);
    }
    // Showing attack range of Player
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackValidator.position, AttackRange);
    }
}
