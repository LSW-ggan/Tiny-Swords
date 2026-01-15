using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventoryManager;

public class ShopUI : MonoBehaviour {
    // 상점 판매 아이템 슬롯
    public List<ShopSlotUI> Slots;

    // 개별 판매 아이템 슬롯 Prefab
    public GameObject ItemSlotPrefab;

    // Grid Layout 설정값
    public GridLayoutGroup Grid;
    public GameObject Content;
    public RectTransform ContentRect;
    
    // 아이템 설명UI
    public GameObject BasicDescriptionUI;
    public GameObject GearDescriptionUI;

    // 상점 세팅
    public Image NPCIcon;
    public TMP_Text NameText;

    // 상점 SO 데이터
    private ShopData Shop;

    public void Open(string name, Sprite icon, ShopData data) {
        InventoryManager.Instance.SetMode(InventoryMode.Trading);

        NPCIcon.sprite = icon;
        NameText.SetText(name);
        Shop = data;
        Slots = new List<ShopSlotUI>();
        RefreshShopUI();
    }

    // 아이템 샵 갱신
    private void RefreshShopUI() {
        for (int i = 0; i < Shop.Items.Length; i++) {
            ShopSlotUI ItemSlot = Instantiate(ItemSlotPrefab, Content.transform).GetComponent<ShopSlotUI>();
            ItemSlot.BasicDescriptionUI = BasicDescriptionUI;
            ItemSlot.GearDescriptionUI = GearDescriptionUI;
            Slots.Add(ItemSlot);
            Slots[i].SetSlot(Shop.Items[i]);
        }
    }

    public void OnClickExitButton() {
        AudioManager.Instance.PlayButtonSound();
        InventoryManager.Instance.SetMode(InventoryMode.Normal);
        Destroy(gameObject);
    }

    private void OnDestroy() {
        InventoryManager.Instance.SaveData();
    }
}
