using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject {
    [Header("기본 정보")]
    public int Id;
    public string SkillName;
    public SkillSet.SkillType type;
    public Sprite SquereIcon;
    public Sprite CircleIcon;
    public float CoolTime;

    [Header("스킬 설명")]
    [TextArea(2, 5)]
    public string Description;

    [Header("선행 스킬")]
    public int PrerequisiteSkillId;
    public int requirePreSkillLevel;

    [Header("이펙트")]
    public GameObject EffectPrefab;

    [Header("레벨별 수치")]
    public int MaxLevel = 5;
    public int[] HealAmount;
    public float[] BuffAmount;
    public int[] ManaRequire;

    [Header("상태 지속 효과")]
    public StatusEffectData[] StatusEffects;

    [Header("범위 공격")]
    public float DamageRange = 4.0f;
    public GameObject DamageEffect;
}
