using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearUI : MonoBehaviour {
    public static ClearUI Instance;

    public GameObject panel;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        panel.SetActive(false);
    }

    public void Show() {
        panel.SetActive(true);
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
