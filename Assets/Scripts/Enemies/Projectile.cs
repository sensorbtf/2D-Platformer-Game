using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float lifeTime;

    private SoundManager soundManager;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        DirectionOfAttack();
    }

    [Header("Sounds")]
    [SerializeField] private AudioClip bombExplode;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
            Player.Instance.TakeDamage(damage);
            soundManager.PlayEnviromentEffects(bombExplode);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
            soundManager.PlayEnviromentEffects(bombExplode);
        }

    }
    private void DirectionOfAttack()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.right);
    }

    [Inject]
    public void construct(SoundManager sM)
    {
        soundManager = sM;
    }

    readonly ShootingEnemy _shootingEnemy;

    public Projectile(ShootingEnemy shootingenemy)
    {
        _shootingEnemy = shootingenemy;
    }

    public class Factory : PlaceholderFactory<Projectile>
    {

    }
}

public class ProjectileSpawner : ITickable
{
    readonly Projectile.Factory _projectileFactory;

    public ProjectileSpawner(Projectile.Factory enemyFactory)
    {
        _projectileFactory = enemyFactory;
    }

    public void Tick()
    {
        //if (ShouldSpawnNewEnemy())
        //{
            var projectile = _projectileFactory.Create();
        //}
    }
}