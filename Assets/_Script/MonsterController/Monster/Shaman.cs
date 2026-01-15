using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class Shaman : Monster {
    enum Phase1 {
        SingleShot,
        TripleShot,
    }

    enum Phase2 {
        HomingShot,
        TripleShot,
        SpreadShot,
    }

    enum Phase3 {
        HomingShot,
        TripleHomingShot,
        SpreadHomingShot,
    }

    // 보스 스텟
    private float _moveSpeed = 0.3f;
    private float _maxHp = 300f;
    private float _currentHp;
    private int _attackPower = 10;
    private int _defense = 10;

    // 감지 및 추적
    private float _detectionRange = 8.0f;
    private bool _isChasing = false;
    private GameObject _target = null;

    // 보스 공격
    private int _currentPhase = 1;
    private bool _isPhaseChanging = false;
    private float _attackRange = 9.0f;
    private bool _isAttacking = false;
    private float _basicAttackCoolTime = 3.0f; // 전체 공격 쿨타임 (어떤 공격이 되었든 공격 간에 특정 간격을 둠)
    private bool _isSpreadShotAttacking = false;
    private bool _isHoming = false;
    public AudioClip ProjectileSoundClip;

    // 1페이즈 공격 패턴 개별 쿨타임
    private float[] _AttackCoolTimes1 ={
        0f, // 단일 공격
        5f, // 3연사
    };
    // 1페이즈 공격 패턴별 마지막 사용 시간
    private float[] _lastAttackTimes1 = {
        0f, // 단일 공격
        0f, // 3연사
    };

    // 2페이즈 공격 패턴 개별 쿨타임
    private float[] _AttackCoolTimes2 = {
        0f, // 유도 단일 공격
        8f, // 3연사
        10f, // 부채꼴
    };
    // 2페이즈 공격 패턴별 마지막 사용 시간
    private float[] _lastAttackTimes2 = {
        0f, // 유도 단일 공격
        0f, // 3연사
        0f, // 부채꼴
    };

    // 3페이즈 공격 패턴 개별 쿨타임
    private float[] _AttackCoolTimes3 = {
        0f, // 유도 단일 공격
        8f, // 유도 3연사
        15f, // 유도 부채꼴
    };
    // 3페이즈 공격 패턴별 마지막 사용 시간
    private float[] _lastAttackTimes3 = {
        0f, // 유도 단일 공격
        0f, // 유도 3연사
        0f, // 유도 부채꼴
    };


    // 몬스터 머리 위치 & 체력바
    public Transform HeadPos;
    public BossMonsterHpBar HpBarPrefab;
    private BossMonsterHpBar _hpBar = null;
    private bool _isDead = false;

    // 투사체
    public GameObject Projectile;
    public GameObject HomingProjectile;

    // 소환 몬스터
    public GameObject Summoner;
    private int _summonCnt = 6;

    private int _exprience = 150;

    protected override int Exprience { get => _exprience; }
    protected override float DetectionRange { get => _detectionRange; }
    protected override float MoveSpeed { get => _moveSpeed; }
    protected override float Hp { get => _currentHp; }
    public override int AttackPower { get => _attackPower; }
    protected override float AttackCoolTime { get => _basicAttackCoolTime; }
    protected override float AttackRange { get => _attackRange; }
    public override int Defense { get => _defense; }

    // 플레이어 위치 정보 & 현재 체력 갱신
    protected override void Start() {
        base.Start();
        _currentHp = _maxHp;
        isBossMonster = true;
        ItemDropTable = new List<(int, float)>() {
            (0, 1.0f), (50, 0.5f), (101, 0.1f), (111, 0.1f), (121, 0.1f), (131, 0.1f),
            (141, 0.1f), (151, 0.1f), (161, 0.1f), (171, 0.1f)
        };
    }

    private void Update() {
        if (_isFrozen || _isDead) return;

        PhaseCheck();
        StateUpdate();
    }

    // 현재 페이즈 확인 (10~7 : 1페이즈 / 6~4 : 2페이즈 / 3~0 : 3페이즈)
    private void PhaseCheck() {
        if (_isPhaseChanging) return;

        float ratio = _currentHp / _maxHp;
        
        if(ratio < 0.7f && _currentPhase == 1) {
            StartCoroutine(EnterPhase2());
        }
        else if(ratio < 0.4f && _currentPhase == 2) {
            StartCoroutine(EnterPhase3());
        }
    }

    // 페이즈2 진입
    private IEnumerator EnterPhase2() {
        _isPhaseChanging = true;
        _currentPhase = 2;

        SummonSpider(); // 거미 소환

        yield return new WaitForSeconds(2.0f);
        _isPhaseChanging = false;
    }

    // 페이즈3 진입
    private IEnumerator EnterPhase3() {
        _isPhaseChanging = true;
        _currentPhase = 3;

        SummonSpider(); // 거미 소환

        yield return new WaitForSeconds(2.0f);
        _isPhaseChanging = false;
    }

    // 거미 소환
    private void SummonSpider() {
        for(int i = 0; i < _summonCnt; i++) {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            GameObject summoner = Instantiate(Summoner, spawnPos, Quaternion.identity).transform.Find("Spider").gameObject;
            summoner.GetComponent<Spider>().isSummoner = true;
        }
    }


    // 보스 상태 갱신
    private void StateUpdate() {
        // 몬스터의 감지 범위 안에 들어오면 보스전 시작
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

        // 페이즈 전환 중이거나 공격 중에는 다른 움직임 차단
        if (_isPhaseChanging) return;

        if (_isAttacking) return;

        // 공격 범위 밖으로 나가면 다시 플레이어 추적
        if (distance > _attackRange) {
            ChasePlayer();
        }
        else {
            // 공격 범위 안에 있다면 추적을 멈추고 페이즈에 따른 공격 실행
            animator.SetBool("isRunning", false);

            if(Time.time - LastAttackTime >= _basicAttackCoolTime) {
                DecideAttackPattern();
            }
        }
    }

    void OnDrawGizmos() // 범위 그리기
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }

    // 플레이어 추적
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

    // 페이즈에 따른 공격 패턴 리스트 선택
    private void DecideAttackPattern() {
        // 페이즈 별로 다른 패턴 구성
        switch (_currentPhase) {
            case 1:
                StartCoroutine(AttackPatternPhase1());
                break;
            case 2:
                StartCoroutine(AttackPatternPhase2());
                break;
            case 3:
                StartCoroutine(AttackPatternPhase3());
                break;
        }
    }

    // 1페이즈 패턴 실행 코루틴
    private IEnumerator AttackPatternPhase1() {
        // 공격 패턴 결정
        int patternIndex;
        if(CanUsePattern(_lastAttackTimes1, _AttackCoolTimes1, (int)Phase1.TripleShot)) {
            patternIndex = (int)Phase1.TripleShot; // 3연사
        }
        else {
            patternIndex = (int)Phase1.SingleShot; // 단일 공격
        }

        // 공격 실행
        switch(patternIndex) {
            case (int)Phase1.SingleShot:
                yield return SingleShotAttack();
                _lastAttackTimes1[(int)Phase1.SingleShot] = Time.time;
                break;
            case (int)Phase1.TripleShot:
                yield return TripleShotAttack();
                _lastAttackTimes1[(int)Phase1.TripleShot] = Time.time;
                break;
        }
    }

    // 2페이즈 패턴 실행 코루틴
    private IEnumerator AttackPatternPhase2() {
        // 공격 패턴 결정
        int patternIndex;
        if (CanUsePattern(_lastAttackTimes2, _AttackCoolTimes2, (int)Phase2.SpreadShot)) {
            patternIndex = (int)Phase2.SpreadShot; // 부채꼴
        }
        else if (CanUsePattern(_lastAttackTimes2, _AttackCoolTimes2, (int)Phase2.TripleShot)) {
            patternIndex = (int)Phase2.TripleShot; // 3연사
        }
        else {
            patternIndex = (int)Phase2.HomingShot; // 유도탄
        }

        // 공격 실행
        switch (patternIndex) {
            case (int)Phase2.HomingShot:
                yield return HomingShotAttack();
                _lastAttackTimes2[(int)Phase2.HomingShot] = Time.time;
                break;
            case (int)Phase2.TripleShot:
                yield return TripleShotAttack();
                _lastAttackTimes2[(int)Phase2.TripleShot] = Time.time;
                break;
            case (int)Phase2.SpreadShot:
                yield return SpreadShotAttack();
                _lastAttackTimes2[(int)Phase2.SpreadShot] = Time.time;
                break;
        }
    }

    // 3페이즈 패턴 실행 코루틴
    private IEnumerator AttackPatternPhase3() {
        // 공격 패턴 결정
        int patternIndex;
        if (CanUsePattern(_lastAttackTimes3, _AttackCoolTimes3, (int)Phase3.SpreadHomingShot)) {
            patternIndex = (int)Phase3.SpreadHomingShot; // 유도 부채꼴
        }
        else if (CanUsePattern(_lastAttackTimes3, _AttackCoolTimes3, (int)Phase3.TripleHomingShot)) {
            patternIndex = (int)Phase3.TripleHomingShot; // 유도탄
        }
        else {
            patternIndex = (int)Phase3.HomingShot; // 유도 3연사
        }

        // 공격 실행
        switch (patternIndex) {
            case (int)Phase3.HomingShot:
                yield return HomingShotAttack();
                _lastAttackTimes3[(int)Phase3.HomingShot] = Time.time;
                break;
            case (int)Phase3.TripleHomingShot:
                yield return TripleHomingShotAttack();
                _lastAttackTimes3[(int)Phase3.TripleHomingShot] = Time.time;
                break;
            case (int)Phase3.SpreadHomingShot:
                yield return SpreadHomingShotAttack();
                _lastAttackTimes3[(int)Phase3.SpreadHomingShot] = Time.time;
                break;
        }
    }

    // 단일 공격
    private IEnumerator SingleShotAttack() {
        _isAttacking = true;

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        _isAttacking = false;
        LastAttackTime = Time.time;
    }

    // 3연사
    private IEnumerator TripleShotAttack() {
        _isAttacking = true;

        for (int i = 0; i < 3; i++) {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
        }

        _isAttacking = false;
        LastAttackTime = Time.time;
    }

    // 유도탄
    private IEnumerator HomingShotAttack() {
        _isAttacking = true;
        _isHoming = true;

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        _isAttacking = false;
        LastAttackTime = Time.time;
    }

    // 부채꼴 모양으로 발사
    private IEnumerator SpreadShotAttack() {
        _isAttacking = true;
        _isSpreadShotAttacking = true;

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        _isAttacking = false;
        LastAttackTime = Time.time;
    }

    // 유도 3연사
    private IEnumerator TripleHomingShotAttack() {
        _isAttacking = true;
        _isHoming = true;

        for (int i = 0; i < 3; i++) {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
        }

        _isAttacking = false;
        LastAttackTime = Time.time;
    }

    // 유도 부채꼴
    private IEnumerator SpreadHomingShotAttack() {
        _isAttacking = true;
        _isSpreadShotAttacking = true;
        _isHoming = true;

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        _isAttacking = false;
        LastAttackTime = Time.time;
    }

    // 각 공격 패턴별 공격 가능 여부 확인용
    private bool CanUsePattern(float[] lastAttacktimes, float[] attackCoolTimes, int attackIndex) {
        return Time.time - lastAttacktimes[attackIndex] >= attackCoolTimes[attackIndex];
    }

    // 보스 피격 처리
    public override void TakeDamage(List<(int, TextType)> damage) {
        if (_isDead) return;

        int totalDamage = 0;
        for (int i = 0; i < damage.Count; i++) {
            totalDamage += damage[i].Item1;
        }
        _currentHp -= totalDamage;
        if (_currentHp < 0) _currentHp = 0;
        if (_hpBar == null) {
            _hpBar = Instantiate(HpBarPrefab).GetComponent<BossMonsterHpBar>();
        }
        _hpBar.UpdateHpBar(_currentHp, _maxHp);
        StatusEffectManager.Instance.CreateFloatingText(HeadPos.position, damage);
        if (_currentHp <= 0) {
            Dead();
        }
    }

    // 보스 사망
    public override void Dead() {
        _isDead = true;
        StopAllCoroutines();

        CameraEffect.Instance.PlayBossDeathEffect(transform, GetComponentInChildren<CinemachineVirtualCamera>());

        Destroy(_hpBar);

        base.Dead();
    }

    // Shaman_Attack animation event method
    public void CreateProjectile() {
        Vector3 dir = (_target.transform.position - transform.position).normalized;
        if (!_isHoming) {
            if (_isSpreadShotAttacking) {
                float[] angles = { -40f, -20f, 0f, 20f, 40f };
                foreach (float angle in angles) {
                    GameObject obj = Instantiate(Projectile, transform.position + Vector3.up, Quaternion.identity); ;
                    Projectile EnergyBall = obj.GetComponent<Projectile>();
                    float radian = angle * Mathf.Deg2Rad;
                    EnergyBall.AttackDir = new Vector3(
                        dir.x * Mathf.Cos(radian) - dir.y * Mathf.Sin(radian),
                        dir.x * Mathf.Sin(radian) + dir.y * Mathf.Cos(radian),
                        0
                    );
                }
                _isSpreadShotAttacking = false;
            }
            else {
                GameObject obj = Instantiate(Projectile, gameObject.transform.position + Vector3.up, Quaternion.identity);
                Projectile EnergyBall = obj.GetComponent<Projectile>();
                EnergyBall.AttackDir = dir;
            }
        }
        else {
            if (_isSpreadShotAttacking) {
                float[] angles = { -40f, -20f, 0f, 20f, 40f };

                foreach (float angle in angles) {
                    GameObject obj = Instantiate(HomingProjectile, transform.position + Vector3.up, Quaternion.identity);
                    Projectile EnergyBall = obj.GetComponent<Projectile>();
                    float radian = angle * Mathf.Deg2Rad;
                    EnergyBall.AttackDir = new Vector3(
                        dir.x * Mathf.Cos(radian) - dir.y * Mathf.Sin(radian),
                        dir.x * Mathf.Sin(radian) + dir.y * Mathf.Cos(radian),
                        0
                    );
                }
                _isSpreadShotAttacking = false;
            }
            else {
                GameObject obj = Instantiate(HomingProjectile, transform.position + Vector3.up, Quaternion.identity);
                Projectile EnergyBall = obj.GetComponent<Projectile>();
                EnergyBall.AttackDir = dir;
            }
            _isHoming = false;
        }
    }

    // Shaman_Attack animation event method
    public void OutputProjectileSound() {
        AudioManager.Instance.PlayEffectSound(ProjectileSoundClip);
    }
}
