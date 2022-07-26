using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrolEnemy : Enemy
{
    [SerializeField] private float speed = 2;
    [SerializeField] private float  startWaitTime = 1.5f;
    [SerializeField] private Transform[] pointsOfPatrol;

    private float  PatrolWaitTime;
    private int  currentPointIndex;
    private bool isFacingRight;

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
        {
            GetComponent<Collider2D>().enabled = false;
            return;
        }
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

            if (transform.position == pointsOfPatrol[currentPointIndex].position == false && !SoundManager.Instance.EnemyEffectsSource.isPlaying)
            {
                SoundManager.Instance.PlayEnemyEffects(runningSound);
                SoundManager.Instance.EnemyEffectsSource.Play();
            }
            else if (transform.position == pointsOfPatrol[currentPointIndex].position)
                SoundManager.Instance.EnemyEffectsSource.Stop();
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (anim.parameters.Any(x => x.name == "Attacking"))
                {
                anim.SetTrigger("Attacking");
                }
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
            Player.Instance.TakeDamage(Damage);
            PushBack(PushBackForce);
            SoundManager.Instance.PlayEnemyEffects(pushBackSound);
        }
    }
    void FlipEnemy()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        isFacingRight = !isFacingRight;
    }

}
