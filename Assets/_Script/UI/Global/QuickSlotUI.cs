using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class QuickSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler {
    public int QuickSlotIndex;
    public Image Icon;
    public TMP_Text AmountText;

    // 퀵슬롯 위에 인벤토리 슬롯의 드래그 슬롯이 Drop된다면 퀵슬롯에 등록
    public void OnDrop(PointerEventData eventData) {
        InventorySlotUI draggingSlot = InventorySlotUI.DraggingSlot;

        // 드래그 슬롯이 비어있는 슬롯이 아닌지 확인 후
        if (draggingSlot.SlotItem == null) {
            return;
        }

        // 슬롯에 담긴 아이템이 소비 아이템인지 확인 (소비 아이템을 제외한 나머지는 퀵슬롯에 등록 불가)
        ItemData itemData = draggingSlot.SlotItem.Data;

        if (itemData.Type != ItemData.ItemType.ConsumableItem) {
            return;
        }

        // 퀵슬롯 매니저에도 이 정보를 넘겨주고
        QuickSlotManager.Instance.SetItem(QuickSlotIndex, draggingSlot.SlotItem);
        // 퀵슬롯에서도 자체적으로 인벤토리 슬롯 아이템 정보 저장
        SetSlot(draggingSlot.SlotItem);
    }

    // 슬롯에 아이템 넣기
    public void SetSlot(InventoryItem item) {
        Icon.enabled = true;
        Icon.sprite = item.Data.Icon;
        AmountText.SetText(item.Amount.ToString());
    }

    // 슬롯 비우기
    public void Clear() {
        Icon.enabled = false;
        AmountText.SetText("");
    }

    // 마우스로 퀵슬롯 클릭 시, 슬롯이 비워짐
    public void OnPointerClick(PointerEventData eventData) {
        QuickSlotManager.Instance.ClearSlot(QuickSlotIndex);
        Clear();
    }
}
