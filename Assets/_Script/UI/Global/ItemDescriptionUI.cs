using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GearItem;

public class ItemDescriptionUI : MonoBehaviour {
    public TMP_Text NameText;
    public TMP_Text DescriptionText;
    public TMP_Text StatListText;
    public TMP_Text StatNumText;
    public Image Icon;
    public TMP_Text priceText;

    public void SetDescription(InventoryItem item) {
        switch(item.Data.Type) {
            case ItemData.ItemType.Gear:
                SetGearDesription(item);
                break;
            case ItemData.ItemType.ConsumableItem:
            case ItemData.ItemType.Material:
            case ItemData.ItemType.Other:
                SetBasicDescription(item);
                break;
        }
    }

    private void SetGearDesription(InventoryItem item) {
        NameText.SetText($"+{item.EnhanceLevel} " + item.Data.Name);
        Icon.sprite = item.Data.Icon;
        DescriptionText.SetText(item.Data.Description);

        GearItem gearInfo = (GearItem)(item.Data);
        string statList = "";
        string statNum = "";
        foreach (var stat in item.GetTotalStats()) {
            string label = stat.Key switch {
                StatType.Hp => "체력",
                StatType.Mp => "마나",
                StatType.Attack => "공격력",
                StatType.Defense => "방어력",
                StatType.Speed => "이동 속도",
                _ => ""
            };

            statList += $"{label}\n";
            statNum += $"{stat.Value}\n";
        }
        StatListText.SetText(statList);
        StatNumText.SetText(statNum);
        priceText.SetText(item.Data.SellPrice.ToString());
    }

    private void SetBasicDescription(InventoryItem item) {
        NameText.SetText(item.Data.Name);
        Icon.sprite = item.Data.Icon;
        DescriptionText.SetText(item.Data.Description);
        priceText.SetText(item.Data.SellPrice.ToString());
    }
}
