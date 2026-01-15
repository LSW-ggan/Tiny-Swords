using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : Projectile {
    private float _speed = 4.5f;
    private int _attackPower = 11;
    private float _duration = 2.0f;
    public GameObject ExplosionEffect;
    public AudioClip ExplosionSoundClip;

    protected override float speed { get => _speed; }
    protected override void Start() {
        base.Start();
        StartCoroutine(ExplosionEnergyBall());
    }

    private IEnumerator ExplosionEnergyBall() {
        yield return new WaitForSeconds(_duration);
        Instantiate(ExplosionEffect, gameObject.transform.position, Quaternion.identity);
        AudioManager.Instance.PlayEffectSound(ExplosionSoundClip);
        Destroy(gameObject);
    }

    void Update() {
        transform.Translate(AttackDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PlayerAttack")) {
            Vector3 dir = gameObject.transform.position - playerTransform.position;
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
