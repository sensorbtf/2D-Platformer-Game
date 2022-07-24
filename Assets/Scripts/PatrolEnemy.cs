using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public static PatrolEnemy Instance { get; private set; }

    [SerializeField]
    private int  health = 2;
    [SerializeField]
    private float  speed = 2;
    [SerializeField]
    private int  damage = 1;
    [SerializeField]
    private float  pushBackForce = 2.2f;
    [SerializeField]
    private float  startWaitTime = 1.5f;

    [SerializeField]
    private Transform[] pointsOfPatrol;

    private float  PatrolWaitTime;
    private int  currentPointIndex;
    private bool  isFacingRight;

    [SerializeField]
    private GameObject Blood;

    // Animator

    private Animator anim;

    // Sound

    [SerializeField]
    private AudioClip gettingDamageSound;
    [SerializeField]
    private AudioClip runningSound;    
    [SerializeField]
    private AudioClip pushBackSound;

    // Properties

    public float Speed
    {
        get { return  speed; }
        set {  speed = value < 0 ?  speed = 0 :  speed = value; }
    }
    public int Damage
    {
        get { return  damage; }
        set {  damage = value < 0 ?  damage = 0 :  damage = value; }
    }
    public float PushBackForce
    {
        get { return  pushBackForce * 1000; }
        set {  pushBackForce = value < 0 ?  pushBackForce = 0 :  pushBackForce = value; }
    }
    public int Health
    {
        get { return  health; }
        set 
        { 
             health = value < 0 ?  health = 0 :  health = value;
            if (value <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }

    private void Start()
    {
        transform.position = pointsOfPatrol[0].position;
        anim = GetComponent<Animator>();
         PatrolWaitTime =  startWaitTime;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void Update()
    {
        if (Health <= 0)
        {
            GetComponent<Collider2D>().enabled = false;
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, pointsOfPatrol[ currentPointIndex].position, Speed * Time.deltaTime);

        if (transform.position == pointsOfPatrol[currentPointIndex].position)
        {
            anim.SetBool("isRunning", false);
            if ( PatrolWaitTime <= 0)
            {
                if ( currentPointIndex + 1 < pointsOfPatrol.Length)
                {
                     currentPointIndex++;
                    FlipEnemy();
                }
                else
                {
                     currentPointIndex = 0;
                    FlipEnemy();
                }
                 PatrolWaitTime =  startWaitTime;
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

    void FlipEnemy()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
         isFacingRight = ! isFacingRight;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
            Player.Instance.TakeDamage(Damage);
            Player.Instance.PushBack(PushBackForce);

            SoundManager.Instance.PlayEnemyEffects(pushBackSound);
        }
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        anim.SetTrigger("GettingDamage");
        Instantiate(Blood, transform.position, Quaternion.identity);

        SoundManager.Instance.PlayEnemyEffects(gettingDamageSound);
    }
    IEnumerator Die()
    {
        anim.SetTrigger("Dying");
        yield return new WaitForSeconds(0.80f);
        Destroy(gameObject);
    }
}
