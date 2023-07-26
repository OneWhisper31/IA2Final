using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CircleQuery))]
public abstract class Entity : MonoBehaviour, IDamageable, IGridEntity
{
    public int Health { get=>health; }
    public bool IsDead { get => health <= 0; }
    [SerializeField] int health=100;

    public Vector3 Position { get => transform.position; set => transform.position = value; }

    public event Action<IGridEntity> OnMove;

    public CircleQuery circleQuery;

    protected Action OnUpdate,OnFixedUpdate;

    private void Awake()
    {
        circleQuery = GetComponent<CircleQuery>();
    }

    private void Update()
    {
        OnUpdate?.Invoke();
    }
    private void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }


    public virtual void OnTakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            OnDeadEvent();
    }

    public virtual void OnHealEvent(int heal)
    {
        health += heal;
    }

    public virtual void OnDeadEvent()
    {
        gameObject.SetActive(false);
        transform.position = GameManager.gm.deadPivot.position;
    }
}
