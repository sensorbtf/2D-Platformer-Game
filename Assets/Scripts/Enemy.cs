using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ICharacters, IEnemy
{
    [Header("Enemy parameters")]
    [SerializeField] private int health = 2;
    [SerializeField] private int damage = 1;
    [SerializeField] private float pushBackForce = 2.2f;

    [Header("Coins reward")]
    [SerializeField] private int minimumCount = 1;
    [SerializeField] private int maximumCount = 2;
    [SerializeField] private GameObject coin = null;

    [Header("Effects")]
    [SerializeField] private GameObject Blood;

    //properties
    public int Health
    {
        get => health;
        set { health = value < 0 ? health = 0 : health = value; }
    }
    public int Damage
    {
        get => damage;
        set { damage = value < 0 ? damage = 0 : damage = value; }
    }
    public float PushBackForce
    {
        get => pushBackForce * 1000;
        set { pushBackForce = value < 0 ? pushBackForce = 0 : pushBackForce = value; }
    }

    protected Animator anim;

    [Header("Sounds")]
    [SerializeField] private AudioClip gettingDamageSound;
    [SerializeField] protected AudioClip pushBackSound;
    [SerializeField] protected AudioClip runningSound;
    [SerializeField] protected AudioClip dyingSound;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !Player.Instance.IsImmune)
        {
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
            Player.Instance.TakeDamage(Damage);
            SoundManager.Instance.PlayEnemyEffects(pushBackSound);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        anim.SetTrigger("GettingDamage");
        if (Health <= 0)
        {
            StartCoroutine(Die());
            SoundManager.Instance.PlayEnemyEffects(dyingSound);
        }
        else
        {
            Instantiate(Blood, transform.position, Quaternion.identity);
            SoundManager.Instance.PlayEnemyEffects(gettingDamageSound);
        }     
    }
    protected void PushBack(float pushBackForce)
    {
        Vector2 direction = (Player.Instance.RB2D.transform.position - transform.position).normalized;
        Player.Instance.RB2D.AddForce(direction * pushBackForce);
        SoundManager.Instance.PlayEnemyEffects(pushBackSound);
    }
    protected IEnumerator Die()
    {
        GetComponent<Collider2D>().enabled = false;
        anim.SetTrigger("Dying");
        yield return new WaitForSeconds(0.80f);
        Spawn();
        Destroy(gameObject);
    }

    protected virtual void Spawn()
    {
        float xPositionDifference = 0;
        int coinsCount = Random.Range(minimumCount, maximumCount);
        for (int i = 0; i < coinsCount; i++)
        {
            Instantiate(coin, new Vector3(transform.position.x + xPositionDifference, transform.position.y, 0), Quaternion.identity);
            xPositionDifference += 0.75f;
        }
    }
}