using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/Meat")]
public class Meat : ConsumableItem {
    public override void GetItem() {
        Use(null);
    }

    public override void Use(InventoryItem item) {
        PlayerDataManager.Instance.Hp = Mathf.Min(PlayerDataManager.Instance.Hp + Hp, PlayerDataManager.Instance.MaxHp);
    }
}
