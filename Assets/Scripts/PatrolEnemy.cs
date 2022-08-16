using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PatrolEnemy : Enemy
{
    [Header("PatrolEnemy")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float startWaitTime = 1.5f;
    [SerializeField] private Transform[] pointsOfPatrol;

    private float PatrolWaitTime;
    private int currentPointIndex;
    private bool isFacingRight;

    private SoundManager soundManager;


    // Properties

    public float Speed
    {
        get => speed;
        set { speed = value < 0 ? speed = 0 : speed = value; }
    }
    private void Start()
    {
        transform.position = pointsOfPatrol[0].position;
        PatrolWaitTime = startWaitTime;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Health <= 0)
            return;

        transform.position = Vector2.MoveTowards(transform.position, pointsOfPatrol[currentPointIndex].position, Speed * Time.deltaTime);

        if (transform.position == pointsOfPatrol[currentPointIndex].position)
        {
            anim.SetBool("isRunning", false);
            if (PatrolWaitTime <= 0)
            {
                if (currentPointIndex + 1 < pointsOfPatrol.Length)
                {
                    currentPointIndex++;
                    FlipEnemy();
                }
                else
                {
                    currentPointIndex = 0;
                    FlipEnemy();
                }
                PatrolWaitTime = startWaitTime;
            }
            else
            {
                PatrolWaitTime -= Time.deltaTime;
            }
        }
        else
        {
            anim.SetBool("isRunning", true);

            WalkingSoundEffect();
        }
    }

    public Vector2 moveDirectionPush;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
            Player.Instance.TakeDamage(Damage);
            PushBack(PushBackForce);
        }
    }
    private void FlipEnemy()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        isFacingRight = !isFacingRight;
    }
    private void WalkingSoundEffect()
    {
<<<<<<< HEAD
        if (transform.position == pointsOfPatrol[currentPointIndex].position == false && !soundManager.enemyEffectsSource.isPlaying)
=======
        if (transform.position != pointsOfPatrol[currentPointIndex].position && !SoundManager.Instance.EnemyEffectsSource.isPlaying)
>>>>>>> 13d2e926e68419e1974194969abaa700615ff344
        {
            soundManager.PlayEnemyEffects(runningSound);
            soundManager.enemyEffectsSource.Play();
        }
        else if (transform.position == pointsOfPatrol[currentPointIndex].position)
            soundManager.enemyEffectsSource.Stop();
    }

    [Inject]
    public void construct(SoundManager sM)
    {
        soundManager = sM;
    }
}
