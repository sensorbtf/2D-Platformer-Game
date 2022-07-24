using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{
    [SerializeField]
    private float speed = 2;
    [SerializeField]
    private float  startWaitTime = 1.5f;

    [SerializeField]
    private float attackRange;
    [SerializeField]
    private Transform AttackValidator;
    [SerializeField]
    private Transform[] pointsOfPatrol;

    private float  PatrolWaitTime;
    private int  currentPointIndex;

    private bool isFacingRight;

    // Properties

    public float Speed
    {
        get { return  speed; }
        set {  speed = value < 0 ?  speed = 0 :  speed = value; }
    }


    private void Start()
    {
        transform.position = pointsOfPatrol[0].position;
        PatrolWaitTime =  startWaitTime;
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
        if (AttackValidator != null)
        {
            Collider2D[] playerToDamage = Physics2D.OverlapCircleAll(AttackValidator.position, attackRange, LayerMask.NameToLayer("Player"));
            foreach (Collider2D player in playerToDamage)
            {
                player.GetComponent<Player>().TakeDamage(Damage);
            } //?
            anim.SetTrigger("Attacking");
            Debug.Log("ATTACKED!");
            SoundManager.Instance.PlayPlayerEffects(pushBackSound);
        }
    }
    void FlipEnemy()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        isFacingRight = !isFacingRight;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackValidator.position, attackRange);
    }

}
