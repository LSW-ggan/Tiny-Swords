using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Gnoll : Monster {
    // 몬스터 정보
    private float _detectionRange = 5.0f;   // 감지 거리
    private float _moveSpeed = 0.5f;        // 이동 속도
    private float _maxHp = 50f;             // 최대 체력
    private float _currentHp;               // 현재 체력
    private int _defense = 2;               // 방어력
    private int _attackPower = 10;          // 공격력
    private float _attackCoolTime = 5.0f;   // 공격 쿨타임
    private float _attackRange = 5.0f;      // 공격 범위
    private int _exprience = 120;            // 처치 경험치

    // 행동 제어 변수
    private bool _isAttacking = false;
    private bool _isChasing = false;

    // 타겟 오브젝트 변수
    private GameObject _target = null;

    // 참조
    public Transform HeadPos;               // 데미지 텍스트 및 체력바 위치
    public MonsterHpBar HpBar;              // 체력바
    public GameObject Projectile;           // 투사체
    public AudioClip ProjectileSoundClip;

    // Getter
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
        _currentHp = _maxHp;
    }

    private void Update() {
        RangeCheck();
    }

    // 플레이어 접근 체크
    private void RangeCheck() {
        // 원 범위 콜라이더 체크
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

        // 플레이어가 감지되지 않음
        if (_target == null) return;

        // 감지되면 거리를 계산
        float distance = Vector3.Distance(transform.position, _target.transform.position);

        if (_isAttacking) return;
        
        // 거리를 계산 후, 공격 범위 안에 있지 않다면 플레이어 추격
        if (distance >= _attackRange) {
            ChasePlayer();
        }
        // 공격 범위 안에 들어왔다면 이동을 멈추고 공격
        else {
            // 투사체 투척 시점에 투척 방향을 바라볼 수 있도록 스프라이트 좌우 반전
            Vector3 dir = (_target.transform.position - transform.position).normalized;

            if (dir.x != 0) {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x);
                transform.localScale = scale;
            }

            // 공격
            animator.SetBool("isWalking", false);
            if (Time.time - LastAttackTime >= _attackCoolTime) {
                Attack();
            }
        }
    }

    // 에디터에서 감지 범위 확인용
    void OnDrawGizmos() // 범위 그리기
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }

    // 플레이어 추격
    protected override void ChasePlayer() {
        animator.SetBool("isWalking", true);

        // 플레이어 방향을 바라보는 방향벡터 계산
        Vector3 dir = (_target.transform.position - transform.position).normalized;

        // 이동
        transform.position += dir * _moveSpeed * Time.deltaTime;

        // 이동 방향에 맞춰 몬스터 스프라이트 좌우 반전
        if (dir.x != 0) {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x);
            transform.localScale = scale;
        }
    }

    // 공격
    private void Attack() {
        animator.SetTrigger("Attack");
    }

    // Gnoll_Throw animation event method
    public void CreateProjectile() {
        // 투사체 생성
        GameObject obj = Instantiate(Projectile, gameObject.transform.position + Vector3.up * 0.3f, Quaternion.identity);
        Projectile Bone = obj.GetComponent<Projectile>();
        // 타겟 방향으로 이동
        Bone.AttackDir = (_target.transform.position - transform.position).normalized;
    }

    // Gnoll_Throw animation event method
    public void OutputProjectileSound() {
        AudioManager.Instance.PlayEffectSound(ProjectileSoundClip);
    }

    // Gnoll_Throw animation event method
    public void StartAttack() {
        _isAttacking = true;
        LastAttackTime = Time.time;
    }

    // Gnoll_Throw animation event method
    public void EndAttack() {
        _isAttacking = false;
    }

    // 피격 처리 로직
    public override void TakeDamage(List<(int, TextType)> damage) {
        // 다회 타격 스킬 피격 시, 누적 피해 데미지 계산
        int totalDamage = 0;
        for (int i = 0; i < damage.Count; i++) {
            totalDamage += damage[i].Item1;
        }
        // 피격 처리
        _currentHp -= totalDamage;
        animator.SetTrigger("Hit");
        if (_currentHp < 0) _currentHp = 0;

        // Hp Bar 업데이트
        HpBar.UpdateHpBar(_currentHp, _maxHp);

        // 데미지 텍스트 생성
        StatusEffectManager.Instance.CreateFloatingText(HeadPos.position, damage);

        // 사망했는지 확인
        if (_currentHp <= 0) {
            Dead();
        }
    }

    public override void Dead() {
        base.Dead();
    }
}
