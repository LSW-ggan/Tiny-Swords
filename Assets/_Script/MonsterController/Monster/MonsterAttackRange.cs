using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackRange : MonoBehaviour {
    private Monster _monster;

    void Start() {
        _monster = transform.parent.GetComponent<Monster>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (!other.isTrigger) return;
            PlayerController player = PlayerDataManager.Instance.Player.GetComponent<PlayerController>();
            if (player!= null) {
                player.TakeDamege(DamageCalculator.CalculateMonsterDamage(_monster.AttackPower, player.Defense));
            }
        }
    }
}
