using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillSet")]
public class SkillSet : ScriptableObject {
    public enum SkillType {
        Heal,
        Aura,
        DashAttack,
        Sanctuary,
        DoubleAttack,
    }

    public SkillData[] Skills;
}
