using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Spider : Monster {
    private int _exprience = 5;

    private float _detectionRange = 12.0f;
    private float _moveSpeed = 3.0f;
    private float _maxHp = 20f;
    private float _currentHp;
    private int _defense = 1;

    private int _attackPower = 2;
    private float _attackCoolTime = 2.5f;
    private float _attackRange = 1.0f;
    private bool _isAttacking = false;

    private bool _isChasing = false;

    private GameObject _target = null;

    public Transform HeadPos;
    public MonsterHpBar HpBar;
    private BoxCollider2D _attackCollider;

    protected override int Exprience { get => _exprience; }
    protected override float DetectionRange { get => _detectionRange; }
    protected override float MoveSpeed { get => _moveSpeed; }
    protected override float Hp { get => _currentHp; }
    public override int AttackPower { get => _attackPower; }
    protected override float AttackCoolTime { get => _attackCoolTime; }
    protected override float AttackRange { get => _attackRange; }
    public override int Defense { get => _defense; }

    protected override void Start() {
        base.Start();
        _attackCollider = transform.Find("AttackCollider").GetComponent<BoxCollider2D>();
        _currentHp = _maxHp;
    }

    private void Update() {
        RangeCheck();
    }

    private void RangeCheck() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _detectionRange);
        foreach (Collider2D col in colliders) {
            if (col.gameObject.CompareTag("Player")) {
                if (!_isChasing) {
                    _isChasing = true;
                    _target = col.gameObject;
                    return;
                }
            }
        }

        if (_target == null) return;

        float distance = Vector3.Distance(transform.position, _target.transform.position);

        if (distance >= _attackRange && !_isAttacking) {
            ChasePlayer();
        }
        else {
            animator.SetBool("isRunning", false);
            if (Time.time - LastAttackTime >= _attackCoolTime) {
                _isAttacking = true;
                Attack();
                LastAttackTime = Time.time;
            }
        }
    }

    void OnDrawGizmos() // 범위 그리기
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }

    protected override void ChasePlayer() {
        animator.SetBool("isRunning", true);

        Vector3 dir = (_target.transform.position - transform.position).normalized;
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
        LastAttackTime = Time.time;
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

    public override void TakeDamage(List<(int, TextType)> damage) {
        int totalDamage = 0;
        for(int i = 0; i < damage.Count; i++) {
            totalDamage += damage[i].Item1;
        }
        _currentHp -= totalDamage;
        if (_currentHp < 0) _currentHp = 0;
        HpBar.UpdateHpBar(_currentHp, _maxHp);
        StatusEffectManager.Instance.CreateFloatingText(HeadPos.position, damage);
        if (_currentHp <= 0) {
            Dead();
        }
    }

    public override void Dead() {
        //animator.SetTrigger("Dead");
        base.Dead();
    }
}
