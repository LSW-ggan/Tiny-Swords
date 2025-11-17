using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour {
    public static ItemDropManager Instance;
    public GameObject goldPrefab;
    public GameObject potionPrefab;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void DropItem(Vector3 pos) {
        float rand = Random.value; // 0~1

        if (rand < 0.5f) {
            // 50% 확률 골드 드랍
            GameObject obj = Instantiate(goldPrefab, pos, Quaternion.identity);
            obj.GetComponent<Item>().amount = Random.Range(10, 21); 
        }
        else if (rand < 0.7f) {
            // 20% 확률 포션 드랍
            GameObject obj = Instantiate(potionPrefab, pos, Quaternion.identity);
            obj.GetComponent<Item>().amount = 10; // 포션 회복량
        }
    }
}
