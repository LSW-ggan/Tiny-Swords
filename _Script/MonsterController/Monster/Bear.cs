using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : Monster {
    private float _detectionRange = 4.0f;
    private float _moveSpeed = 1.5f;
    private float _maxHp = 50f;
    private float _currentHp;

    private int _attackPower = 8;
    private float _attackCoolTime = 2.5f;
    private float _attackRange = 0.75f;
    private bool _isAttacking = false;

    private bool _isChasing = false;

    public Transform HeadPos;
    public MonsterHpBar HpBar; 
    private CircleCollider2D _attackCollider;

    protected override float detectionRange { get => _detectionRange; }
    protected override float moveSpeed { get => _moveSpeed; }
    protected override float hp { get => _currentHp; }
    public override int AttackPower { get => _attackPower; }
    protected override float attackCoolTime { get => _attackCoolTime; }
    protected override float attackRange { get => _attackRange; }

    protected override void Start() {
        base.Start();
        player = Player.Instance.GetComponent<Transform>();
        _attackCollider = transform.Find("AttackCollider").GetComponent<CircleCollider2D>();
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

        if (distance >= attackRange && !_isAttacking) {
            ChasePlayer();
        }
        else {
            animator.SetBool("isRunning", false);
            if (Time.time - lastAttackTime >= _attackCoolTime) {
                _isAttacking = true;
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    protected override void ChasePlayer() {
        animator.SetBool("isRunning", true);

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

    // attack animation event method
    public void StartAttack() {
        _isAttacking = true;
        lastAttackTime = Time.time;
    }

    // attack animation event method
    public void EndAttack() {
        _isAttacking = false;
    }

    // attack animation event method
    public void EnableAttackCollider() {
        _attackCollider.enabled = true;
    }

    // attack animation event method
    public void DisableAttackCollider() {
        _attackCollider.enabled = false;
    }

    public override void TakeDamage(int damage) {
        _currentHp -= damage;
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
