using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

// 플레이어 데미지 계산기
public static class DamageCalculator {
    // 플레이어가 몬스터를 타격
    public static (int, TextType) CalculatePlayerDamage(int baseAttack, int balance, float critical, int targetDefense) {
        TextType isCritical = TextType.MonsterTakeDamage;

        // 최소 데미지 비율 (밸런스)
        float minRate = 0.80f + balance * 0.001f; // 밸런스 1당 +0.1%
        minRate = Mathf.Clamp(minRate, 0.8f, 1.0f);

        float maxRate = 1.15f;

        float rate = Random.Range(minRate, maxRate);
        float damage = baseAttack * rate;

        // 치명타 적용
        float multiplier = 1.5f;
        if (Random.value < critical / 100.0f) {
            isCritical = TextType.MonsterCriticalDamage;
            damage *= multiplier;
        }

        // 방어력 적용
        damage = ApplyDefense(damage, targetDefense);

        return (Mathf.Max(1, Mathf.RoundToInt(damage)), isCritical);
    }

    // 몬스터가 플레이어를 타격
    public static int CalculateMonsterDamage(int baseAttack, int targetDefense) {
        float damage = baseAttack;

        // 몬스터 치명타 적용 (20% 고정)
        float multiplier = 1.5f;
        if (Random.value < 0.2f) {
            damage *= multiplier;
        }

        // 방어력 적용
        damage = ApplyDefense(damage, targetDefense);

        return Mathf.Max(1, Mathf.RoundToInt(damage));
    }

    // 방어력 적용
    private static float ApplyDefense(float damage, int defense) {
        float reductionRate = 100f / (100f + defense);
        return damage * reductionRate;
    }
}

