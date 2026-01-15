using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleAttack : Skill {
    private GameObject _player;
    private Animator _animator;
    private PlayerController _playerController;

    public DoubleAttack(SkillData data) : base(data) {
        LastUsedTime -= data.CoolTime;
    }

    public override void SkillUse(GameObject player) {
        _player = player;
        LastUsedTime = Time.time;

        ManaDecrease();

        _playerController = _player.GetComponent<PlayerController>();
        _animator = _player.GetComponent<Animator>();

        _playerController.StartCoroutine(CoDoubleAttack());
    }

    private IEnumerator CoDoubleAttack() {
        _playerController._isAttacking = true;

        if (_player.transform.localScale.x > 0) {
            Data.EffectPrefab.transform.rotation = Quaternion.Euler(0, 0, 0f);
        }
        else {
            Data.EffectPrefab.transform.rotation = Quaternion.Euler(0, 180, 0f);
        }
        _animator.SetBool("isRunning", false);
        _animator.SetTrigger("DoubleAttack");
        GameObject.Instantiate(Data.EffectPrefab, _player.transform.position, Data.EffectPrefab.transform.rotation);
        yield return new WaitForSeconds(0.32f);
        CameraEffect.Instance.Shake(0.1f);
        yield return FirstAttack();
        yield return new WaitForSeconds(0.3f);
        yield return SecondAttack();
        yield return new WaitForSeconds(0.3f);
        _playerController._isAttacking = false;
    }

    private IEnumerator FirstAttack() {
        Vector2 _attackBox = new Vector2(2.5f, 1.4f);
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(
            _player.transform.position + Vector3.right * _attackBox.x / 2 * _player.transform.localScale.x + Vector3.up * _attackBox.y / 2,
            _attackBox, 0);

        foreach (Collider2D collider in collider2Ds) {
            if (collider.CompareTag("Enemy")) {
                if (!collider.isTrigger) continue;

                GameObject.Instantiate(Data.DamageEffect, collider.gameObject.transform);
                Monster enemy = collider.gameObject.GetComponent<Monster>();
                int attackBase = (int)(_playerController.AttackPower * Data.BuffAmount[SkillLevel]);

                List<(int, TextType)> damage = new List<(int, TextType)>();
                damage.Add(DamageCalculator.CalculatePlayerDamage(attackBase, _playerController.Balance, _playerController.Critical, enemy.Defense));
                enemy.TakeDamage(damage);
            }
        }
        yield return null;
    }

    public IEnumerator SecondAttack() {
        Vector2 _attackBox = new Vector2(2.0f, 1.4f);
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(_player.transform.position + Vector3.up * _attackBox.y / 2, _attackBox, 0);

        foreach (Collider2D collider in collider2Ds) {
            if (collider.CompareTag("Enemy")) {
                if (!collider.isTrigger) continue;

                GameObject.Instantiate(Data.DamageEffect, collider.gameObject.transform);
                Monster enemy = collider.gameObject.GetComponent<Monster>();
                int attackBase = (int)(PlayerDataManager.Instance.Attack * Data.BuffAmount[SkillLevel]);

                List<(int, TextType)> damage = new List<(int, TextType)>();
                for (int i = 0; i < 3; i++) {
                    damage.Add(DamageCalculator.CalculatePlayerDamage(attackBase, _playerController.Balance, _playerController.Critical, enemy.Defense));
                }

                enemy.TakeDamage(damage);
            }
        }
        yield return null;
    }
}
