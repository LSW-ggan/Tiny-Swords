using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : Projectile {
    private float _speed = 4.5f;       // 투사체 속도
    private int _attackPower = 10;     // 투사체 피해량

    protected override float speed { get => _speed; }
    protected override void Start() {
        base.Start();
        Destroy(gameObject, 5.0f);
    }

    void Update() {
        transform.Translate(AttackDir * speed * Time.deltaTime);
    }

    // 플레이어 접촉시 데미지
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (player != null) {
                player.TakeDamege(DamageCalculator.CalculateMonsterDamage(_attackPower, player.Defense));
            }
            Destroy(gameObject);
        }
    }
}
