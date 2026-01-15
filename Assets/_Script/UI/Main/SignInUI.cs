using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Text;

public class SignInUI : MonoBehaviour {
    public GameObject SignUpPanel;
    public GameObject NotificationUI;
    private ConfirmUI _notification;

    public TMP_InputField InputIdField;
    public TMP_InputField InputPasswordField;

    private void Start() {
        _notification = NotificationUI.GetComponent<ConfirmUI>();
    }

    public void OnClickSignInButton() {
        AudioManager.Instance.PlayButtonSound();
        SignInData data = new SignInData { email = InputIdField.text, password = InputPasswordField.text };
        string jsonString = JsonUtility.ToJson(data);

        string url = "http://127.0.0.1:8000/api/auth/login";
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        StartCoroutine(Api.Post(request, jsonString, (code, json) => {
            switch(code) {
                case 200:
                    SignInResponse response = JsonUtility.FromJson<SignInResponse>(json);

                    AuthManager.AccessToken = response.accessToken;
                    SceneManager.LoadScene((int)Scenes.BuildNumber.Loading);
                    break;
                case 400:
                case 401:
                    NotificationUI.SetActive(true);
                    _notification.SetText("올바르지 않은 아이디/패스워드\n입니다.");
                    break;
                default:
                    NotificationUI.SetActive(true);
                    _notification.SetText("서버 에러");
                    break;
            }
        }));
    }

    public void OnClickSignUpButton() {
        AudioManager.Instance.PlayButtonSound();
        SignUpPanel.SetActive(true);
    }
}
