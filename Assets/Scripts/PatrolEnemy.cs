using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public static PatrolEnemy Instance { get; private set; }

    [SerializeField]
    private int _health = 2;
    [SerializeField]
    private float _speed = 2;
    [SerializeField]
    private int _damage = 1;
    [SerializeField]
    private float _pushBackForce = 2.2f;
    [SerializeField]
    private float _startWaitTime = 1.5f;

    [SerializeField]
    private Transform[] pointsOfPatrol;

    private float _PatrolWaitTime;
    private int _currentPointIndex;
    private bool _isFacingRight;

    [SerializeField]
    private GameObject Blood;

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
        if (Health <= 0)
        {
            GetComponent<Collider2D>().enabled = false;
            return;
        }

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
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        _isFacingRight = !_isFacingRight;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
            Player.Instance.TakeDamage(Damage);
            Player.Instance.PushBack(PushBackForce);
        }
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        anim.SetTrigger("GettingDamage");
        Instantiate(Blood, transform.position, Quaternion.identity);
    }
    IEnumerator Die()
    {
        anim.SetTrigger("Dying");
        yield return new WaitForSeconds(0.80f);
        Destroy(gameObject);
    }
}
