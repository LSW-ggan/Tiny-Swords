using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, 
    IBeginDragHandler, 
    IDragHandler, 
    IEndDragHandler, 
    IDropHandler, 
    IPointerClickHandler, 
    IPointerEnterHandler,
    IPointerExitHandler 
    {
    // 현재 드래그 중인 슬롯 인스턴스
    public static InventorySlotUI DraggingSlot;
    // 드래그앤 드롭 전용 단일 슬롯 객체
    public InventoryDragSlotUI DragSlotUI;

    // 장비 장착 및 퀵슬롯 장착 표시 아이콘
    public Image EquipMarkIcon;
    public Image QuickMarkIcon;
    
    // 아이템 아이콘
    public Image ItemIcon;

    // 슬롯 아이템 정보
    public TMP_Text AmountText;
    public InventoryItem SlotItem;

    // 아이템 설명창 프리펩
    public GameObject GearDescriptionUI; 
    public GameObject BasicDescriptionUI;

    // 해당 슬롯의 인덱스 번호
    public int SlotIndex;
    private InventoryItem _targetItem => InventoryManager.Instance.GetItem(SlotIndex);

    private void OnDisable() {
        DraggingSlot = null;
    }

    // 슬롯에 아이템 등록
    public void SetItem(InventoryItem item) {
        SlotItem = item;
        ItemIcon.sprite = SlotItem.Data.Icon;
        ItemIcon.enabled = true;
        ItemIcon.SetNativeSize();
        AmountText.SetText(SlotItem.Amount.ToString());

        // 장착 중이라면 장착 표시 활성화
        if (item.IsEquipped) {
            EquipMarkIcon.enabled = true;
        }
        else {
            EquipMarkIcon.enabled = false;
        }
        // 퀵슬롯 등록 중이라면 퀵 표시 활성화
        if(item.IsQuickEquipped) {
            QuickMarkIcon.enabled = true;
        }
        else {
            QuickMarkIcon.enabled = false;
        }
    }


    // 슬롯 비우기
    public void ClearSlot() {
        SlotItem = null;
        EquipMarkIcon.enabled = false;
        QuickMarkIcon.enabled = false;
        ItemIcon.enabled = false;
        AmountText.SetText("");
    }

    // 드래그 시작 이벤트
    public void OnBeginDrag(PointerEventData eventData) {
        DraggingSlot = this;
        if(DraggingSlot.SlotItem == null) {
            return;
        }
        // 드래그 슬롯 객체에 해당 슬롯의 정보를 넘겨줌
        DragSlotUI.Drag(DraggingSlot.SlotItem.Data.Icon);
    }

    // 드래그 진행 중
    public void OnDrag(PointerEventData eventData) {
        // 드래그 슬롯이 마우스를 따라감
        DragSlotUI.MouseFollow(Input.mousePosition);
    }

    // 드래그 종료 이벤트
    public void OnEndDrag(PointerEventData eventData) {
        DragSlotUI.EndDrag();

        if(eventData.pointerEnter == null) {
            StartCoroutine(InventoryManager.Instance.DropItem(_targetItem, _targetItem.Amount));
        }
        DraggingSlot = null;
    }

    // 다른 슬롯의 아이템이 본 슬롯에 드랍되었을 때
    public void OnDrop(PointerEventData eventData) {
        if (InventorySlotUI.DraggingSlot == null) return;

        // 드래그 아이템의 슬롯과 위치 교환
        InventoryManager.Instance.SlotItemSwap(InventorySlotUI.DraggingSlot.SlotIndex, this.SlotIndex);
    }

    // 슬롯 클릭 인벤트
    public void OnPointerClick(PointerEventData eventData) {
        // 더블클릭 시
        if (eventData.clickCount > 1) {
            switch(InventoryManager.Instance.GetMode()) {
                // 상점 이용 중이라면 아이템 판매 기능 호출
                case InventoryManager.InventoryMode.Trading:
                    if (_targetItem != null) {
                        StartCoroutine(InventoryManager.Instance.SellItem(_targetItem));
                    }
                    break;
                // 일반 상태라면 아이템 사용
                case InventoryManager.InventoryMode.Normal:
                    InventoryManager.Instance.UseItem(SlotItem);
                    break;
                // 강화창 이용 중이라면 강화 타겟 슬롯에 등록
                case InventoryManager.InventoryMode.Enhance:
                    EnhanceUI.Instance.SetTarget(SlotItem);
                    break;
            }
        }
    }

    // 슬롯에 포인터를 올릴 때, 해당 슬롯의 아이템 설명 UI 활성화 및 정보 출력
    public void OnPointerEnter(PointerEventData eventData) {
        if (SlotItem == null) return;

        // 아이템이 장비 아이템이라면
        if (SlotItem.Data is GearItem) {
            GearDescriptionUI.SetActive(true);
            GearDescriptionUI.transform.position = eventData.position;
            GearDescriptionUI.GetComponent<ItemDescriptionUI>().SetDescription(SlotItem);
        }
        // 그외 다른 아이템이라면
        else {
            BasicDescriptionUI.SetActive(true);
            BasicDescriptionUI.transform.position = eventData.position;
            BasicDescriptionUI.GetComponent<ItemDescriptionUI>().SetDescription(SlotItem);
        }
    }

    // 포인터가 빠져나가면 아이템 설명 UI 비활성화
    public void OnPointerExit(PointerEventData eventData) {
        GearDescriptionUI.SetActive(false);
        BasicDescriptionUI.SetActive(false);
    }
}
