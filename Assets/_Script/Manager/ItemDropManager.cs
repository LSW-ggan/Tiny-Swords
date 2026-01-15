using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour {
    public static ItemDropManager Instance;
    public ItemLibrary Item;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void DropItem(Vector3 pos, List<(int, float)> itemDropTable) {
        float rand;
        
        for (int i = 0; i < itemDropTable.Count; i++) {
            rand = Random.value; // 0 ~ 1
            ItemData dropItem = Item.ItemList[itemDropTable[i].Item1];
            float percentage = itemDropTable[i].Item2;
            if(rand <= percentage) {
                Instantiate(dropItem.ItemPrefab, pos, Quaternion.identity);
                pos.x += 0.7f;
            }
        }
    }
}
