using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacters
{
    public int Health { get; set; }
    public int Damage { get; set; }
    public void TakeDamage(int damage);
}
