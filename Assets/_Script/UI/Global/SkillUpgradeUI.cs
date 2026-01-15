using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkillUpgradeUI : MonoBehaviour {
    public SkillSet PlayerSkillSet;
    public SkillUpgradeSlotUI[] Slots;
    public TMP_Text SkillPointText;

    public GameObject PlayerSkillEquipUI;
    public TMP_Text SkillInfoName;
    public Image SkillInfoIcon;
    public TMP_Text SkillInfoLevel;
    public TMP_Text SkillInfoDescription;

    public event Action OnChangedSelectedSkill;

    private SkillData _selectedSkill;
    public SkillData SelectedSkill {
        get => _selectedSkill;
        set {
            _selectedSkill = value;
            OnChangedSelectedSkill?.Invoke();
        }
    }

    private void Awake() {
        OnChangedSelectedSkill += RefreshSkillInfoUI;
    }

    private void OnDestroy() {
        OnChangedSelectedSkill -= RefreshSkillInfoUI;
    }

    private void OnEnable() {
        _selectedSkill = PlayerSkillSet.Skills[0];
        RefreshSkillUpgradeUI();
        RefreshSkillInfoUI();
    }

    public void RefreshSkillUpgradeUI() {
        for (int i = 0; i < PlayerSkillSet.Skills.Length; i++) {
            SkillData skillData = PlayerSkillSet.Skills[i];
            if (skillData == null) continue;
            Slots[i].SetSkillInfo(skillData);
        }
        SkillPointText.SetText($"{ PlayerDataManager.Instance.SkillPoint} Point");
    }

    public void RefreshSkillInfoUI() {
        SkillInfoIcon.sprite = _selectedSkill.SquereIcon;
        SkillInfoName.SetText(_selectedSkill.SkillName);
        SkillInfoLevel.SetText($"Lv. {SkillManager.Instance.GetLevel(_selectedSkill)}");
        SkillInfoDescription.SetText(_selectedSkill.Description);
    }

    public void OnClickSkillEquipButton() {
        AudioManager.Instance.PlayButtonSound();
        if (SkillManager.Instance.GetLevel(_selectedSkill) > 0) {
            PlayerSkillEquipUI.SetActive(true);
            PlayerSkillEquipUI.GetComponent<SkillEquipUI>().Open(_selectedSkill);
        }
    }

    public void OnClickUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        int currentLevel = SkillManager.Instance.GetLevel(_selectedSkill);
        if (PlayerDataManager.Instance.SkillPoint == 0 || !IsUnlockedSkill()) {
            return;
        }
        if (currentLevel != _selectedSkill.MaxLevel) {
            SkillManager.Instance.SetLevel(_selectedSkill, currentLevel + 1);
            PlayerDataManager.Instance.SkillPoint--;
        }
        RefreshSkillUpgradeUI();
        RefreshSkillInfoUI();
    }

    public bool IsUnlockedSkill() {
        if (_selectedSkill.PrerequisiteSkillId != -1) {
            if (SkillManager.Instance.GetLevel(_selectedSkill.PrerequisiteSkillId) < _selectedSkill.requirePreSkillLevel) {
                return false;
            }
        }
        return true;
    }

    public void OnClickExitButton() {
        AudioManager.Instance.PlayButtonSound();
        gameObject.SetActive(false);
    }
}
