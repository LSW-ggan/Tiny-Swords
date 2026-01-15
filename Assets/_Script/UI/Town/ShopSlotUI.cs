using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour, 
    IPointerClickHandler, 
    IPointerEnterHandler,
    IPointerExitHandler 
    {
    public Image Icon;
    public TMP_Text NameText;
    public TMP_Text PriceText;
    public GameObject QuantityUIPrefab;
    public GameObject GearDescriptionUI;
    public GameObject BasicDescriptionUI;

    private ShopItemData _data;

    // 판매 아이템 슬롯 세팅
    public void SetSlot(ShopItemData data) {
        _data = data;
        Icon.sprite = _data.Item.Icon;
        NameText.SetText(_data.Item.Name);
        PriceText.SetText(_data.Price.ToString());
    }

    // 더블 클릭 시 구매 로직 실행
    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.clickCount > 1) {
            AudioManager.Instance.PlayButtonSound();
            StartCoroutine(Purchase());
        }
    }

    // 아이템 구매 로직
    private IEnumerator Purchase() {
        bool isCancel = false;
        int purchaseAmount = 1;

        // 골드 부족 체크
        if(InventoryManager.Instance.Gold / _data.Price == 0) {
            GlobalUIManager.Instance.CreateAlertUI("골드가 부족합니다.");
        }
        // 인벤토리에 여유 공간이 있는지
        else if (InventoryManager.Instance.IsFull(_data.Item)) {
            GlobalUIManager.Instance.CreateAlertUI("인벤토리가 가득 찼습니다.");
        }
        else {
            yield return StartCoroutine(GlobalUIManager.Instance.CreateQuantityUI("구매 수량을 입력해주세요.", 
                (int)(InventoryManager.Instance.Gold / _data.Price), (answer, amount) => {
                    isCancel = !answer;
                    purchaseAmount = amount;
                }
            ));
            // 구매 처리
            if (!isCancel) {
                InventoryManager.Instance.Gold -= (_data.Price * purchaseAmount);
                InventoryManager.Instance.AddItem(_data.Item, purchaseAmount);
                AudioManager.Instance.PlayEffectSound(InventoryManager.Instance.ItemPurchaseAudioClip);
            }
        }
    }

    // 판매 아이템 설명창 UI 표시
    public void OnPointerEnter(PointerEventData eventData) {
        if (_data == null) return;

        if (_data.Item is GearItem) {
            GearDescriptionUI.SetActive(true);
            GearDescriptionUI.transform.position = eventData.position;
            GearDescriptionUI.GetComponent<ItemDescriptionUI>().SetDescription(new InventoryItem(_data.Item, 1));
        }
        else {
            BasicDescriptionUI.SetActive(true);
            BasicDescriptionUI.transform.position = eventData.position;
            BasicDescriptionUI.GetComponent<ItemDescriptionUI>().SetDescription(new InventoryItem(_data.Item, 1));
        }
    }

    // 판매 아이템 설명창 UI 비활성화
    public void OnPointerExit(PointerEventData eventData) {
        GearDescriptionUI.SetActive(false);
        BasicDescriptionUI.SetActive(false);
    }
}