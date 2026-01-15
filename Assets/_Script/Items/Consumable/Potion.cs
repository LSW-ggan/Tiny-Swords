using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/Potion")]
public class Potion : ConsumableItem {
    public override void GetItem() {
        InventoryManager.Instance.AddItem(this, Amount);
    }

    public override void Use(InventoryItem item) {
        item.Amount -= 1;
        PlayerDataManager.Instance.Hp = Mathf.Min(PlayerDataManager.Instance.Hp + Hp, PlayerDataManager.Instance.MaxHp);
        PlayerDataManager.Instance.Mp = Mathf.Min(PlayerDataManager.Instance.Mp + Mp, PlayerDataManager.Instance.MaxMp);
        AudioManager.Instance.PlayEffectSound(InventoryManager.Instance.UsePotionSoundClip);
    }
}
