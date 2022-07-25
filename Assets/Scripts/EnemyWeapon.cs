using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : Enemy
{
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            anim.SetTrigger("Attacking");
            Player.Instance.TakeDamage(Damage);
            SoundManager.Instance.PlayPlayerEffects(pushBackSound);
        }
    }
}
