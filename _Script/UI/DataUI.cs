using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataUI : MonoBehaviour {
    public TMP_Text GoldText;
    public TMP_Text AttackText;

    void Start() {
        UpdateGoldUI();
        UpdateAttackUI();
        PlayerDataManager.Instance.OnGoldChanged += UpdateGoldUI;
        PlayerDataManager.Instance.OnStatChanged += UpdateAttackUI;
    }

    private void OnDestroy() {
        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.OnHpChanged -= UpdateGoldUI;
    }

    private void UpdateGoldUI() {
        GoldText.text = PlayerDataManager.Instance.Gold.ToString();
    }

    private void UpdateAttackUI() {
        AttackText.text = PlayerDataManager.Instance.AttackPower.ToString();
    }
}
