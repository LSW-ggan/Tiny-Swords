using System.Collections.Generic;

// 인벤토리 데이터 DTO
[System.Serializable]
public class InventoryItemBundle {
    public List<InventoryItemData> inventory;
}

[System.Serializable]
public class InventoryItemData {
    public int itemDataId;
    public int slotIndex;
    public int amount;
    public int enhanceLevel;
    public bool isEquipped;
    public int quickSlotIndex = -1;
}