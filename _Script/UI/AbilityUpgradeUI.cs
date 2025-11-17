using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityUpgradeUI : MonoBehaviour {
    private int _price = 10;
    public TMP_Text AttackStat;
    public TMP_Text SpeedStat;

    private void OnEnable() {
        StatSync();
    }

    public void OnClickAttackUpgradeButton() {
        if(PlayerDataManager.Instance.Gold >= _price) {
            PlayerDataManager.Instance.Gold -= _price;
            PlayerDataManager.Instance.AttackPower++;
            StatSync();
        }
    }

    public void OnClickSpeedUpgradeButton() {
        if (PlayerDataManager.Instance.Gold >= _price) {
            PlayerDataManager.Instance.Gold -= _price;
            PlayerDataManager.Instance.Speed += 0.1f;
            StatSync();
        }
    }

    private void StatSync() {
        AttackStat.SetText($"Attack : {PlayerDataManager.Instance.AttackPower}");
        SpeedStat.SetText($"Speed : {PlayerDataManager.Instance.Speed:F1}");
    }
}
