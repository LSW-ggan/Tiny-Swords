using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnoll : Monster {
    private float _detectionRange = 5.0f;
    private float _moveSpeed = 0.5f;
    private float _maxHp = 50f;
    private float _currentHp;

    private int _attackPower = 10;
    private float _attackCoolTime = 5.0f;
    private float _attackRange = 3.5f;
    private bool _isAttacking = false;

    private bool _isChasing = false;

    public Transform HeadPos;
    public MonsterHpBar HpBar;
    public GameObject Projectile;

    protected override float detectionRange { get => _detectionRange; }
    protected override float moveSpeed { get => _moveSpeed; }
    protected override float hp { get => _currentHp; }
    public override int AttackPower { get => _attackPower; }
    protected override float attackCoolTime { get => _attackCoolTime; }
    protected override float attackRange { get => _attackRange; }

    protected override void Start() {
        base.Start();
        player = Player.Instance.GetComponent<Transform>();
        _currentHp = _maxHp;
    }

    private void Update() {
        RangeCheck();
    }

    private void RangeCheck() {
        float distance = Vector3.Distance(transform.position, player.position);

        if (!_isChasing) {
            if (distance <= detectionRange) {
                _isChasing = true;
            }
            return;
        }

        if (_isAttacking) {
            return;
        }
        else if (distance >= attackRange) {
            ChasePlayer();
        }
        else {
            Vector3 dir = (player.position - transform.position).normalized;

            if (dir.x != 0) {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x);
                transform.localScale = scale;
            }
            animator.SetBool("isWalking", false);
            if (Time.time - lastAttackTime >= _attackCoolTime) {
                Attack();
            }
        }
    }

    protected override void ChasePlayer() {
        animator.SetBool("isWalking", true);

        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * _moveSpeed * Time.deltaTime;

        if (dir.x != 0) {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x);
            transform.localScale = scale;
        }
    }

    private void Attack() {
        animator.SetTrigger("Attack");
    }

    // Gnoll_Throw animation event method
    public void CreateProjectile() {
        GameObject obj = Instantiate(Projectile, gameObject.transform.position, Quaternion.identity);
        Projectile Bone = obj.GetComponent<Projectile>();
        Bone.AttackDir = (player.position - transform.position).normalized;
    }

    // Gnoll_Throw animation event method
    public void StartAttack() {
        _isAttacking = true;
        lastAttackTime = Time.time;
    }

    // Gnoll_Throw animation event method
    public void EndAttack() {
        _isAttacking = false;
    }

    public override void TakeDamage(int damage) {
        _currentHp -= damage;
        animator.SetTrigger("Hit");
        if (_currentHp < 0) _currentHp = 0;
        HpBar.UpdateHpBar(_currentHp, _maxHp);
        DamageText.CreateDamageText(HeadPos.position, damage.ToString());
        if (_currentHp <= 0) {
            Dead();
        }
    }

    public override void Dead() {
        //animator.SetTrigger("Dead");
        base.Dead();
    }
}
