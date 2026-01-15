using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUI : MonoBehaviour {
    public GearSlotUI[] GearSlots;
    public InventorySlotUI[] ItemSlots;
    public TMP_Text GoldText;

    // 인벤토리 슬롯 초기화
    private void Awake() {
        for (int i = 0; i < ItemSlots.Length; i++) {
            ItemSlots[i].SlotIndex = i;
        }
    }

    // 인벤토리 데이터 변화 시, UI 갱신 이벤트 등록
    private void Start() {
        RefreshInventory();
        RefreshGear();

        InventoryManager.Instance.OnInventoryChanged += RefreshInventory;
        InventoryManager.Instance.OnInventoryChanged += RefreshGear;
        QuickSlotManager.Instance.OnQuickSlotChanged += RefreshInventory;
    }

    private void OnDestroy() {
        InventoryManager.Instance.OnInventoryChanged -= RefreshInventory;
        InventoryManager.Instance.OnInventoryChanged -= RefreshGear;
        QuickSlotManager.Instance.OnQuickSlotChanged -= RefreshInventory;
    }

    // 인벤토리 UI 갱신
    public void RefreshInventory() {
        List<InventoryItem> items = InventoryManager.Instance.Items;

        for (int i = 0; i < items.Count; i++) {
            if (items[i] == null) {
                ItemSlots[i].ClearSlot();
            }
            else {
                ItemSlots[i].SetItem(items[i]);
            }
        }

        GoldText.SetText(InventoryManager.Instance.Gold.ToString());
    }

    // 장비칸 UI 갱신
    private void RefreshGear() {
        if(InventoryManager.Instance.GetMode() != InventoryManager.InventoryMode.Normal) {
            return;
        }
        List<InventoryItem> gears = InventoryManager.Instance.EquipGears;
        for(int i = 0; i < gears.Count; i++) {
            if(gears[i] == null) {
                GearSlots[i].ClearSlot();
            }
            else {
                GearSlots[i].SetItem(gears[i]);
            }
        }
    }

    // 인벤토리 닫기
    public void OnClickExitButton() {
        AudioManager.Instance.PlayButtonSound();
        InventoryManager.Instance.SetMode(InventoryManager.InventoryMode.Normal);
        Destroy(gameObject);
    }
}
