using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour, ICharacters, IPlayer
{
    // Singletone instance
    public static Player Instance { get; private set; } 

    private Rigidbody2D rB2D;
    private Collider2D playerCollider;
    private Animator anim;
    private float input;
    private SoundManager soundManager;

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
    private readonly float dashingPower = 100;
    private readonly float dashingTime = 0.2f;
    private readonly float dashingCooldown = 1f;

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
        if (isDashing )
            return;
        if (PlayerConfig.Instance.Health <= 0)
            return;

        input = Input.GetAxisRaw("Horizontal");

        PlayerPositionChecker();
        PlayerInput();
        HeroState();
        HeroStateAnimations();
    }
    private void FixedUpdate()
    {
        RB2D.velocity = new Vector2(input * PlayerConfig.Instance.Speed, RB2D.velocity.y);

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
            if (input != 0 && !soundManager.PlayerWalkingSource.isPlaying)
            {
                soundManager.PlayWalkingEffect(runningSound);
                soundManager.PlayerWalkingSource.Play();
            }
            else if (input == 0 || !isGrounded)
                soundManager.PlayerWalkingSource.Stop();
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
            Invoke(nameof(SetWallJumpingToFalse), PlayerConfig.Instance.WallSlidingSpeed);

            soundManager.PlayPlayerEffects(jumpSound);
        }

        if (isJumpingFromWall)
            doJumpFromWall = true;

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isGrounded)
            doDash = true;

        if (!(Time.time > nextAttackTime)) return;
        if (!Input.GetKeyDown(KeyCode.Space) || isImmune) return;
        
        StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
        soundManager.PlayPlayerEffects(attackSound);
        anim.SetTrigger("Attacking");
        nextAttackTime = Time.time + PlayerConfig.Instance.TimeBetweenAttacks;
    }
    private void Jump()
    {
        soundManager.PlayPlayerEffects(jumpSound);

        RB2D.velocity = Vector2.up * PlayerConfig.Instance.JumpForce;
        doJump = false;
    }
    private void Attack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackValidator.position, PlayerConfig.Instance.AttackRange, whatAreEnemies);

        foreach (Collider2D enemies in enemiesToDamage)
            enemies.GetComponent<Enemy>().TakeDamage(PlayerConfig.Instance.Damage);
    }
    private void SlideOnWall()
    {
        RB2D.velocity = new Vector2(RB2D.velocity.x, Mathf.Clamp(RB2D.velocity.y, -PlayerConfig.Instance.WallSlidingSpeed, float.MaxValue));
    }
    private void JumpFromWall()
    {
        RB2D.velocity = new Vector2(PlayerConfig.Instance.XWallForce * -input, PlayerConfig.Instance.YWallForce);
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

        soundManager.PlayPlayerEffects(getDamagedSound);

        Instantiate(Blood, transform.position, Quaternion.identity);

        if (PlayerConfig.Instance.Health > 0)
            StartCoroutine(TemporaryGodmode());

        anim.SetTrigger("GettingDamage");
        PlayerConfig.Instance.Health -= damage;
    }
    public IEnumerator Die()
    {
        soundManager.MuteDespiteMusic();
        soundManager.PlayMusic(deathSound);

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
        soundManager.PlayPlayerEffects(jumpedDownSound);

        Physics2D.IgnoreCollision(playerCollider, mapCollider, true);
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreCollision(playerCollider, mapCollider, false);
        doJumpDown = false;
    }
    private IEnumerator TemporaryGodmode()
    {
        soundManager.MusicSource.pitch = 1.5f;

        isImmune = true;
        anim.SetTrigger("GodModeOn");
        StartIgnoringCollisions();
        yield return new WaitForSeconds(1.5f);
        StopIgnoringCollisions();
        isImmune = false;

        soundManager.MusicSource.pitch = 1f;
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
        Gizmos.DrawWireSphere(attackValidator.position, PlayerConfig.Instance.AttackRange);
    }
    [Inject]
    public void construct(SoundManager sM)
    {
        soundManager = sM;
    }
}
