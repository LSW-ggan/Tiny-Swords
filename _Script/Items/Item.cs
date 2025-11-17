using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Gold,
    HpPotion
}

public class Item : MonoBehaviour {
    public ItemType _itemType;
    public int amount;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            switch (_itemType) {
                case ItemType.Gold:
                    AddGold();
                    break;
                case ItemType.HpPotion:
                    AddHp();
                    break;
            }
            Destroy(gameObject);
        }

    }

    private void AddGold() {
        PlayerDataManager.Instance.Gold += amount;
    }

    private void AddHp() {
        PlayerDataManager.Instance.Hp =Mathf.Min(PlayerDataManager.Instance.Hp + amount, PlayerDataManager.Instance.MaxHp);
    }
}
