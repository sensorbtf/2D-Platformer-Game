using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Fundamental Player fields
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private Rigidbody2D RB2D;
    [SerializeField]
    private float _wallSlidingSpeed;

    // Hero state Checkers
    private bool _isFacingRight = true;
    private bool _isGrounded;
    private bool _isTouchingWall;
    private bool _isSlidingWall;

    // Hero move checkers

    [SerializeField]
    private Transform _platformTouchingValidator;
    [SerializeField]
    private Transform _WallTouchingValidator;

    [SerializeField]
    private float _radiousChecker;
    [SerializeField]
    private LayerMask _whatIsPlatform;

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
    private void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        RB2D.velocity = new Vector2(input * _speed, RB2D.velocity.y);

        _isGrounded = Physics2D.OverlapCircle(_platformTouchingValidator.position, _radiousChecker, _whatIsPlatform);

        if (Input.GetKeyDown(KeyCode.UpArrow) && _isGrounded == true)
        {
            RB2D.velocity = Vector2.up * JumpForce;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && _isGrounded == true)
        {
            StartCoroutine(JumpOff());
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
    }

    void FlipHero()
    {
       transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
       _isFacingRight =! _isFacingRight;
    }
    void SetWallJumpingToFalse()
    {
        _isJumpingFromWall = false;
    }
    //Method for Moving From Platform
    IEnumerator JumpOff()
    {
        Physics2D.IgnoreCollision(playerCollider, mapCollider, true);
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreCollision(playerCollider, mapCollider, false);
    }
}