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

    bool _isFacingRight = true;

    bool isGrounded;
    [SerializeField]
    private Transform _platformTouchingValidator;
    [SerializeField]
    private float _radiousChecker;
    [SerializeField]
    private LayerMask _whatIsPlatform;

    private void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        RB2D.velocity = new Vector2(input * _speed, RB2D.velocity.y);

        isGrounded = Physics2D.OverlapCircle(_platformTouchingValidator.position, _radiousChecker, _whatIsPlatform);

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded == true)
        {
            RB2D.velocity = Vector2.up * _jumpForce;
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
