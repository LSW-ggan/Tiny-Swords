using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

enum Direction {
    Left,
    Right,
}

public class Player : MonoBehaviour {
    private Direction _playerDirection;
    public Animator Anim;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidBody;
    private CapsuleCollider2D _attackCollider;

    public GameObject DashEffect;
    public GameObject GuardEffect;
    public Transform HeadPos;
    public DamageTextManager DamageText;

    private float _speed;
    public int MaxHp;
    public int CurrentHp;
    public int AttackPower { get; private set; }
    public int _attackPower1 { get; private set; }
    public int _attackPower2 { get; private set; }

    private bool _isDashing = false;
    private float _dashSpeed = 8f;
    private float _dashDuration = 0.2f;
    private float _dashCoolTime = 1.0f;

    private bool _isAttacking = false;
    private int _comboAttackStep = 0;
    private float _comboAttackTime = 0.4f;
    private float _lastAttackTime = 0.0f;

    private bool _isReady = false;

    private Coroutine _comboCoroutine;

    public static Player Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        PlayerDataSync();
        PlayerDataManager.Instance.OnStatChanged += PlayerDataSync;
        CurrentHp = MaxHp;
        Anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _playerDirection = Direction.Right;
        _attackCollider = transform.Find("AttackCollider").GetComponent<CapsuleCollider2D>();
        _isReady = true;
    }

    private void Update() {
        if (_isReady) {
            Attack();
            Guard();
            Move();
            Dash();
        }
    }

    private void PlayerDataSync() {
        _speed = PlayerDataManager.Instance.Speed;
        MaxHp = PlayerDataManager.Instance.MaxHp;
        AttackPower = PlayerDataManager.Instance.AttackPower;
        _attackPower1 = AttackPower;
        _attackPower2 = (int)(_attackPower1 * 1.5f);
    }

    // 공격
    private void Attack() {
        if(_isDashing || _isAttacking || Anim.GetBool("isGuard")) {
            return;
        }
        if(Input.GetKeyDown(KeyCode.S)) {
            if (!_isAttacking && _comboAttackStep == 0) {
                Anim.SetTrigger("Attack1");
            }
        }
    }

    // attack animation event method
    public void StartAttack() {
        if(_comboAttackStep == 1) {
            AttackPower = _attackPower2;
        }
        _isAttacking = true;
    }

    // attack animation event method
    public void EndAttack() {
        _isAttacking = false;
        _lastAttackTime = Time.time;

        _comboAttackStep++;
        if(_comboAttackStep == 1) {
            if (_comboCoroutine != null) StopCoroutine(_comboCoroutine);
            _comboCoroutine = StartCoroutine(CoComboAttack());
        }
        else {
            if (_comboCoroutine != null) {
                StopCoroutine(_comboCoroutine);
                _comboCoroutine = null;
            }
            _lastAttackTime = 0.0f;
            _comboAttackStep = 0;
        }
        AttackPower = _attackPower1;
    }

    // attack animation event method
    public void EnableAttackCollider() {
        _attackCollider.enabled = true;
    }

    // attack animation event method
    public void DisableAttackCollider() {
        _attackCollider.enabled = false;
    }

    // 콤보 어택 코루틴
    private IEnumerator CoComboAttack() {
        bool isComboInput = false;
        while (Time.time - _lastAttackTime <= _comboAttackTime) {
            if (Input.GetKeyDown(KeyCode.S)) {
                isComboInput = true;
                break;
            }
            yield return null;
        }
        if (isComboInput) {
            Anim.SetTrigger("Attack2");
        }
        else {
            _comboAttackStep = 0;
        }
    }

    // 가드
    private void Guard() {
        if (_isDashing || _isAttacking) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            if(!Anim.GetBool("isGuard")) {
                Anim.SetBool("isGuard", true);
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            Anim.SetBool("isGuard", false);
        }
    }

    // 이동
    private void Move() {
        if(_isDashing || _isAttacking || Anim.GetBool("isGuard")) {
            return;
        }
        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical"))) {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 movement = new Vector3(h, v, 0);

            if (h != 0) {
                if(h < 0) {
                    transform.localScale = new Vector3(-1, 1, 1);
                    _playerDirection = Direction.Left;
                }
                else {
                    transform.localScale = new Vector3(1, 1, 1);
                    _playerDirection = Direction.Right;
                }
            }
            transform.Translate(movement * _speed * Time.deltaTime);
            Anim.SetBool("isRunning", true);
        }
        else Anim.SetBool("isRunning", false);
    }

    // 대시
    private void Dash() {
        if (!Anim.GetBool("isRunning") || _isAttacking) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            StartCoroutine(CoDash());
        }
    }

    // 대시 코루틴
    private IEnumerator CoDash() {
        _isDashing = true;

        GameObject effect = Instantiate(DashEffect, transform.position + Vector3.down * 0.15f, Quaternion.identity);

        Vector3 dir;
        if (transform.localScale.x > 0) {
            dir = Vector3.right;
            effect.transform.localScale = new Vector3(1, 1, 1);
        }
        else {
            dir = Vector3.left;
            effect.transform.localScale = new Vector3(-1, 1, 1);
        }

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

        if(!Anim.GetBool("isGuard")) {
            DamageText.CreateDamageText(HeadPos.position, damage.ToString());
            CurrentHp = CurrentHp - damage > 0 ? CurrentHp - damage : 0;
            PlayerDataManager.Instance.Hp = CurrentHp;
            StartCoroutine(knockBack(0.07f));
            if (CurrentHp <= 0) {
                Dead();
            }
        }
        else if (isUnblockable) {
            damage /= 2;
            DamageText.CreateDamageText(HeadPos.position, damage.ToString());
            CurrentHp = CurrentHp - damage > 0 ? CurrentHp - damage : 0;
            PlayerDataManager.Instance.Hp = CurrentHp;
            StartCoroutine(knockBack(0.03f));
            if (CurrentHp <= 0) {
                Dead();
            }
        }
        else {
            DamageText.CreateDamageText(HeadPos.position, "Guard");
            Vector3 effectPos = gameObject.transform.position;
            if (_playerDirection == Direction.Left) {
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
            StartCoroutine(knockBack(0.03f));
        }
    }

    // 넉백 처리 코루틴
    private IEnumerator knockBack(float time) {
        Vector3 dir;
        if(_playerDirection == Direction.Left) {
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
        Anim.SetTrigger("Dead");
        StartCoroutine(ShowFailUIAfterDelay());
    }
    IEnumerator ShowFailUIAfterDelay() {
        yield return new WaitForSeconds(2.0f);
        FailUI.Instance.Show();
    }
}
