using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Monster : MonoBehaviour {
    protected abstract float detectionRange { get; }
    protected abstract float moveSpeed { get; }
    protected abstract float hp { get; }
    public abstract int AttackPower { get; }
    protected abstract float attackCoolTime { get; }
    protected abstract float attackRange { get; }

    protected float lastAttackTime = 0f;

    protected Transform player;
    protected Animator animator;
    protected DamageTextManager DamageText;
    public event Action<Monster> OnDead;

    protected virtual void Start() {
        animator = GetComponent<Animator>();
        DamageText = DamageTextManager.Instance;
    }

    protected abstract void ChasePlayer();

    public abstract void TakeDamage(int damage);

    public virtual void Dead() {
        ItemDropManager.Instance.DropItem(transform.position);
        OnDead?.Invoke(this);
        Destroy(gameObject.transform.parent.gameObject);
    }
}
