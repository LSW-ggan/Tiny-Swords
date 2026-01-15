using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillEquipUI : MonoBehaviour {
    public Image[] Slots = new Image[4];

    private SkillData _equipSkill;

    public void Open(SkillData skill) {
        _equipSkill = skill;     
        RefreshSkillEquipUI();
    }

    public void RefreshSkillEquipUI() {
        for(int i = 0; i < Slots.Length; i++) {
            SkillData skillData = SkillManager.Instance.SkillSlots[i];
            if (skillData == null) continue;
            Slots[i].sprite = skillData.SquereIcon;
            Slots[i].color = new Vector4(255, 255, 255, 255);
        }
    }

    public void OnClickSlot(int index) {
        AudioManager.Instance.PlayButtonSound();
        SkillManager.Instance.SetSkillSlot(index, _equipSkill);
        StartCoroutine(CloseNextFrame());
    }

    private IEnumerator CloseNextFrame() {
        yield return null;
        gameObject.SetActive(false);
    }
} 
