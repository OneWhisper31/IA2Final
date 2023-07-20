using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int health { get;}

    public abstract void OnTakeDamage (int damage);
    public abstract void OnHealEvent  (int heal);
    public abstract void OnDeadEvent  ();
}
