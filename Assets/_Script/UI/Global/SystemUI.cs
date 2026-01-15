using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SystemUI : MonoBehaviour {
    public GameObject SettingPanel;
    public TMP_Text ProgressText;
    public Button SaveButton;
    public Button SaveButton2;

    public Slider MasterAudioSlider;
    public Slider BGMAudioSlider;
    public Slider SFXAudioSlider;

    private void OnEnable() {
        MasterAudioSlider.value = AudioManager.Instance.MasterVolume;
        BGMAudioSlider.value = AudioManager.Instance.BGMVolume;
        SFXAudioSlider.value = AudioManager.Instance.SFXVolume;
    }

    public void OnMasterAudioValueChanged() {
        AudioManager.Instance.SetMasterVolume(MasterAudioSlider.value);
    }

    public void OnBGMAudioValueChanged() {
        AudioManager.Instance.SetBGMVolume(BGMAudioSlider.value);
    }

    public void OnSFXAudioValueChanged() {
        AudioManager.Instance.SetSFXVolume(SFXAudioSlider.value);
    }

    public void OnClickSaveButton() {
        AudioManager.Instance.PlayButtonSound();
        SaveButton.interactable = false;
        SaveButton2.interactable = false;
        StartCoroutine(SaveData());
    }

    private IEnumerator SaveData() {
        bool isPlayerDataSaved = false;
        bool isInventoryDataSaved = false;
        bool isSkillDataSaved = false;

        bool isLoading = true;
        ProgressText.SetText("저장 중...");
        PlayerDataManager.Instance.SaveData((bool isSuccess) => {
            if (isSuccess) {
                Debug.Log("플레이 데이터 저장완료");
                isPlayerDataSaved = true;
                isLoading = false;
            }
            else {
                ProgressText.SetText("저장 실패");
                StartCoroutine(EraseProgressTextDelay());
                isPlayerDataSaved = false;
                isLoading = false;
            }
        });
        while (isLoading) yield return null;
        if(!isPlayerDataSaved) yield break;

        isLoading = true;
        InventoryManager.Instance.SaveData((bool isSuccess) => {
            if (isSuccess) {
                isInventoryDataSaved = true;
                isLoading = false;
            }
            else {
                ProgressText.SetText("저장 실패");
                StartCoroutine(EraseProgressTextDelay());
                isInventoryDataSaved = false;
                isLoading = false;
            }
            isLoading = false;
        });
        while (isLoading) yield return null;
        if (!isInventoryDataSaved) yield break;

        isLoading = true;
        SkillManager.Instance.SaveData((bool isSuccess) => {
            if (isSuccess) {
                isSkillDataSaved = true;
                isLoading = false;
            }
            else {
                ProgressText.SetText("저장 실패");
                StartCoroutine(EraseProgressTextDelay());
                isSkillDataSaved = false;
                isLoading = false;
            }
            isLoading = false;
        });
        while (isLoading) yield return null;
        if (!isSkillDataSaved) yield break;
        ProgressText.SetText("저장 완료");
        StartCoroutine(EraseProgressTextDelay());
    }

    private IEnumerator EraseProgressTextDelay() {
        yield return new WaitForSeconds(3.0f);
        ProgressText.SetText("");
        SaveButton.interactable = true;
        SaveButton2.interactable = true;
    }

    public void OnClickSettingButton() {
        AudioManager.Instance.PlayButtonSound();
        SettingPanel.SetActive(true);
    }

    public void OnClickMainButton() {
        AudioManager.Instance.PlayButtonSound();
        OnClickSaveButton();
        SceneManager.LoadScene((int)Scenes.BuildNumber.Main);
    }

    public void OnClickEndGameButton() {
        AudioManager.Instance.PlayButtonSound();
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void OnClickExitButton() {
        AudioManager.Instance.PlayButtonSound();
        SettingPanel.SetActive(false);
    }
}
