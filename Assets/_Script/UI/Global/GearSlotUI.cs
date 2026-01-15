using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GearSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler {
    public Image ItemIcon;
    
    public GearItem.GearItemType Type;

    public void SetItem(InventoryItem item) {
        ItemIcon.sprite = item.Data.Icon;
        ItemIcon.enabled = true;
        ItemIcon.SetNativeSize();
    }

    public void ClearSlot() {
        ItemIcon.sprite = null;
        ItemIcon.enabled = false;
    }

    public void OnDrop(PointerEventData eventData) {
        if (InventorySlotUI.DraggingSlot == null) {
            return;
        }
        ItemIcon.sprite = InventorySlotUI.DraggingSlot.ItemIcon.sprite;
        InventoryManager.Instance.UseItem(InventorySlotUI.DraggingSlot.SlotItem);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.clickCount > 1)
            InventoryManager.Instance.RequestUnequip((int)Type);
    }
}
