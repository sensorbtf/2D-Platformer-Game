using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //essential fields

    [SerializeField]
    private int health = 2;
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private float pushBackForce = 2.2f;

    [SerializeField]
    private GameObject Blood;

    //properties

    public int Health
    {
        get { return health; }
        set { health = value < 0 ? health = 0 : health = value; }
    }
    public int Damage
    {
        get { return damage; }
        set { damage = value < 0 ? damage = 0 : damage = value; }
    }
    public float PushBackForce
    {
        get { return pushBackForce * 1000; }
        set { pushBackForce = value < 0 ? pushBackForce = 0 : pushBackForce = value; }
    }

    // Animator
    [SerializeField]
    protected Animator anim;

    // Sound

    [SerializeField]
    private AudioClip gettingDamageSound;
    [SerializeField]
    protected AudioClip pushBackSound;
    [SerializeField]
    protected AudioClip runningSound;
    [SerializeField]
    protected AudioClip dyingSound;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
            Player.Instance.TakeDamage(Damage);
            PushBack(PushBackForce);
            SoundManager.Instance.PlayEnemyEffects(pushBackSound);
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        anim.SetTrigger("GettingDamage");
        if (Health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            Instantiate(Blood, transform.position, Quaternion.identity);
        }
        SoundManager.Instance.PlayEnemyEffects(gettingDamageSound);
    }

    public void PushBack(float pushBackForce)
    {
        Vector2 direction = (transform.position).normalized;
        Player.Instance.RB2D.AddForce(direction * pushBackForce);
    }

    IEnumerator Die()
    {
        SoundManager.Instance.PlayEnemyEffects(dyingSound);

        anim.SetTrigger("Dying");
        yield return new WaitForSeconds(0.80f);
        Destroy(gameObject);
    }

}