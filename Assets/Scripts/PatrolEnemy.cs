using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [SerializeField]
    private Transform[] pointsOfPatrol;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private float _startWaitTime;

    private float _PatrolWaitTime;
    private int _currentPointIndex;
    private bool _isFacingRight;

    // Animator

    private Animator anim;

    // Properties

    public float Speed
    {
        get { return _speed; }
        set { _speed = value < 0 ? _speed = 0 : _speed = value; }
    }
    public int Damage
    {
        get { return _damage; }
        set { _damage = value < 0 ? _damage = 0 : _damage = value; }
    }


    private void Start()
    {
        transform.position = pointsOfPatrol[0].position;
        anim = GetComponent<Animator>();
        _PatrolWaitTime = _startWaitTime;
    }
    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, pointsOfPatrol[_currentPointIndex].position, Speed * Time.deltaTime);

        if (transform.position == pointsOfPatrol[_currentPointIndex].position)
        {
            anim.SetBool("isRunning", false);
            if (_PatrolWaitTime <= 0)
            {
                if (_currentPointIndex + 1 < pointsOfPatrol.Length)
                {
                    _currentPointIndex++;
                    FlipEnemy();
                }
                else
                {
                    _currentPointIndex = 0;
                    FlipEnemy();
                }
                _PatrolWaitTime = _startWaitTime;
            }
            else
            {
                _PatrolWaitTime -= Time.deltaTime;
            }
        }
        else
        { 
            anim.SetBool("isRunning", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.TakeDamage(Damage);
        }
    }

    void FlipEnemy()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = !_isFacingRight;
    }
}
