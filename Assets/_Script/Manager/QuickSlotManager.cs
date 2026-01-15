using System;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour {
    public static QuickSlotManager Instance;

    // 퀵슬롯에 등록된 InventoryItem 정보 리스트
    public InventoryItem[] Items;

    // 퀵슬롯 개수
    private int _slotCount = 5;
    
    // 슬롯 변화 이벤트
    public event Action OnQuickSlotChanged;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Items = new InventoryItem[_slotCount];
    }

    // 슬롯에 세팅
    public void SetItem(int slotIndex, InventoryItem item) {
        // 이미 다른 퀵슬롯에 들어가 있으면 제거
        if (item.QuickSlotIndex != -1) {
            ClearSlot(item.QuickSlotIndex);
        }

        // 해당 슬롯에 기존 아이템 있으면 해제
        ClearSlot(slotIndex);

        // 아이템 등록
        Items[slotIndex] = item;
        item.QuickSlotIndex = slotIndex;

        // 관련 UI 갱신
        OnQuickSlotChanged?.Invoke();
    }

    // 슬롯 비우기
    public void ClearSlot(int index) {
        if (Items[index] != null) {
            Items[index].QuickSlotIndex = -1;
            Items[index] = null;
        }

        OnQuickSlotChanged?.Invoke();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1) && Items[0] != null) {
            InventoryManager.Instance.UseItem(Items[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && Items[1] != null) {
            InventoryManager.Instance.UseItem(Items[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && Items[2] != null) {
            InventoryManager.Instance.UseItem(Items[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && Items[3] != null) {
            InventoryManager.Instance.UseItem(Items[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && Items[4] != null) {
            InventoryManager.Instance.UseItem(Items[4]);
        }
    }
}
