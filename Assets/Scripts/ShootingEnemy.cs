using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : Enemy
{
    [Header("Projectile parameters")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private Transform shotPoint;

    private float nextShotTime;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        DeActivateColliderOnDeath();
        if (Time.time > nextShotTime)
        {
            anim.SetTrigger("Attacking");
            nextShotTime = Time.time + timeBetweenShots;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
            Player.Instance.TakeDamage(Damage);
            SoundManager.Instance.PlayEnemyEffects(pushBackSound);
        }
    }
    void FireProjectile()
    {
        Instantiate(projectile, shotPoint.position, shotPoint.rotation);
    }
    void DeActivateColliderOnDeath()
    {
        if (Health <= 0)
        {
            GetComponent<Collider2D>().enabled = false;
            return;
        }
    }
}
