using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : Projectile {
    private float _speed = 4.5f;
    private int _attackPower = 11;
    public GameObject ExplosionEffect;

    protected override float speed { get => _speed; }
    protected override void Start() {
        base.Start();
        Invoke("ExplosionEnergyBall", 2.0f);
    }

    private void ExplosionEnergyBall() {
        Instantiate(ExplosionEffect, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void Update() {
        transform.Translate(AttackDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PlayerAttack")) {
            Vector3 dir = gameObject.transform.position - player.position;
            AttackDir = dir;
        }
        else if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            if (player != null) {
                player.TakeDamege(_attackPower, true);
            }
            Instantiate(ExplosionEffect, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
