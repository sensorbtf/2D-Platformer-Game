using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    // Singletone instance

    public static Player Instance { get; private set; }

    // Fundamental Player fields
    [SerializeField]    private int  health = 3;
    [SerializeField]    private int  damage = 1;
    [SerializeField]    private float  speed = 5;
    [SerializeField]    private float  jumpForce = 30;
    [SerializeField]    private float  wallSlidingSpeed = 3;
    [SerializeField]    private float  timeBetweenAttacks = 0.4f; 
    [SerializeField]    private float  attackRange = 0.5f;
    
    private Rigidbody2D rB2D;
    private float input;

    [SerializeField]    private GameObject Blood;

    // Hero Dashing

    private bool canDash = true;
    private float dashingPower = 100;
    private readonly float dashingTime = 0.2f;
    private float dashingCooldown = 0.5f;

    private float doubleTapTime;
    private KeyCode lastKeyCode;

    // Hero state Checkers
    private bool  isFacingRight = true;
    private bool  isGrounded;
    private bool  isTouchingWall;
    private bool  isSlidingWall;
    private float nextAttackTime;

    // Hero move checkers

    [SerializeField]    private Transform  platformTouchingValidator;
    [SerializeField]    private Transform  wallTouchingValidator;
    [SerializeField]    private Transform  attackValidator;

    [SerializeField]    private float  radiousChecker;
    [SerializeField]    private LayerMask  whatIsPlatform;
    [SerializeField]    private LayerMask  whatAreWallsAndCeiling;
    [SerializeField]    private LayerMask  whatAreEnemies;

    [SerializeField]    private  Collider2D playerCollider;
    [SerializeField]    private Collider2D mapCollider;

    // Wall-jumping fields
    private bool  isJumpingFromWall;
    [SerializeField]    private float  xWallForce = 5;
    [SerializeField]    private float  yWallForce = 20;
    [SerializeField]    private float  wallJumpTime = 0.1f;

    // Animator
    private Animator anim;

    // Sound

    AudioSource audioSource;

    [SerializeField]    private AudioClip jumpSound;
    [SerializeField]    private AudioClip runningSound;
    [SerializeField]    private AudioClip getDamagedSound;
    [SerializeField]    private AudioClip jumpedDownSound;
    [SerializeField]    private AudioClip attackSound;
    [SerializeField]    private AudioClip deathSound;

    // Properties for most used fields
    public Rigidbody2D RB2D { get => rB2D; set => rB2D = value; }
    public float Speed
    {
        get => speed;
        set {  speed = value < 0 ?  speed = 0 : speed = value; }
    }
    public float JumpForce
    {
        get => jumpForce;
        set {  jumpForce = value < 0 ?  jumpForce = 0 : jumpForce = value; }
    }
    public float WallSlidingSpeed
    {
        get => wallSlidingSpeed;
        set {  wallSlidingSpeed = value < 0 ?  wallSlidingSpeed = 0 : wallSlidingSpeed = value; }
    }
    public float TimeBetweenAtacks
    {
        get => timeBetweenAttacks;
        set { timeBetweenAttacks = value < 0 ? timeBetweenAttacks = 0 : timeBetweenAttacks = value; }
    }
    public float AttackRange
    {
        get => attackRange;
        set {  attackRange = value < 0 ? attackRange = 0 : attackRange = value; }
    }
    public int Damage
    {
        get => damage;
        set {  damage = value < 0 ?  damage = 0 : damage = value; }
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

        audioSource = GetComponent<AudioSource>();
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
        input = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle( platformTouchingValidator.position,  radiousChecker,  whatIsPlatform);
        isTouchingWall = Physics2D.OverlapCircle( wallTouchingValidator.position,  radiousChecker,  whatAreWallsAndCeiling);

        if (Input.GetKeyDown(KeyCode.UpArrow) &&  isGrounded == true)
        {
            RB2D.velocity = Vector2.up * JumpForce;

            SoundManager.Instance.PlayPlayerEffects(jumpSound);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) &&  isGrounded == true)
        {
            StartCoroutine(JumpOff());
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) &&  isSlidingWall == true)
        {
            isJumpingFromWall = true;
            Invoke("SetWallJumpingToFalse",  wallJumpTime);

            SoundManager.Instance.PlayPlayerEffects(jumpSound);
        }
        if ( isJumpingFromWall == true)
        {
            RB2D.velocity = new Vector2( xWallForce * -input,  yWallForce);
        }
        if (Time.time > nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
               
                StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));


                anim.SetTrigger("Attacking");
                nextAttackTime = Time.time + TimeBetweenAtacks;

                SoundManager.Instance.PlayPlayerEffects(attackSound);
            }
        }

        if (input > 0 &&  isFacingRight == false)
        {
            FlipHero();
        }
        else if (input < 0 &&  isFacingRight == true)
        {
            FlipHero();
        }

        if ( isTouchingWall == true &&  isGrounded == false && input != 0)
        {
             isSlidingWall = true;
        }
        else
        {
             isSlidingWall = false;
        }

        if ( isSlidingWall)
        {
            RB2D.velocity = new Vector2(RB2D.velocity.x, Mathf.Clamp(RB2D.velocity.y, -WallSlidingSpeed, float.MaxValue));
        }

        // animations
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
        if ( isGrounded == true)
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
    private void FixedUpdate()
    {
        RB2D.velocity = new Vector2(input * speed, RB2D.velocity.y);

        if (Input.GetKeyDown(KeyCode.RightArrow) && canDash == true && isGrounded == false)
        {
            if (doubleTapTime > Time.time && lastKeyCode == KeyCode.RightArrow)
            {
                StartCoroutine(Dash(1));
            }
            else
            {
                doubleTapTime = Time.time + 0.5f;
            }

            lastKeyCode = KeyCode.RightArrow;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && canDash == true && isGrounded == false)
        {
            if (doubleTapTime > Time.time && lastKeyCode == KeyCode.LeftArrow)
            {
                StartCoroutine(Dash(-1));
            }
            else
            {
                doubleTapTime = Time.time + 0.5f;
            }
            lastKeyCode = KeyCode.LeftArrow;
        }
    }
    // Hero attack
    public void Attack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackValidator.position, AttackRange, whatAreEnemies);
        foreach (Collider2D enemies in enemiesToDamage)
        {
            enemies.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    // Flipping sprite from left to right
    void FlipHero()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
         isFacingRight = ! isFacingRight;
    }
    // Wall jumping ture/false
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

    //Method for jumping off from platform
    IEnumerator JumpOff()
    {
        SoundManager.Instance.PlayPlayerEffects(jumpedDownSound);

        Physics2D.IgnoreCollision(playerCollider, mapCollider, true);
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreCollision(playerCollider, mapCollider, false);
    }
    //Method for temporary godmode
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
        Debug.Log("Test");

        canDash = false;
        RB2D.velocity = new Vector2(RB2D.velocity.x, 0f);
        RB2D.AddForce(new Vector2(dashingPower * Direction, 0f), ForceMode2D.Impulse);
        float originalGravity = RB2D.gravityScale;
        RB2D.gravityScale = 0;

        yield return new WaitForSeconds(dashingCooldown);
        RB2D.gravityScale = originalGravity;
        canDash = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackValidator.position, AttackRange);
    }

}
