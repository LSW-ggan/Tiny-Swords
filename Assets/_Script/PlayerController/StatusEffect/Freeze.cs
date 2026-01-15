using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Freeze : IStatusEffect {
    private MaterialPropertyBlock _block;
    private Monster _monsterInstance;

    // 빙결 적용
    public void Apply(GameObject target, StatusEffectData data, float coefficient) {
        _monsterInstance = target.GetComponent<Monster>();

        if (target == null) return;

        var _targetRenderer = target.GetComponentsInChildren<SpriteRenderer>();

        if (_block == null)
            _block = new MaterialPropertyBlock();

        foreach (var randerer in _targetRenderer) {
            // 빙결 효과 시, 파란 계열로 타겟의 스프라이트 색상 표시
            randerer.GetPropertyBlock(_block);
            _block.SetColor("_Color", new Color(130/255f, 130/255f, 1.0f, 1.0f));
            randerer.SetPropertyBlock(_block);
        }
        _monsterInstance.SetFrozenStatusEffect(true);
        _monsterInstance.StopAnimation();
    }

    // 효과 삭제
    public void Remove(GameObject target, StatusEffectData data) {
        if (_monsterInstance == null) return;

        var renderers = target.GetComponentsInChildren<SpriteRenderer>();

        if (_block == null)
            _block = new MaterialPropertyBlock();

        foreach (var randerer in renderers) {
            randerer.GetPropertyBlock(_block);
            _block.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
            randerer.SetPropertyBlock(_block);
        }

        _monsterInstance.SetFrozenStatusEffect(false);
        _monsterInstance.StartAnimation();
    }
    // 별도의 이펙트 없으므로 생략
    public void ReapplyEffect(GameObject target, StatusEffectData data) { }
}
