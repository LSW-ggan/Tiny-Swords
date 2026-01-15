using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailUI : MonoBehaviour {
    public AudioClip FailAudioClip;
    public TMP_Text PlayTimeText;
    private int _dungeonSceneBuildNumber;
    private int _townSceneBuildNumber;

    public void OnEnable() {
        AudioManager.Instance.PlayEffectSound(FailAudioClip);
    }

    public void Init(float startTime, int DungeonNum, int TownNum) {
        float playTime = Time.time - startTime;
        int hours = (int)(playTime / 3600);
        int minutes = (int)((playTime % 3600) / 60);
        int seconds = (int)(playTime % 60);

        string timeText = $"{hours}:{minutes}:{seconds}";

        _dungeonSceneBuildNumber = DungeonNum;
        _townSceneBuildNumber = TownNum;

        PlayTimeText.SetText(timeText);
    }

    public void OnClickRetry() {
        StartCoroutine(CoRetry());
    }

    public void OnClickReturnToTown() {
        StartCoroutine(CoReturn());
    }

    private IEnumerator CoRetry() {
        yield return CameraEffect.Instance.CoFadeOut();
        PlayerDataManager.Instance.Hp = PlayerDataManager.Instance.MaxHp;
        SceneManager.LoadScene(_dungeonSceneBuildNumber);
    }

    private IEnumerator CoReturn() {
        yield return CameraEffect.Instance.CoFadeOut();
        PlayerDataManager.Instance.Hp = PlayerDataManager.Instance.MaxHp;
        SceneManager.LoadScene(_townSceneBuildNumber);
    }
}
