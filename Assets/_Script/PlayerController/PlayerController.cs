using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 플레이어의 진행방향 - 스프라이트 애니매이션 좌우 반전을 위함
enum PlayerDirection {
    Left,
    Right,
}

public class PlayerController : MonoBehaviour {
    // 컴포넌트 변수
    private PlayerDirection _playerDirection;
    private SpriteRenderer _renderer;
    private CapsuleCollider2D _attackCollider;
    private PlayerAudioController _audio;
    private Animator _animator;

    // 플레이어 기본 조작 이펙트
    public GameObject DashEffect;
    public GameObject GuardEffect;

    // 데미지, 알림 텍스트 출력 Transform
    public Transform HeadPos;

    // 플레이어 데이터 (PlayerDataManager와 동기화)
    public int MaxHp;
    public int CurrentHp;
    public int MaxMp;
    public int CurrentMp;
    public int Defense;
    public float Critical;
    public int Balance;

    private float _speed;

    // 공격력
    public int AttackPower { get; private set; }
    public int AttackPower1 { get; private set; }   // 1타 공격
    public int AttackPower2 { get; private set; }   // 2타 공격

    // 대쉬 조작 변수
    private bool _isDashing = false;
    private float _dashSpeed;
    private float _dashDuration;
    private float _dashCoolTime;

    // 공격 조작 변수
    public bool _isAttacking = false;
    private int _comboAttackStep = 0;
    private float _comboAttackTime = 0.4f;
    private float _lastAttackTime = 0.0f;

    // 상태 조작 변수
    private bool _isReady = false;
    private bool _isDead = false;

    private Coroutine _comboCoroutine;
   
    private void Start() {
        // 플레이어 데이터 초기 세팅 - PlayerDataManager의 데이터와 동기화
        PlayerDataManager.Instance.IsDead = false;
        PlayerDataManager.Instance.OnStatChanged += PlayerDataSync;
        PlayerDataManager.Instance.OnHpChanged += PlayerDataSync;
        PlayerDataManager.Instance.OnMpChanged += PlayerDataSync;
        PlayerDataSync();
        CurrentHp = MaxHp;
        CurrentMp = MaxMp;

        // 컴포넌트 변수 초기화
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _playerDirection = PlayerDirection.Right;
        _attackCollider = transform.Find("AttackCollider").GetComponent<CapsuleCollider2D>();
        _isReady = true;
        _audio = GetComponent<PlayerAudioController>();

        // 씬 전환 후 재생성 시, 기존에 가지고 있던 버프 다시 적용
        StatusEffectManager.Instance.PlayerReapplyStatusEffect(gameObject);
    }

    // 조작 Update
    private void Update() {
        if (_isReady && !_isDead) {
            Attack();
            Guard();
            Move();
            Dash();
        }
    }

    // PlayerDataManager 정보 동기화 함수
    private void PlayerDataSync() {
        _speed = PlayerDataManager.Instance.Speed;
        CurrentHp = PlayerDataManager.Instance.Hp;
        MaxHp = PlayerDataManager.Instance.MaxHp;
        CurrentMp = PlayerDataManager.Instance.Mp;
        MaxMp = PlayerDataManager.Instance.MaxMp;
        Defense = PlayerDataManager.Instance.Defense;
        Critical = PlayerDataManager.Instance.Critical;
        Balance = PlayerDataManager.Instance.Balance;  
        AttackPower = PlayerDataManager.Instance.Attack;
        AttackPower1 = AttackPower;
        AttackPower2 = (int)(AttackPower1 * 1.5f);
        

        _dashSpeed = PlayerDataManager.Instance.DashSpeed;
        _dashDuration = PlayerDataManager.Instance.DashDuration;
        _dashCoolTime = PlayerDataManager.Instance.DashCoolTime;
    }

    // 플레이어가 조작을 받을 준비가 끝난 경우 true로 변경
    public void SetInput(bool flag) {
        _isReady = flag;
    }

    // 공격
    private void Attack() {
        // 유닛이 이미 대시, 공격, 가드를 진행하고 있는 동안에는 실행하지 않음
        if(_isDashing || _isAttacking || _animator.GetBool("isGuard")) {
            return;
        }
        if(Input.GetKeyDown(KeyCode.S)) {
            if (!_isAttacking && _comboAttackStep == 0) {
                _animator.SetTrigger("Attack1");
            }
        }
    }

    // attack animation event method
    public void StartAttack() {
        // 현재 공격이 2타 스매시인지 확인
        if(_comboAttackStep == 1) {
            // 2타 스매시 공격력 적용
            AttackPower = AttackPower2;
        }
        _isAttacking = true;
    }

    // attack animation event method
    public void EndAttack() {
        _lastAttackTime = Time.time;

        // 콤보 증가
        _comboAttackStep++;
        // 1타 공격을 진행한 경우
        if(_comboAttackStep == 1) {
            if (_comboCoroutine != null) StopCoroutine(_comboCoroutine);
            // 콤보 코루틴
            _comboCoroutine = StartCoroutine(CoComboAttack());
        }
        else {
            if (_comboCoroutine != null) {
                StopCoroutine(_comboCoroutine);
                _comboCoroutine = null;
            }
            // 마지막 타격 시간 초기화, 콤보 초기화
            _lastAttackTime = 0.0f;
            _comboAttackStep = 0;
        }
        AttackPower = AttackPower1;
        // 공격 종료
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

    // attack refresh animation event method
    public void RefreshAttackStatus() {
        _attackCollider.enabled = false;
        _comboAttackStep = 0;
    }

    // 콤보 어택 코루틴 (1타 -> 2타)
    private IEnumerator CoComboAttack() {
        bool isComboInput = false;
        // _comboAttackTime만큼 키 입력 대기
        while (Time.time - _lastAttackTime <= _comboAttackTime) {
            if (Input.GetKeyDown(KeyCode.S)) {
                isComboInput = true;
                break;
            }
            yield return null;
        }
        // 만약 시간 안에 다시 입력이 들어오면 2타 스매시 실행
        if (isComboInput) {
            _animator.SetTrigger("Attack2");
        }
        // 시간 안에 입력이 들어오지 않았다면 콤보 초기화
        else {
            _comboAttackStep = 0;
        }
    }

    // 가드
    private void Guard() {
        // 다른 행동 중에는 대시 가드 불가능
        if (_isDashing || _isAttacking || _animator.GetBool("isRunning")) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            if(!_animator.GetBool("isGuard")) {
                _animator.SetBool("isGuard", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            _animator.SetBool("isGuard", false);
        }
    }

    // 이동
    private void Move() {
        if(_isDashing || _isAttacking || _animator.GetBool("isGuard")) {
            return;
        }
        // 키보드 입력
        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical"))) {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 movement = new Vector3(h, v, 0);
            // 이동 시, 진행 방향에 따라 스프라이트 좌우 반전 처리
            if(h < 0) {
                transform.localScale = new Vector3(-1, 1, 1);
                _playerDirection = PlayerDirection.Left;
            }
            else if(h > 0) {
                transform.localScale = new Vector3(1, 1, 1);
                _playerDirection = PlayerDirection.Right;
            }

            transform.Translate(movement * _speed * Time.deltaTime);
            // Unit_Walk 애니매이션
            _animator.SetBool("isRunning", true);
        }
        // 키보드 입력 끝
        else _animator.SetBool("isRunning", false);
    }

    // 대시
    private void Dash() {
        // 달리는 중에 사용 가능
        if (!_animator.GetBool("isRunning") || _isAttacking) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            StartCoroutine(CoDash());
        }
    }

    // 대시 코루틴
    private IEnumerator CoDash() {
        _isDashing = true;
        _dashSpeed = _speed * 1.8f;

        GameObject effect = Instantiate(DashEffect, transform.position + Vector3.up * 0.1f, Quaternion.identity);

        // 플레이어 진행 방향을 고려하여 대시 이펙트 출력
        Vector3 dir;
        if (transform.localScale.x > 0) {
            dir = Vector3.right;
            effect.transform.localScale = new Vector3(1, 1, 1);
        }
        else {
            dir = Vector3.left;
            effect.transform.localScale = new Vector3(-1, 1, 1);
        }

        // 플레이어 이동
        float elapsed = 0f;
        while (elapsed < _dashDuration) {
            transform.Translate(dir * _dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isDashing = false;

        yield return new WaitForSeconds(_dashCoolTime);
    }

    // 플레이어 데미지
    public void TakeDamege(int damage, bool isUnblockable = false) {
        if (CurrentHp == 0) return;

        // 가드를 올리고 있지 않은 경우
        if(!_animator.GetBool("isGuard")) {
            // 피격 사운드 출력
            AudioManager.Instance.PlayEffectSound(_audio.PlayerTakeDamageSoundClip);
            // 피격 데미지 출력
            StatusEffectManager.Instance.CreateFloatingText(HeadPos.position, damage.ToString(), TextType.PlayerTakeDamage);
            // 데미지 처리
            CurrentHp = CurrentHp - damage > 0 ? CurrentHp - damage : 0;
            PlayerDataManager.Instance.Hp = CurrentHp;
            // 넉백 처리
            StartCoroutine(knockBack(0.07f));
            // 플레이어 사망 체크
            if (CurrentHp <= 0) {
                Dead();
            }
        }
        // 가드를 올리고 있지만 가드 불가 공격이 들어온 경우
        else if (isUnblockable) {
            // 피격 사운드 출력
            AudioManager.Instance.PlayEffectSound(_audio.PlayerTakeDamageSoundClip);
            // 데미지 경감
            damage /= 2;
            // 데미지 출력
            StatusEffectManager.Instance.CreateFloatingText(HeadPos.position, damage.ToString(), TextType.PlayerTakeDamage);
            // 데미지 처리
            CurrentHp = CurrentHp - damage > 0 ? CurrentHp - damage : 0;
            PlayerDataManager.Instance.Hp = CurrentHp;
            // 넉백 처리 (일반 피격보다 덜 밀려나도록)
            StartCoroutine(knockBack(0.03f));
            // 플레이어 사망 체크
            if (CurrentHp <= 0) {
                Dead();
            }
        }
        // 가드를 이용하여 공격을 막은 경우
        else {
            // 가드 피격 사운드 출력
            AudioManager.Instance.PlayEffectSound(_audio.ShieldGaurdSoundClip);
            // 가드 텍스트 출력
            StatusEffectManager.Instance.CreateFloatingText(HeadPos.position, "Guard", TextType.PlayerTakeDamage);
            // 가드 이펙트 출력
            Vector3 effectPos = gameObject.transform.position;
            if (_playerDirection == PlayerDirection.Left) {
                effectPos.x -= 0.21f;
                effectPos.y -= 0.08f;
            }
            else {
                effectPos.x += 0.21f;
                effectPos.y -= 0.08f;
            }
            GameObject effect = Instantiate(GuardEffect, effectPos, Quaternion.identity);
            effect.GetComponent<SpriteRenderer>().sortingOrder = _renderer.sortingOrder + 1;
            effect.transform.SetParent(gameObject.transform.parent);
            // 넉백 처리 (일반 피격보다 덜 밀려나도록)
            StartCoroutine(knockBack(0.03f));
        }
    }

    // 넉백 처리 코루틴
    private IEnumerator knockBack(float time) {
        // 플레이어의 방향과 반대로 넉백
        Vector3 dir;
        if(_playerDirection == PlayerDirection.Left) {
            dir = Vector3.right;
        }
        else {
            dir = Vector3.left;
        }

        float elapsed = 0f;
        while (elapsed < time) {
            if(elapsed > time / 3.0f) {
                transform.Translate(dir * _dashSpeed * Time.deltaTime);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // 플레이어 사망
    private void Dead() {
        _animator.SetTrigger("Dead");
        _isDead = true;
        PlayerDataManager.Instance.IsDead = true;
    }
}
