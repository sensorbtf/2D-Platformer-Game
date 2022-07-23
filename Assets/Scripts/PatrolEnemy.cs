using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public static PatrolEnemy Instance { get; private set; }

    [SerializeField]
    private int _health;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private float _pushBackForce;
    [SerializeField]
    private float _startWaitTime;

    [SerializeField]
    private Transform[] pointsOfPatrol;

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
    public float PushBackForce
    {
        get { return _pushBackForce * 1000; }
        set { _pushBackForce = value < 0 ? _pushBackForce = 0 : _pushBackForce = value; }
    }
    public int Health
    {
        get { return _health; }
        set 
        { 
            _health = value < 0 ? _health = 0 : _health = value;
            if (value <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }

    private void Start()
    {
        transform.position = pointsOfPatrol[0].position;
        anim = GetComponent<Animator>();
        _PatrolWaitTime = _startWaitTime;
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

    void FlipEnemy()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = !_isFacingRight;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.TakeDamage(Damage);
            Player.Instance.PushBack(PushBackForce);
        }
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        anim.SetTrigger("GettingDamage");
    }
    IEnumerator Die()
    {
        anim.SetTrigger("Dying");
        yield return new WaitForSeconds(0.80f);
        Destroy(gameObject);
    }
}
