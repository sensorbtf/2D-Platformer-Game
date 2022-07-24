using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    // Singletone instance

    public static Player Instance { get; private set; }

    // Fundamental Player fields
    [SerializeField]
    private int _health = 3;
    [SerializeField]
    private int _damage = 1;
    [SerializeField]
    private float _speed = 5;
    [SerializeField]
    private float _jumpForce = 30;
    [SerializeField]
    private Rigidbody2D RB2D;
    [SerializeField]
    private float _wallSlidingSpeed;
    [SerializeField]
    private float _timeBetweenAtacks; 
    private float _nextAttackTime;
    [SerializeField]
    private float _attackRange;

    [SerializeField]
    private GameObject Blood;

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
    public Collider2D enemyCollider;

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
                RB2D.constraints = RigidbodyConstraints2D.FreezePosition;
                StartCoroutine(Die());
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

        if (Input.GetKeyDown(KeyCode.UpArrow) && _isSlidingWall == true)
        {
            _isJumpingFromWall = true;
            Invoke("SetWallJumpingToFalse", _wallJumpTime);
        }

        if (_isJumpingFromWall == true)
        {
            RB2D.velocity = new Vector2(_xWallForce * -input, _yWallForce);
        }

        if (Time.time > _nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(_attackValidator.position, AttackRange, _whatAreEnemies);
                StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
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
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        _isFacingRight = !_isFacingRight;
    }
    // Wall jumping ture/false
    void SetWallJumpingToFalse()
    {
        _isJumpingFromWall = false;
    }
    public void TakeDamage(int damage)
    {
        anim.SetTrigger("GettingDamage");
        Health -= damage;
        StartCoroutine(TemporaryGodmode());
        Instantiate(Blood, transform.position, Quaternion.identity);
    }
    public void PushBack(float pushBackForce)
    {
        Vector2 direction = (transform.position - PatrolEnemy.Instance.transform.position).normalized;
        RB2D.AddForce(direction * pushBackForce);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackValidator.position,AttackRange);
    }
    IEnumerator Die()
    {
        anim.SetTrigger("Dying");
        yield return new WaitForSeconds(0.80f);
        Destroy(gameObject);
    }
    //Method for jumping off from platform
    IEnumerator JumpOff()
    {
        Physics2D.IgnoreCollision(playerCollider, mapCollider, true);
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreCollision(playerCollider, mapCollider, false);
    }
    //Method for temporary godmode
    IEnumerator TemporaryGodmode()
    {
        anim.SetTrigger("GodModeOn");
        Physics2D.IgnoreLayerCollision(6, 11, true);
        yield return new WaitForSeconds(1.5f); // DODAÆ WYSZUKIWANIE warstw po nazwie i pozmidniæ ¿eby nie by³o magic numbers
        Physics2D.IgnoreLayerCollision(6, 11, false);
    }
}
