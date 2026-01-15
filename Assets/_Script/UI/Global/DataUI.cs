using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DataUI : MonoBehaviour {
    public TMP_Text GoldText;
    public TMP_Text AttackText;

    void Start() {
        UpdateGoldUI();
        UpdateAttackUI();
        InventoryManager.Instance.OnGoldChanged += UpdateGoldUI;
        PlayerDataManager.Instance.OnStatChanged += UpdateAttackUI;
    }

    private void OnDestroy() {
        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.OnHpChanged -= UpdateGoldUI;
        InventoryManager.Instance.OnGoldChanged -= UpdateGoldUI;
        PlayerDataManager.Instance.OnStatChanged -= UpdateAttackUI;
    }

    private void UpdateGoldUI() {
        GoldText.text = InventoryManager.Instance.Gold.ToString();
    }

    private void UpdateAttackUI() {
        AttackText.text = PlayerDataManager.Instance.Attack.ToString();
    }
}
