using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAura : IStatusEffect {
    private GameObject _auraEffect; // 버프 이펙트
    private int _bonusValue;        // 버프 상승 값 (버프의 상승 스탯 종류에 따라 추가)

    // 버프 적용
    public void Apply(GameObject target, StatusEffectData data, float coefficient) {
        // 기존 스탯의 %만큼 보너스 스탯 계산(절대값으로 상승 시킨다면 파라미터 추가 수정 필요)
        int _originalValue = PlayerDataManager.Instance.Attack;
        _bonusValue = (int)(_originalValue * coefficient - _originalValue);
        // 보너스 스탯 적용
        PlayerDataManager.Instance.BonusAttack += _bonusValue;

        // 상태 효과 이펙트가 존재한다면, 타겟에 적용
        if (data.EffectPrefab != null) {
            _auraEffect = GameObject.Instantiate(data.EffectPrefab, target.transform.position, Quaternion.identity, target.transform);
            _auraEffect.GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder + 3;
        }
    }
    // 버프 종료
    public void Remove(GameObject target, StatusEffectData data) {
        // 기존 스탯으로 복구
        PlayerDataManager.Instance.BonusAttack -= _bonusValue;

        if (_auraEffect != null) {
            GameObject.Destroy(_auraEffect);
        }
    }
    // 버프 이펙트 재적용
    public void ReapplyEffect(GameObject target, StatusEffectData data) {
        if (data.EffectPrefab != null) {
            _auraEffect = GameObject.Instantiate(data.EffectPrefab, target.transform.position, Quaternion.identity, target.transform);
        }
    }
}
