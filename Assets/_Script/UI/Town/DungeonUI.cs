using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour {
    public GameObject DungeonMap;
    public GameObject AlertUIPrefab;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            DungeonMap.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if(DungeonMap != null) DungeonMap.SetActive(false);
        }
    }

    public void OnClickStageOneButton() {
        AudioManager.Instance.PlayButtonSound();
        StartCoroutine(CoLoadScene((int)Scenes.BuildNumber.Dungeon1));
    }

    public void OnClickStageTwoButton() {
        AudioManager.Instance.PlayButtonSound();
        GlobalUIManager.Instance.CreateAlertUI("Comming Soon...");
    }

    public void OnClickStageThreeButton() {
        AudioManager.Instance.PlayButtonSound();
        StartCoroutine(CoLoadScene((int)Scenes.BuildNumber.Dungeon3));
    }

    private IEnumerator CoLoadScene(int num) {
        yield return CameraEffect.Instance.CoFadeOut();
        SceneManager.LoadScene(num);
    }
}
