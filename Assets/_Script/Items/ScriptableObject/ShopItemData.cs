using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Shop/ItemData")]
public class ShopItemData : ScriptableObject {
    // 아이템 데이터 SO
    public ItemData Item;
    // 해당 아이템의 판매 가격 정의
    public int Price;
}
