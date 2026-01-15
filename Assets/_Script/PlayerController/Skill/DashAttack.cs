using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class DashAttack : Skill {
    private float _attackSpeed = 60.0f;
    private float _skillTime = 0.14f;
    private Vector2 _attackBox = new Vector2(8.5f, 2.5f);

    public DashAttack(SkillData data) : base(data) {
        LastUsedTime -= data.CoolTime;
    }

    public override void SkillUse(GameObject player) {
        // 마지막 시전 시간 저장 - 쿨타임용
        LastUsedTime = Time.time;

        // 마나 소모
        ManaDecrease();

        player.GetComponent<PlayerController>().StartCoroutine(CoDashAttack(player));
    }

    // 스킬 시전
    private IEnumerator CoDashAttack(GameObject player) {
        PlayerController playerController = player.GetComponent<PlayerController>();
        Animator animator = player.GetComponent<Animator>();
        playerController._isAttacking = true;
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Attack3");

        // 스킬 시전 방향 결정
        Vector3 dir;
        if (player.transform.localScale.x > 0) {
            dir = Vector3.right;
            Data.EffectPrefab.transform.rotation = Quaternion.Euler(0, 0, 90f);
        }
        else {
            dir = Vector3.left;
            Data.EffectPrefab.transform.rotation = Quaternion.Euler(0, 0, -90f);
        }
        
        // 이펙트 위치 결정
        Vector3 middlePos = player.transform.position + new Vector3(dir.x * _attackBox.x / 2, 0.75f, 0);
        GameObject.Instantiate(Data.EffectPrefab, middlePos, Data.EffectPrefab.transform.rotation);

        // 스킬 타격 범위 체크 후 데미지 처리
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(middlePos, _attackBox, 0);

        foreach (Collider2D collider in collider2Ds) {
            // 적이 있다면 타격
            if (collider.CompareTag("Enemy")) {
                if (!collider.isTrigger) continue;

                // 데미지 이펙트 생성
                if(Data.DamageEffect != null) {
                    GameObject.Instantiate(Data.DamageEffect, collider.gameObject.transform);
                }
                Monster enemy = collider.gameObject.GetComponent<Monster>();
                int attackBase = (int)(PlayerDataManager.Instance.Attack * Data.BuffAmount[SkillLevel]);

                // 데미지 산정 및 타겟의 TakeDamage 클래스로 전달
                List<(int, TextType)> damage = new List<(int, TextType)>();
                for (int i = 0; i < 3; i++) {
                    damage.Add(DamageCalculator.CalculatePlayerDamage(
                        attackBase, playerController.Balance, playerController.Critical, enemy.Defense
                    ));
                }
                enemy.TakeDamage(damage);
            }
        }

        // 화면 이펙트
        CameraEffect.Instance.Shake(0.2f);

        // 대시 방향으로 플레이어 이동
        float elapsed = 0f;
        while (elapsed < _skillTime) {
            player.transform.Translate(dir * _attackSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        playerController._isAttacking = false;
    }
}
