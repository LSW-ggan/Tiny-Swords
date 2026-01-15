using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ItemLibrary")]
public class ItemLibrary : ScriptableObject {
    public ItemData[] ItemList;

    public ItemData GetItemData(int itemDataId) {
        return ItemList[itemDataId];
    }
}
