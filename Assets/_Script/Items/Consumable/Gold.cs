using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/Gold")]
public class Gold : ConsumableItem {
    public override void GetItem() {
        Use(null);
    }

    public override void Use(InventoryItem item) {
        InventoryManager.Instance.Gold += Amount;
    }
}
