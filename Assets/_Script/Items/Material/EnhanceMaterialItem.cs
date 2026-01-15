using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Material/Enhancement")]
public class EnhanceMaterialItem : MaterialItem {
    public override void GetItem() {
        InventoryManager.Instance.AddItem(this, 1000);
    }

    public override void Use(InventoryItem item) {
        return;
    }
}
