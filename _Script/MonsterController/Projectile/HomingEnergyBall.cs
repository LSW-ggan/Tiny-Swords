using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingEnergyBall : Projectile {
    private float _speed = 2.0f;
    private int _attackPower = 11;
    private bool _isHoming = true;
    private float spawnTime;
    private float _homingDelay = 1.5f;
    public GameObject ExplosionEffect;

    protected override float speed { get => _speed; }
    protected override void Start() {
        base.Start();
        spawnTime = Time.time;
        Invoke("ExplosionEnergyBall", 5.0f);
    }

    private void ExplosionEnergyBall() {
        Instantiate(ExplosionEffect, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void Update() {
        if (Time.time - spawnTime < _homingDelay) {
            transform.Translate(AttackDir * speed * Time.deltaTime);
            return;
        }

        if (_isHoming) {
            Vector3 targetDir = (player.position - transform.position).normalized;
            AttackDir = Vector3.RotateTowards(
                AttackDir,         
                targetDir,        
                4.0f * Time.deltaTime,
                0f).normalized;
            transform.Translate(AttackDir * speed * Time.deltaTime);
        }
        else {
            transform.Translate(AttackDir * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PlayerAttack")) {
            _isHoming = false;
            Vector3 dir = (gameObject.transform.position - player.position).normalized;
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
