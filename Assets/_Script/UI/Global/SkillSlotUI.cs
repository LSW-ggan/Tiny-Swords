using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour {
    public Image Icon;
    public Image CoolTimePanel;
    public TextMeshProUGUI CoolTimeText;

    private SkillData _data;

    public void SetSkill(SkillData data) {
        _data = data;
        Icon.sprite = data.CircleIcon;
    }

    private void Update() {
        if (_data == null) return;

        float remainingTime = SkillManager.Instance.GetCoolTimeRemaining(_data);

        CoolTimePanel.fillAmount = remainingTime / _data.CoolTime;
        CoolTimeText.text = remainingTime > 0 ? Mathf.CeilToInt(remainingTime).ToString() : "";
    }
}

