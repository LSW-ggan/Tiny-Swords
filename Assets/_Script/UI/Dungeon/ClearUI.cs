using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ClearUI : MonoBehaviour {
    public TMP_Text RewardText;
    public TMP_Text PlayTimeText;

    private int _dungeonSceneBuildNumber;
    private int _townSceneBuildNumber;

    public void Init(float startTime, int DungeonNum, int TownNum) {
        int reward = Random.Range(50, 100);
        InventoryManager.Instance.Gold += reward;
        RewardText.SetText($"+{reward.ToString()}");

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
