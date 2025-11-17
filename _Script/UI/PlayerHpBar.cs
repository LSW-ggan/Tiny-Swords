using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHpBar : MonoBehaviour {
    public Image Hp;
    public TMP_Text Num;

    private void Start() {
        UpdateHpBar();
        PlayerDataManager.Instance.OnHpChanged += UpdateHpBar;
        Num.text = $"{(int)PlayerDataManager.Instance.MaxHp} / {(int)PlayerDataManager.Instance.MaxHp}";
    }

    private void OnDestroy() {
        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.OnHpChanged -= UpdateHpBar;
    }

    public void UpdateHpBar() {
        float currentHp = PlayerDataManager.Instance.Hp;
        float maxHp = PlayerDataManager.Instance.MaxHp;
        float ratio = currentHp / maxHp;
        Hp.fillAmount = ratio;
        Num.text = $"{(int)(maxHp * ratio)} / {(int)maxHp}";
    }
}
