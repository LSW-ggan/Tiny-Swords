using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttackRange : MonoBehaviour {
    public PlayerController Player;
    public GameObject TakeDamageEffect;

    private void OnTriggerEnter2D(Collider2D other) {
        // 타격 범위 몬스터 타격
        if (other.CompareTag("Enemy")) {
            if (!other.isTrigger) return;
            // 타격 위치에 몬스터 피격 이펙트 생성
            Vector3 dirVector = (other.gameObject.transform.position - Player.transform.position).normalized;
            float distance = Vector3.Distance(other.gameObject.transform.position, Player.transform.position);
            Instantiate(TakeDamageEffect, Player.transform.position + Vector3.up + dirVector * 1.0f, Quaternion.identity);

            Monster enemy = other.GetComponent<Monster>();
            if (enemy != null) {
                // 데미지 리스트 생성
                List<(int, TextType)> damage = new List<(int, TextType)>();
                // 플레이어와 몬스터의 스탯을 고려하여 데미지 산정
                damage.Add(DamageCalculator.CalculatePlayerDamage(Player.AttackPower, Player.Balance, Player.Critical, enemy.Defense));
                // 타격 몬스터의 TakeDamage 메소드를 호출하여 전달
                enemy.TakeDamage(damage); 
            }
        }
    }
}
