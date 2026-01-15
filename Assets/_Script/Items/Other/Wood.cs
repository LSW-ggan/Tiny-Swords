using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Other/Wood")]
public class Wood : OtherItem {
    public override void GetItem() {
        Amount = Random.Range(1, 4);
        InventoryManager.Instance.AddItem(this, Amount);
    }

    public override void Use(InventoryItem item) {
        
    }
}
