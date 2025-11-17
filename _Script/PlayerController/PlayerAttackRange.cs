using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRange : MonoBehaviour {
    private Player _player;

    void Start() {
        _player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            Monster enemy = other.GetComponent<Monster>();
            if (enemy != null) {
                enemy.TakeDamage(_player.AttackPower);
            }
        }
    }
}
