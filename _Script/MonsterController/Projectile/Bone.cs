using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : Projectile {
    private float _speed = 4.5f;
    private int _attackPower = 10;

    protected override float speed { get => _speed; }
    protected override void Start() {
        base.Start();
        Invoke("DestroyProjectile", 5.0f);
    }

    void Update() {
        transform.Translate(AttackDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Player player = Player.Instance;
            if (player != null) {
                player.TakeDamege(_attackPower);
            }
            Destroy(gameObject);
        }
    }
}
