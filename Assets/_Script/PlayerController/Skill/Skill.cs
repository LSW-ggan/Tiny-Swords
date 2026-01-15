using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skill {
    protected SkillData Data;
    protected int SkillLevel;
    public float LastUsedTime { get; protected set; }

    // 스킬 클래스 생성자
    public Skill(SkillData data) {
        Data = data;
        SkillLevel = SkillManager.Instance.GetLevel(Data);
    }

    // 스킬 사용 가능 여부 체크
    public virtual bool SkillCanUse(GameObject player) {
        if (Time.time < LastUsedTime + Data.CoolTime) {
            StatusEffectManager.Instance.CreateFloatingText(
                    player.GetComponent<PlayerController>().HeadPos.position,
                    "스킬 쿨타임",
                    TextType.Default
            );
            return false;
        }
        else if (PlayerDataManager.Instance.Mp < Data.ManaRequire[SkillManager.Instance.GetLevel(Data)]) {
            StatusEffectManager.Instance.CreateFloatingText(
                    player.GetComponent<PlayerController>().HeadPos.position,
                    "마나 부족",
                    TextType.Default
            );
            return false;
        }
        else return true;
    }

    // 스킬 사용
    public abstract void SkillUse(GameObject player);

    // 시전 플레이어 마나 감소
    public void ManaDecrease() {
        PlayerDataManager.Instance.Mp -= Data.ManaRequire[SkillLevel];
    }
}
