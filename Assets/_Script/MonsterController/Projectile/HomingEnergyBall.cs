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
    public AudioClip ExplosionSoundClip;
    private float _duration = 5.0f;

    protected override float speed { get => _speed; }
    protected override void Start() {
        base.Start();
        spawnTime = Time.time;
        StartCoroutine(ExplosionEnergyBall());
    }

    private IEnumerator ExplosionEnergyBall() {
        yield return new WaitForSeconds(_duration);
        Instantiate(ExplosionEffect, gameObject.transform.position, Quaternion.identity);
        AudioManager.Instance.PlayEffectSound(ExplosionSoundClip);
        Destroy(gameObject);
    }

    void Update() {
        if (Time.time - spawnTime < _homingDelay) {
            transform.Translate(AttackDir * speed * Time.deltaTime);
            return;
        }

        if (_isHoming) {
            Vector3 targetDir = (playerTransform.position - transform.position).normalized;
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
            Vector3 dir = (gameObject.transform.position - playerTransform.position).normalized;
            AttackDir = dir;
        }
        else if (other.CompareTag("Player")) {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) {
                player.TakeDamege(DamageCalculator.CalculateMonsterDamage(_attackPower, player.Defense), true);
            }
            Instantiate(ExplosionEffect, gameObject.transform.position, Quaternion.identity);
            AudioManager.Instance.PlayEffectSound(ExplosionSoundClip);
            Destroy(gameObject);
        }
    }
}
