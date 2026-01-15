using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Aura : Skill {
    public Aura(SkillData data) : base(data) {
        LastUsedTime = -data.CoolTime;
    }

    public override void SkillUse(GameObject player) {
        PlayerController playerController = player.GetComponent<PlayerController>();
        player.GetComponent<Animator>().SetBool("isRunning", false);
        playerController._isAttacking = true;
        LastUsedTime = Time.time;

        ManaDecrease();

        playerController.StartCoroutine(Explosion(player));
        
    }

    private IEnumerator Explosion(GameObject player) {
        PlayerController playerController = player.GetComponent<PlayerController>();
        GameObject startEffect = GameObject.Instantiate(Data.EffectPrefab, player.transform.position, Quaternion.identity);
        startEffect.GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder + 2000;
        startEffect.transform.parent = player.transform;

        yield return new WaitForSeconds(0.2f);
        CameraEffect.Instance.Shake(0.1f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, Data.DamageRange);
        foreach (Collider2D col in colliders) {
            if (!col.isTrigger) continue;
            if (col.gameObject.CompareTag("Enemy")) {

                GameObject.Instantiate(Data.DamageEffect, col.gameObject.transform.position, Quaternion.identity);
                Monster enemy = col.gameObject.GetComponent<Monster>();
                int attackBase = (int)(PlayerDataManager.Instance.Attack * Data.BuffAmount[SkillLevel]);
                List<(int, TextType)> damage = new List<(int, TextType)>();
                damage.Add(DamageCalculator.CalculatePlayerDamage(attackBase, playerController.Balance, playerController.Critical, enemy.Defense));
                enemy.TakeDamage(damage);
            }
        }

        foreach (StatusEffectData StatusEffect in Data.StatusEffects) {
            StatusEffectManager.Instance.ApplyStatusEffect(StatusEffect, player, Data.BuffAmount[SkillLevel]);
        }
        playerController._isAttacking = false;
    }
}
