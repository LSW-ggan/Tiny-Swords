using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    public Vector3 AttackDir = new Vector3(0, 0, 0);
    protected abstract float speed { get; }
    protected Transform player;

    protected virtual void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void DestroyProjectile() {
        Destroy(gameObject);
    }
}
