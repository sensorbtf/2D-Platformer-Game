using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public static Player Instance { get; set; }
    public Rigidbody2D RB2D { get; set; }

    public int Health { get; set; }
    public int Damage { get; set; }
    public float Speed { get; set; }
    public float JumpForce { get; set; }
    public float WallSlidingSpeed { get; set; }
    public float TimeBetweenAtacks { get; set; }
    public float AttackRange { get; set; }
    public bool IsImmune { get; set; }

}
