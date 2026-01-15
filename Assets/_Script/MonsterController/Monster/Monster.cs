using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Monster : MonoBehaviour {
    // 아이템 드랍 테이블
    protected List<(int, float)> ItemDropTable = new List<(int, float)>() { (0, 0.5f), (1, 0.2f), (50, 0.5f) };
    protected abstract float DetectionRange { get; }    // 감지 범위
    protected abstract float MoveSpeed { get; }         // 이동 속도
    protected abstract float Hp { get; }                // 체력
    public abstract int AttackPower { get; }            // 공격력
    public abstract int Defense { get; }                // 방어력

    protected abstract float AttackCoolTime { get; }    // 공격 쿨타임
    protected abstract float AttackRange { get; }       // 공격 범위
    protected abstract int Exprience { get; }           // 처치 경험치

    protected float LastAttackTime = 0f;                // 공격 쿨타임 처리 변수

    // 컴포넌트 변수
    protected Animator animator;

    // 몬스터 사망 이벤트
    public event Action<Monster> OnDead;

    // 몬스터 타입
    public bool isBossMonster = false;
    public bool isSummoner = false;

    // 이펙트
    public GameObject DeadEffect;

    // 사운드
    public AudioClip MonsterDeadSoundClip;
    protected bool _isFrozen = false;

    protected virtual void Start() {
        GameObject obj = PlayerDataManager.Instance.Player;
        animator = GetComponent<Animator>();
    }

    // 플레이어 추격
    protected abstract void ChasePlayer();

    // 몬스터 피격
    public abstract void TakeDamage(List<(int, TextType)> damage);

    // 사망 처리
    public virtual void Dead() {
        AudioManager.Instance.PlayEffectSound(MonsterDeadSoundClip);
        // 보스 몬스터인 경우
        if (isBossMonster) {
            PlayerDataManager.Instance.Exprience += Exprience;
            animator.SetTrigger("Dead");
        }
        else if (!isSummoner) {
            DropItem();
            OnDead?.Invoke(this);
            PlayerDataManager.Instance.Exprience += Exprience;
            Instantiate(DeadEffect, transform.position, Quaternion.identity);
            Destroy(gameObject.transform.parent.gameObject);
        }
        else {
            Instantiate(DeadEffect, transform.position, Quaternion.identity);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    // Boss Monster dead animation method
    public void DropItem() {
        ItemDropManager.Instance.DropItem(transform.position, ItemDropTable);
        OnDead?.Invoke(this);
        Instantiate(DeadEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    // 몬스터 애니메이션 정지
    public void StopAnimation() {
        animator.speed = 0f;
    }

    // 몬스터 애니매이션 정지 해제
    public void StartAnimation() {
        animator.speed = 1f;
    }

    // 상태 효과에 따른 몬스터 행동 제어
    protected virtual bool CanAct() {
        return !_isFrozen;
    }

    // 몬스터 빙결 효과 적용
    public void SetFrozenStatusEffect(bool flag) {
        _isFrozen = flag;
    }

}
