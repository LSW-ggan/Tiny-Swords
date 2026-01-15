using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType {
    FireAura,
    Freeze,
    // Slow,
    // Stun,
}

[CreateAssetMenu(menuName = "StatusEffect/StatusEffectData")]
public class StatusEffectData : ScriptableObject {
    [Header("버프 기본 정보")]
    public StatusEffectType Type;

    [Header("상태 아이콘")]
    public Sprite Icon;

    [Tooltip("지속 시간 (Seconds)")]
    public float Duration = 5f;

    [Tooltip("이펙트")]
    public GameObject EffectPrefab;
}
