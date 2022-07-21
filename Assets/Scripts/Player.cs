using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private Rigidbody2D RB2D;


    private bool _isFacingRight = true;
    private bool _isGrounded;

    private bool isTouchingWall;
    [SerializeField]
    private Transform _WallTouchingValidator;
    private bool _isSlidingWall;
    [SerializeField]
    private float _wallSlidingSpeed;


    [SerializeField]
    private Transform _platformTouchingValidator;
    [SerializeField]
    private float _radiousChecker;
    [SerializeField]
    private LayerMask _whatIsPlatform;

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
        
        if (input > 0 && _isFacingRight == false)
        {
            FlipHero();
        }
        else if (input < 0 && _isFacingRight == true)
        {
            FlipHero();
        }
    }

    void FlipHero()
    {
       transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
       _isFacingRight =! _isFacingRight;
    }
}
