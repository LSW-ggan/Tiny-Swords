using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IceSpike : MonoBehaviour {
    private SanctuaryIceSpawner owner;
    public StatusEffectData StatusEffect;
    private PlayerController _player;
    private int _attackBase;
    public AudioClip IceSpikeSoundClip;

    public void OnEnable() {
        AudioManager.Instance.PlayEffectSound(IceSpikeSoundClip);
    }

    public void Init(SanctuaryIceSpawner spawner, GameObject player, int damage) {
        owner = spawner;
        _player = player.GetComponent<PlayerController>();
        _attackBase = damage;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Monster monster = other.GetComponent<Monster>();
        if (monster == null || !other.isTrigger) return;

        if (owner.damagedMonsters.Contains(other.gameObject)) return;

        owner.damagedMonsters.Add(other.gameObject);

        List<(int, TextType)> damage = new List<(int, TextType)>();
        for(int i = 0; i < 5; i++) {
            damage.Add(DamageCalculator.CalculatePlayerDamage(_attackBase, _player.Balance, _player.Critical, monster.Defense));
        }
        monster.TakeDamage(damage);
        StatusEffectManager.Instance.ApplyStatusEffect(StatusEffect, other.gameObject, 0);
    }
}