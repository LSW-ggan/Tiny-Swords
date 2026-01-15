using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityUpgradeUI : MonoBehaviour {
    public TMP_Text AttackStat;
    public TMP_Text AttackLv;
    public int AttackIncrease = 2;

    public TMP_Text DefenseStat;
    public TMP_Text DefenseLv;
    public int DefenseIncrease = 1;

    public TMP_Text CriticalStat;
    public TMP_Text CriticalLv;
    public float CriticalIncrease = 0.2f;

    public TMP_Text BalanceStat;
    public TMP_Text BalanceLv;
    public int BalanceIncrease = 1;

    public TMP_Text SpeedStat;
    public TMP_Text SpeedLv;
    public float SpeedIncrease = 0.05f;

    public TMP_Text StatPointText;

    private void OnEnable() {
        RefreshStatUI();
    }

    public void OnClickAttackOnePointUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        AttackUpgrade(1);
    }

    public void OnClickAttackTenPointUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        AttackUpgrade(10);
    }

    private void AttackUpgrade(int point) {
        if (PlayerDataManager.Instance.StatPoint >= point) {
            PlayerDataManager.Instance.StatPoint -= point;
            PlayerDataManager.Instance.BaseAttack += (point * AttackIncrease);
            PlayerDataManager.Instance.AttackLevel += point;
            RefreshStatUI();
        }
    }
    public void OnClickDefenseOnePointUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        DefenseUpgrade(1);
    }

    public void OnClickDefenseTenPointUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        DefenseUpgrade(10);
    }

    private void DefenseUpgrade(int point) {
        if (PlayerDataManager.Instance.StatPoint >= point) {
            PlayerDataManager.Instance.StatPoint -= point;
            PlayerDataManager.Instance.BaseDefense += (DefenseIncrease * point);
            PlayerDataManager.Instance.DefenseLevel += point;
            RefreshStatUI();
        }
    }

    public void OnClickCriticalOnePointUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        CriticalUpgrade(1);
    }

    public void OnClickCriticalTenPointUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        CriticalUpgrade(10);
    }

    private void CriticalUpgrade(int point) {
        if (PlayerDataManager.Instance.StatPoint >= point) {
            PlayerDataManager.Instance.StatPoint -= point;
            PlayerDataManager.Instance.BaseCritical += (CriticalIncrease * point);
            PlayerDataManager.Instance.CriticalLevel += point;
            RefreshStatUI();
        }
    }

    public void OnClickBalanceOnePointUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        BalanceUpgrade(1);
    }

    public void OnClickBalanceTenPointUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        BalanceUpgrade(10);
    }

    private void BalanceUpgrade(int point) {
        if (PlayerDataManager.Instance.StatPoint >= point) {
            PlayerDataManager.Instance.StatPoint -= point;
            PlayerDataManager.Instance.BaseBalance += (BalanceIncrease * point);
            PlayerDataManager.Instance.BalanceLevel += point;
            RefreshStatUI();
        }
    }

    public void OnClickSpeedOnePointUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        SpeedUpgrade(1);
    }

    public void OnClickSpeedTenPointUpgradeButton() {
        AudioManager.Instance.PlayButtonSound();
        SpeedUpgrade(10);
    }

    private void SpeedUpgrade(int point) {
        if (PlayerDataManager.Instance.StatPoint >= point) {
            PlayerDataManager.Instance.StatPoint -= point;
            PlayerDataManager.Instance.BaseSpeed += (SpeedIncrease * point);
            PlayerDataManager.Instance.SpeedLevel += point;
            RefreshStatUI();
        }
    }

    private void RefreshStatUI() {
        AttackStat.SetText($"{PlayerDataManager.Instance.Attack}");
        AttackLv.SetText($"{PlayerDataManager.Instance.AttackLevel} Lv");

        DefenseStat.SetText($"{PlayerDataManager.Instance.Defense}");
        DefenseLv.SetText($"{PlayerDataManager.Instance.DefenseLevel} Lv");

        CriticalStat.SetText($"{PlayerDataManager.Instance.Critical:F1}");
        CriticalLv.SetText($"{PlayerDataManager.Instance.CriticalLevel} Lv");

        BalanceStat.SetText($"{PlayerDataManager.Instance.Balance}");
        BalanceLv.SetText($"{PlayerDataManager.Instance.BalanceLevel} Lv");

        SpeedStat.SetText($"{PlayerDataManager.Instance.Speed:F2}");
        SpeedLv.SetText($"{PlayerDataManager.Instance.SpeedLevel} Lv");

        StatPointText.text = $"{PlayerDataManager.Instance.StatPoint} Point";
    }

    public void OnClickExitButton() {
        AudioManager.Instance.PlayButtonSound();
        gameObject.SetActive(false);
    }
}
