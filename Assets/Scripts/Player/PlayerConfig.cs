using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig : MonoBehaviour
{
    // Singletone instance
    public static PlayerConfig Instance { get; private set; } 
    
    [Header("Player parameters")]
    [SerializeField] private int  health = 3;
    [SerializeField] private int  damage = 1;
    [SerializeField] private float  speed = 5;
    [SerializeField] private float  jumpForce = 30;
    [SerializeField] private float  wallSlidingSpeed = 3;
    [SerializeField] private float  timeBetweenAttacks = 0.4f; 
    [SerializeField] private float  attackRange = 0.5f;
    [SerializeField] private float xWallForce = 5;
    [SerializeField] private float yWallForce = 20;
    [SerializeField] private float wallJumpTime = 0.1f;
    
    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (value <= 0)
            {
                StartCoroutine(Player.Instance.Die());
            }
        }
    }
    public int Damage
    {
        get => damage;
        set { damage = value < 0 ? damage = 0 : damage = value; }
    }
    public float Speed
    {
        get => speed;
        set { speed = value < 0 ?  speed = 0 : speed = value; }
    }
    public float JumpForce
    {
        get => jumpForce;
        set { jumpForce = value < 0 ?  jumpForce = 0 : jumpForce = value; }
    }
    public float WallSlidingSpeed
    {
        get => wallSlidingSpeed;
        set { wallSlidingSpeed = value < 0 ?  wallSlidingSpeed = 0 : wallSlidingSpeed = value; }
    }
    public float TimeBetweenAttacks
    {
        get => timeBetweenAttacks;
        set { timeBetweenAttacks = value < 0 ? timeBetweenAttacks = 0 : timeBetweenAttacks = value; }
    }
    public float AttackRange
    {
        get => attackRange;
        set { attackRange = value < 0 ? attackRange = 0 : attackRange = value; }
    }
    public float XWallForce
    {
        get => xWallForce;
        set { xWallForce = value < 0 ? xWallForce = 0 : xWallForce = value; }
    }
    public float YWallForce
    {
        get => yWallForce;
        set { yWallForce = value < 0 ? yWallForce = 0 : yWallForce = value; }
    }
    public float WallJumpTime
    {
        get => wallJumpTime;
        set { wallJumpTime = value < 0 ? wallJumpTime = 0 : wallJumpTime = value; }
    }
    
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }
}
