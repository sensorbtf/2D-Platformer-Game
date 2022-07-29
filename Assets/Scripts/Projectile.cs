using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float lifeTime;

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
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
            SoundManager.Instance.PlayEnviromentEffects(bombExplode);
        }

    }
    private void DirectionOfAttack()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.right);
    }
}
