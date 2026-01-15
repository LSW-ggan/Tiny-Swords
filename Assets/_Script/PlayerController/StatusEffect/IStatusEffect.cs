using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffect {
    // 상태 효과 적용 로직
    public void Apply(GameObject target, StatusEffectData data, float coefficient);
    // 상태 효과 삭제 로직
    public void Remove(GameObject target, StatusEffectData data);
    // 씬 전환시 이펙트 재적용 로직
    public void ReapplyEffect(GameObject target, StatusEffectData data);
}
