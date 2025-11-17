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
            Player player = other.GetComponent<Player>();
            if (player!= null) {
                player.TakeDamege(_monster.AttackPower);
            }
        }
    }
}
