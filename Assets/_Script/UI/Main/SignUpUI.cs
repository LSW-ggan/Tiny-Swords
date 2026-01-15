using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SignupUI : MonoBehaviour {
    private bool _isDuplicate = true;
    private bool _isConfirmed = false;

    public TMP_InputField InputIdField;
    public TMP_InputField InputPasswordField;
    public TMP_InputField InputPasswordConfirmField;
    public TMP_Text PasswordConfirmText;
    public GameObject NotificationUI;
    private ConfirmUI _notification;

    private void Start() {
        _notification = NotificationUI.GetComponent<ConfirmUI>();
    }

    public void OnEnable() {
        _isDuplicate = true;
        _isConfirmed = false;
        InputIdField.text = "";
        InputPasswordField.text = "";
        InputPasswordConfirmField.text = "";
    }

    public void OnValueChangedPasswordConfirmFIeld() {
        if(InputPasswordField.text != InputPasswordConfirmField.text){
            PasswordConfirmText.SetText("불일치");
            PasswordConfirmText.color = new Color(255, 0, 0, 255);
            _isConfirmed = false;
        }
        else {
            PasswordConfirmText.SetText("일치");
            PasswordConfirmText.color = new Color(0, 255, 0, 255);
            _isConfirmed = true;
        }
    }

    public void OnClickExitButton() {
        AudioManager.Instance.PlayButtonSound();
        gameObject.SetActive(false);
    }

    public void OnClickEmailCheckButton() {
        AudioManager.Instance.PlayButtonSound();
        EmailCheckData data = new EmailCheckData { email = InputIdField.text };
        string jsonString = JsonUtility.ToJson(data);

        string url = "http://127.0.0.1:8000/api/auth/check-email";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        StartCoroutine(Api.Post(request, jsonString, (code, json) => {
            NotificationUI.SetActive(true);
            switch (code) {
                case 200:
                    _isDuplicate = false;
                    _notification.SetText("사용 가능한 이메일입니다.");
                    break;
                case 400:
                    _notification.SetText("올바른 이메일 형식이 아닙니다.");
                    break;
                case 409:
                    _notification.SetText("존재하는 이메일입니다.");
                    break;
                default:
                    _notification.SetText("서버 에러");
                    break;
            }
        }));
    }

    public void OnClickSignUpButton() {
        AudioManager.Instance.PlayButtonSound();
        if (_isDuplicate) {
            NotificationUI.SetActive(true);
            _notification.SetText("이메일 중복확인을 해주세요.");
            return;
        }
        else if(!_isConfirmed) {
            NotificationUI.SetActive(true);
            _notification.SetText("비밀번호가 불일치합니다.");
            return;
        }
        
        SignUpData data = new SignUpData { email = InputIdField.text, password = InputPasswordField.text };
        string jsonString = JsonUtility.ToJson(data);

        string url = "http://127.0.0.1:8000/api/auth/join";
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        StartCoroutine(Api.Post(request, jsonString, (code, json) => {
            switch (code) {
                case 200:
                    NotificationUI.SetActive(true);
                    _notification.SetText("회원 가입이 완료되었습니다.");
                    gameObject.SetActive(false);
                    break;
                default:
                    NotificationUI.SetActive(true);
                    _notification.SetText("서버 에러");
                    break;
            }
        }));
    }
}
