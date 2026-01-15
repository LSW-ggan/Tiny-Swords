using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : MonoBehaviour {
    public SkillSlotUI[] Slots;

    private void Awake() {
        SkillManager.Instance.OnSkillSlotsChanged += RefreshSkillUI;
    }
    private void OnDestroy() {
        SkillManager.Instance.OnSkillSlotsChanged -= RefreshSkillUI;
    }

    private void Start() {
        RefreshSkillUI();
    }

    public void RefreshSkillUI() {
        for (int i = 0; i < SkillManager.Instance.SkillSlots.Length; i++) {
            SkillData skillData = SkillManager.Instance.SkillSlots[i];
            if (skillData == null) continue;
            Slots[i].SetSkill(skillData);
        }
    }

}
