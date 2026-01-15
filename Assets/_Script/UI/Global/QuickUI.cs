using UnityEngine;

public class QuickUI : MonoBehaviour {
    public QuickSlotUI[] Slots;
    private void Start() {
        RefreshQuickSlots();
        QuickSlotManager.Instance.OnQuickSlotChanged += RefreshQuickSlots;
        InventoryManager.Instance.OnInventoryChanged += RefreshQuickSlots;
    }

    private void OnDestroy() {
        QuickSlotManager.Instance.OnQuickSlotChanged -= RefreshQuickSlots;
        InventoryManager.Instance.OnInventoryChanged -= RefreshQuickSlots;
    }

    public void RefreshQuickSlots() {
        InventoryItem[] items = QuickSlotManager.Instance.Items;
        for (int i = 0; i < items.Length; i++) {
            if (items[i] == null || items[i].Amount == 0) {
                items[i] = null;
                Slots[i].Clear();
                continue;
            }
            Slots[i].SetSlot(items[i]);
        }
    }
}
