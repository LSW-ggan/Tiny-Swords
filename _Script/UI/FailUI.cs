using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailUI : MonoBehaviour {
    public static FailUI Instance;

    public GameObject panel;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        panel.SetActive(false);   // ÆÐ³Î¸¸ ¼û±è
    }

    public void Show() {
        panel.SetActive(true);    // Á×¾úÀ» ¶§ º¸¿©ÁÜ
    }

    public void OnClickRetry() {
        Player.Instance.CurrentHp = PlayerDataManager.Instance.MaxHp;
        PlayerDataManager.Instance.Hp = Player.Instance.CurrentHp;
        Player.Instance.Anim.Rebind();
        SceneManager.LoadScene("Dungeon1");
    }

    public void OnClickReturnToTown() {
        Player.Instance.CurrentHp = PlayerDataManager.Instance.MaxHp;
        PlayerDataManager.Instance.Hp = Player.Instance.CurrentHp;
        Player.Instance.Anim.Rebind();
        SceneManager.LoadScene("Town");
    }
}
