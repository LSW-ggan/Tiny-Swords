using UnityEngine;

public class ItemPickup : MonoBehaviour {
    public ItemData Item;

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if(Item.Type == ItemData.ItemType.FieldItem || !InventoryManager.Instance.IsFull(Item)) {
                AudioManager.Instance.PlayEffectSound(InventoryManager.Instance.ItemPickUpAudioClip);
                Item.GetItem();
                Destroy(gameObject);
            }
            else {
                StatusEffectManager.Instance.CreateFloatingText(
                    collision.GetComponent<PlayerController>().HeadPos.position,
                    "인벤토리 부족",
                    TextType.Default
                );
            }
        }
    }
}