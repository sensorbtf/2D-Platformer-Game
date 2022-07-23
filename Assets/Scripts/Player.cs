using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Singletone instance

    public static Player Instance { get; private set; }

    // Fundamental Player fields
    [SerializeField]
    private int _health;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private Rigidbody2D RB2D;
    [SerializeField]
    private float _wallSlidingSpeed;
    [SerializeField]
    private float _timeBetweenAtacks; 
    private float _nextAttackTime;
    [SerializeField]
    private float _attackRange;


    // Hero state Checkers
    private bool _isFacingRight = true;
    private bool _isGrounded;
    private bool _isTouchingWall;
    private bool _isSlidingWall;

    // Hero move checkers

    [SerializeField]
    private Transform _platformTouchingValidator;
    [SerializeField]
    private Transform _wallTouchingValidator;
    [SerializeField]
    private Transform _attackValidator;

    [SerializeField]
    private float _radiousChecker;
    [SerializeField]
    private LayerMask _whatIsPlatform;
    [SerializeField]
    private LayerMask _whatAreWallsAndCeiling;
    [SerializeField]
    private LayerMask _whatAreEnemies;

    public Collider2D playerCollider;
    public Collider2D mapCollider;

    // Wall-jumping fields
    private bool _isJumpingFromWall;
    [SerializeField]
    private float _xWallForce;
    [SerializeField]
    private float _yWallForce;
    [SerializeField]
    private float _wallJumpTime;

    // Animator

    private Animator anim;
    private static Player instance;

    // Properties for most used fields
    public float Speed
    {
        get { return _speed; }
        set { _speed = value < 0 ? _speed = 0 : _speed = value; }
    }
    public float JumpForce
    {
        get { return _jumpForce; }
        set { _jumpForce = value < 0 ? _jumpForce = 0 : _jumpForce = value; }
    }
    public float WallSlidingSpeed
    {
        get { return _wallSlidingSpeed; }
        set { _wallSlidingSpeed = value < 0 ? _wallSlidingSpeed = 0 : _wallSlidingSpeed = value; }
    }
    public float TimeBetweenAtacks
    {
        get { return _timeBetweenAtacks; }
        set { _timeBetweenAtacks = value < 0 ? _timeBetweenAtacks = 0 : _timeBetweenAtacks = value; }
    }
    public float AttackRange
    {
        get { return _attackRange; }
        set { _attackRange = value < 0 ? _attackRange = 0 : _attackRange = value; }
    }
    public int Damage
    {
        get { return _damage; }
        set { _damage = value < 0 ? _damage = 0 : _damage = value; }
    }
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            if (value <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
        float input = Input.GetAxisRaw("Horizontal");
        RB2D.velocity = new Vector2(input * _speed, RB2D.velocity.y);

        _isGrounded = Physics2D.OverlapCircle(_platformTouchingValidator.position, _radiousChecker, _whatIsPlatform);
        _isTouchingWall = Physics2D.OverlapCircle(_wallTouchingValidator.position, _radiousChecker, _whatAreWallsAndCeiling);

        if (Input.GetKeyDown(KeyCode.UpArrow) && _isGrounded == true)
        {
            RB2D.velocity = Vector2.up * JumpForce;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && _isGrounded == true)
        {
            StartCoroutine(JumpOff());
        }

        if (Time.time > _nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(_attackValidator.position, AttackRange, _whatAreEnemies);

                foreach (Collider2D enemies in enemiesToDamage)
                {
                    PatrolEnemy.Instance.TakeDamage(Damage);
                }

                anim.SetTrigger("Attacking");
                _nextAttackTime = Time.time + TimeBetweenAtacks;
            }
        }

        if (input > 0 && _isFacingRight == false)
        {
            FlipHero();
        }
        else if (input < 0 && _isFacingRight == true)
        {
            FlipHero();
        }

        if (_isTouchingWall == true && _isGrounded == false && input != 0)
        {
            _isSlidingWall = true;
        }
        else
        {
            _isSlidingWall = false;
        }

        if (_isSlidingWall)
        {
            RB2D.velocity = new Vector2(RB2D.velocity.x, Mathf.Clamp(RB2D.velocity.y, -WallSlidingSpeed, float.MaxValue));
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && _isSlidingWall == true)
        {
            _isJumpingFromWall = true;
            Invoke("SetWallJumpingToFalse", _wallJumpTime);
        }

        if (_isJumpingFromWall == true)
        {
            RB2D.velocity = new Vector2(_xWallForce * -input, _yWallForce);
        }

        // animations
        if (input != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        if (_isGrounded == true)
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


    // Flipping sprite from left to right
    void FlipHero()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = !_isFacingRight;
    }
    // Wall jumping ture/false
    void SetWallJumpingToFalse()
    {
        _isJumpingFromWall = false;
    }
    //Method for jumping off from platform
    IEnumerator JumpOff()
    {
        Physics2D.IgnoreCollision(playerCollider, mapCollider, true);
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreCollision(playerCollider, mapCollider, false);
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackValidator.position,AttackRange);
    }
}
