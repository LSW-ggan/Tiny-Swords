using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Troll : Monster {
    // 보스 정보
    private int _exprience = 500;           // 처치 경험치
    private float _moveSpeed = 1.0f;        // 이동 속도
    private float _maxHp = 5000f;           // 최대 체력
    private float _currentHp;               // 현재 체력
    private int _attackPower = 30;          // 공격력
    private int _defense = 20;              // 방어력
    private float _detectionRange = 8.0f;   // 감지 범위
    private float _attackRange = 3.0f;      // 공격 범위
    private float _chargeDuration = 3.0f;   // 차지 후 돌진 공격, 차지 시간

    // 전체 공격 쿨타임 (어떤 공격이 되었든 공격 간에 특정 간격을 둠)
    private float _basicAttackCoolTime = 2.0f;

    // 보스 공격
    public CircleCollider2D AttackCollider;
    public GameObject WarningSquarePrefab;
    public GameObject FallingClubPrefab;

    // 행동 제어 변수
    private GameObject _target = null;      // 타겟
    private bool _isChasing = false;        // 추적 여부
    private int _currentPhase = 1;          // 현재 보스 페이즈
    private bool _isPhaseChanging = false;  // 페이즈 변환 중
    private bool _isAttacking = false;      // 공격중
    private bool _isRecovery = false;       // 회복중
    private bool _isDead = false;           // 사망

    // 사운드
    public AudioClip FirstSwingSoundClip;
    public AudioClip SecondSwingSoundClip;
    public AudioClip FirstFootStepSoundClip;
    public AudioClip SecondFootStepSoundClip;
    public AudioClip TrollRoarSoundClip;

    // 1 페이즈 패턴
    enum Phase1 {
        BasicAttack,
    }
    // 2 페이즈 패턴
    enum Phase2 {
        BasicAttack,
        DashAttack,
    }
    // 3 페이즈 패턴
    enum Phase3 {
        BasicAttack,
        DashAttack,
        ClubRain,
    }

    // 1페이즈 공격 패턴 개별 쿨타임
    private float[] _AttackCoolTimes1 ={
        15f, // 기본 공격
    };

    // 1페이즈 공격 패턴별 마지막 사용 시간
    private float[] _lastAttackTimes1 = {
        0f,
    };

    // 2페이즈 공격 패턴 개별 쿨타임
    private float[] _AttackCoolTimes2 = {
        0f,  // 기본 공격
        15f, // 대시 어택
    };
    // 2페이즈 공격 패턴별 마지막 사용 시간
    private float[] _lastAttackTimes2 = {
        0f,
        5f,
    };

    // 3페이즈 공격 패턴 개별 쿨타임
    private float[] _AttackCoolTimes3 = {
        0f,  // 기본 공격
        15f, // 대시 어택
        30f, // 메테오
    };
    // 3페이즈 공격 패턴별 마지막 사용 시간
    private float[] _lastAttackTimes3 = {
        0f,
        10f,
        15f,
    };

    // 몬스터 머리 위치 & 체력바
    public Transform HeadPos;
    public BossMonsterHpBar HpBarPrefab;
    private BossMonsterHpBar _hpBar = null;

    // 투사체
    public GameObject Projectile;

    // 소환 몬스터
    public GameObject Summoner;
    private int _summonCnt = 5;


    // Getter
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
        if (!CanAct() || _isRecovery || _isDead ) return;
       
        PhaseCheck();
        StateUpdate();
    }

    // attack animation event method
    public void EnableAttackCollider() {
        AttackCollider.enabled = true;
    }

    // attack animation event method
    public void DisableAttackCollider() {
        AttackCollider.enabled = false;
    }

    // 현재 페이즈 확인 (10~7 : 1페이즈 / 6~4 : 2페이즈 / 3~0 : 3페이즈)
    private void PhaseCheck() {
        if (_isPhaseChanging) return;

        float ratio = _currentHp / _maxHp;

        if (ratio < 0.7f && _currentPhase == 1) {
            StartCoroutine(EnterPhase2());
        }
        else if (ratio < 0.4f && _currentPhase == 2) {
            StartCoroutine(EnterPhase3());
        }
    }

    // 페이즈2 진입
    private IEnumerator EnterPhase2() {
        _isPhaseChanging = true;
        _currentPhase = 2;

        SummonGnoll();

        yield return new WaitForSeconds(2.0f);
        _isPhaseChanging = false;
    }

    // 페이즈3 진입
    private IEnumerator EnterPhase3() {
        _isPhaseChanging = true;
        _currentPhase = 3;

        SummonGnoll(); // 소환수 소환

        yield return new WaitForSeconds(2.0f);
        _isPhaseChanging = false;
    }

    // 몬스터 소환
    private void SummonGnoll() {
        for (int i = 0; i < _summonCnt; i++) {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), 0);
            GameObject summoner = Instantiate(Summoner, spawnPos, Quaternion.identity).transform.Find("Gnoll").gameObject;
            summoner.GetComponent<Gnoll>().isSummoner = true;
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

            if (Time.time - LastAttackTime >= _basicAttackCoolTime) {
                DecideAttackPattern();
            }
        }
    }

    // 에디터에서 감지 범위 확인용
    void OnDrawGizmos() // 범위 그리기
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }

    // 플레이어 추적
    protected override void ChasePlayer() {
        animator.SetBool("isRunning", true);

        Vector3 dir = (_target.transform.position - transform.position).normalized;
        MonsterPositionMove(dir * _moveSpeed * Time.deltaTime);

        if (dir.x != 0) {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x);
            transform.localScale = scale;
        }
    }

    // 몬스터 이동
    private void MonsterPositionMove(Vector3 dest) {
        if (_isFrozen || _isDead) return;
        transform.position += dest;
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
        if (CanUsePattern(_lastAttackTimes1, _AttackCoolTimes1, (int)Phase1.BasicAttack)) {
            patternIndex = (int)Phase1.BasicAttack; // 기본 공격
        }
        else {
            patternIndex = (int)Phase1.BasicAttack; // 기본 공격
        }

        // 공격 실행
        switch (patternIndex) {
            case (int)Phase1.BasicAttack:
                yield return BasicAttack();
                _lastAttackTimes1[(int)Phase1.BasicAttack] = Time.time;
                break;
        }
    }

    // 2페이즈 패턴 실행 코루틴
    private IEnumerator AttackPatternPhase2() {
        // 공격 패턴 결정
        int patternIndex;
        if (CanUsePattern(_lastAttackTimes2, _AttackCoolTimes2, (int)Phase2.DashAttack)) {
            patternIndex = (int)Phase2.DashAttack; // 대쉬 공격
        }
        else {
            patternIndex = (int)Phase2.BasicAttack; // 기본 공격
        }

        // 공격 실행
        switch (patternIndex) {
            case (int)Phase2.BasicAttack:
                yield return BasicAttack();
                _lastAttackTimes2[(int)Phase2.BasicAttack] = Time.time;
                break;
            case (int)Phase2.DashAttack:
                yield return DashAttack();
                _lastAttackTimes2[(int)Phase2.DashAttack] = Time.time;
                break;
        }
    }

    // 3페이즈 패턴 실행 코루틴
    private IEnumerator AttackPatternPhase3() {
        // 공격 패턴 결정
        int patternIndex;
        if(CanUsePattern(_lastAttackTimes3, _AttackCoolTimes3, (int)Phase3.ClubRain)) {
            patternIndex = (int)Phase3.ClubRain; // 메테오 공격
        }
        else if (CanUsePattern(_lastAttackTimes3, _AttackCoolTimes3, (int)Phase3.DashAttack)) {
            patternIndex = (int)Phase3.DashAttack; // 대쉬 공격
        }
        else {
            patternIndex = (int)Phase3.BasicAttack; // 기본 공격
        }

        // 공격 실행
        switch (patternIndex) {
            case (int)Phase3.BasicAttack:
                yield return BasicAttack();
                _lastAttackTimes3[(int)Phase3.BasicAttack] = Time.time;
                break;
            case (int)Phase3.DashAttack:
                yield return DashAttack();
                _lastAttackTimes3[(int)Phase3.DashAttack] = Time.time;
                break;
            case (int)Phase3.ClubRain:
                yield return ClubRain();
                _lastAttackTimes3[(int)Phase3.ClubRain] = Time.time;
                break;
        }
    }

    // 단일 공격
    private IEnumerator BasicAttack() {
        _isAttacking = true;

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        _isAttacking = false;
        LastAttackTime = Time.time;
    }

    // 돌진 공격
    private IEnumerator DashAttack() {
        _isAttacking = true;
        Vector3 dashDir = (_target.transform.position - transform.position).normalized;
        GameObject attackSquare = Instantiate(WarningSquarePrefab, gameObject.transform.position, Quaternion.identity);

        attackSquare.transform.eulerAngles = new Vector3(
            dashDir.x,
            dashDir.y,
            Vector3.SignedAngle(Vector3.up, dashDir, Vector3.forward));

        animator.SetBool("Windup", true);

        Vector3 originScale = attackSquare.transform.localScale;
        float chargeStartTime = Time.time;
        while (Time.time - chargeStartTime < _chargeDuration) {
            if (_isDead) yield break;
            attackSquare.transform.localScale = new Vector3(originScale.x, (Time.time - chargeStartTime) / _chargeDuration, originScale.z);

            Vector3 curDir = (_target.transform.position - transform.position).normalized;
            float angle = Vector3.SignedAngle(Vector3.up, curDir, transform.forward);
            dashDir = attackSquare.transform.up;
            attackSquare.transform.rotation = Quaternion.Lerp(attackSquare.transform.rotation, Quaternion.Euler(curDir.x, curDir.y, angle), Time.deltaTime * 7.0f);
            yield return new WaitForSeconds(0.01f);
        }

        animator.SetBool("DashAttack", true);
        AudioManager.Instance.PlayEffectSound(TrollRoarSoundClip);
        Destroy(attackSquare);
        animator.SetBool("Windup", false);

        Vector3 curScale = gameObject.transform.localScale;

        if (dashDir.x > 0) {
            gameObject.transform.localScale = new Vector3(Mathf.Abs(curScale.x), curScale.y, curScale.z);
        }
        else {
            gameObject.transform.localScale = new Vector3(-1 * Mathf.Abs(curScale.x), curScale.y, curScale.z);
        }

        float startTime = Time.time;
        while(Time.time - startTime < 4.0f) {
            if (_isDead) yield break;
            MonsterPositionMove(dashDir * _moveSpeed * 2f * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("DashAttack", false);
        _isRecovery = true;

        yield return new WaitForSeconds(4.0f);

        _isRecovery = false;

        _isAttacking = false;
        LastAttackTime = Time.time;
    }

    // 메테오 공격
    private IEnumerator ClubRain() {
        int count = 15;
        float interval = 0.5f;

        _isAttacking = true;
        animator.SetBool("Windup", true);

        for (int i = 0; i < count; i++) {
            if (_isDead) yield break;
            Vector3 targetPos = PlayerDataManager.Instance.Player.transform.position;

            // FallingClub 생성
            GameObject fallingClub = Instantiate(FallingClubPrefab);
            fallingClub.GetComponent<FallingClub>().Init(targetPos);

            if (i + 1 == count) break;
            else yield return new WaitForSeconds(interval);
        }

        animator.SetBool("Windup", false);
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
        if(_hpBar == null) {
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

    // Troll_Attack animation method
    public void OutputFirstTrollSwingSound() {
        AudioManager.Instance.PlayEffectSound(FirstSwingSoundClip);
    }

    // Troll_Attack animation method
    public void OutputSecondTrollSwingSound() {
        AudioManager.Instance.PlayEffectSound(SecondSwingSoundClip);
    }

    // Troll_Walk animation method
    public  void OutputFirstFootStepSound() {
        AudioManager.Instance.PlayEffectSound(FirstFootStepSoundClip);
    }
    
    // Troll_Walk animation method
    public  void OutputSecondFootStepSound() {
        AudioManager.Instance.PlayEffectSound(SecondFootStepSoundClip);
    }

}
