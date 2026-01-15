using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkillUpgradeSlotUI : MonoBehaviour {
    public Image Icon;
    public TMP_Text LevelText;
    private SkillData _data = null;

    public SkillUpgradeUI Manager;

    public void OnClickSkillIcon() {
        AudioManager.Instance.PlayButtonSound();
        Manager.SelectedSkill = _data;
    }

    public bool IsUnlockedSkill() {
        if (_data.PrerequisiteSkillId != -1) {
            if (SkillManager.Instance.GetLevel(_data.PrerequisiteSkillId) < _data.requirePreSkillLevel) {
                return false;
            }
        }
        return true;
    }

    public void SetSkillInfo(SkillData data) {
        _data = data;
        Icon.sprite = _data.SquereIcon;
        int level = SkillManager.Instance.GetLevel(data);
        if(IsUnlockedSkill()) {
            Vector4 color = new Vector4(255, 255, 255, 255);
            GetComponent<Image>().color = color;
            Icon.color = color;
        }
        if (level == data.MaxLevel) {
            LevelText.SetText("M");
        }
        else LevelText.SetText($"Lv. {SkillManager.Instance.GetLevel(data)}");
    }
}
