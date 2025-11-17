using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PortalToDungen : MonoBehaviour {
    public GameObject DungeonUI;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            DungeonUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if(DungeonUI != null) DungeonUI.SetActive(false);
        }
    }

    public void OnClickStageButton1() {
        SceneManager.LoadScene("Dungeon1");
    }
}
