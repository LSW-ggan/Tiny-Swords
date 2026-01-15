using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConfirmUI : MonoBehaviour {
    public TMP_Text NotificationText;
    public bool isClicked = false;
    public bool Answer = true;

    private void OnDestroy() {
        isClicked = false;
    }

    public void SetText(string text) {
        NotificationText.SetText(text);
    }

    public void OnClickConfirmButton() {
        AudioManager.Instance.PlayButtonSound();
        isClicked = true;
        Answer = true;
        gameObject.SetActive(false);
    }

    public void OnClickCancelButton() {
        AudioManager.Instance.PlayButtonSound();
        isClicked = true;
        Answer = false;
        gameObject.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            isClicked = true;
            Answer = true;
            gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            isClicked = true;
            Answer = false;
            gameObject.SetActive(false);
        }
    }
}
