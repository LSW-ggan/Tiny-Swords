using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {
    public Image Hp;
    public TMP_Text HpText;

    public Image Mp;
    public TMP_Text MpText;

    public Image Exp;
    public TMP_Text LevelText;

    private void Start() {
        UpdateHpBar();
        UpdateMpBar();
        UpdateExpBoard();
        UpdateLevelText();
        PlayerDataManager.Instance.OnHpChanged += UpdateHpBar;
        PlayerDataManager.Instance.OnMpChanged += UpdateMpBar;
        PlayerDataManager.Instance.OnExpChanged += UpdateExpBoard;
        PlayerDataManager.Instance.OnLevelChanged += UpdateLevelText;
        PlayerDataManager.Instance.OnLevelChanged += UpdateHpBar;
        PlayerDataManager.Instance.OnLevelChanged += UpdateMpBar;
        HpText.text = $"{(int)PlayerDataManager.Instance.MaxHp}/{(int)PlayerDataManager.Instance.MaxHp}";
        MpText.text = $"{(int)PlayerDataManager.Instance.MaxMp}/{(int)PlayerDataManager.Instance.MaxMp}";
    }

    private void OnDestroy() {
        if (PlayerDataManager.Instance != null) {
            PlayerDataManager.Instance.OnHpChanged -= UpdateHpBar;
            PlayerDataManager.Instance.OnMpChanged -= UpdateMpBar;
            PlayerDataManager.Instance.OnExpChanged -= UpdateExpBoard;
            PlayerDataManager.Instance.OnLevelChanged -= UpdateLevelText;
            PlayerDataManager.Instance.OnLevelChanged -= UpdateHpBar;
            PlayerDataManager.Instance.OnLevelChanged -= UpdateMpBar;
        }
    }

    public void UpdateHpBar() {
        float currentHp = PlayerDataManager.Instance.Hp;
        float maxHp = PlayerDataManager.Instance.MaxHp;
        float ratio = currentHp / maxHp;
        Hp.fillAmount = ratio;
        HpText.text = $"{(int)(maxHp * ratio)}/{(int)maxHp}";
    }

    public void UpdateMpBar() {
        float currentMp = PlayerDataManager.Instance.Mp;
        float maxMp = PlayerDataManager.Instance.MaxMp;
        float ratio = currentMp / maxMp;
        Mp.fillAmount = ratio;
        MpText.text = $"{(int)(maxMp * ratio)}/{(int)maxMp}";
    }

    public void UpdateExpBoard() {
        float currentExp = PlayerDataManager.Instance.Exprience;
        int level = PlayerDataManager.Instance.Level;
        float maxExp = PlayerDataManager.Instance.RequireExprienceTable[level];
        float ratio = currentExp / maxExp;
        Exp.fillAmount = ratio;
    }

    public void UpdateLevelText() {
        LevelText.text = $"Lv. {PlayerDataManager.Instance.Level}";
    }
}
