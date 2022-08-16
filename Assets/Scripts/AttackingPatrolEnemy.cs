using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingPatrolEnemy : MonoBehaviour
{
    [Header("AttackingEnemy")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackColdown = 1.5f;

    private float nextAttack = 0.2f;

    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] Transform player;
    [SerializeField] Transform attackValidator;

    private Animator anim;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (player != null && attackValidator != null && attackColdown != 0)
        {
            AttackPlayer();
        }
    }
    void AttackPlayer()
    {
        if (Vector2.Distance(player.position, attackValidator.position) <= attackRange && Time.time > nextAttack)
        {
            Collider2D[] playerToDamage = Physics2D.OverlapCircleAll(attackValidator.position, attackRange, whatIsPlayer);
            foreach (Collider2D player in playerToDamage)
            {
                player.GetComponent<Player>().TakeDamage(attackDamage);
            }
            anim.SetTrigger("Attacking");
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
            nextAttack = Time.time + attackColdown;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackValidator.position, attackRange);
    }
}
