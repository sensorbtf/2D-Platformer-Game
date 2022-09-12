using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public static Player Instance { get; set; }
    public Rigidbody2D RB2D { get; set; }
    
    public bool IsImmune { get; set; }

}
