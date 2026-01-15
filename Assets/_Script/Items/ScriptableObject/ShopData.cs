using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Shop/ShopData")]
public class ShopData : ScriptableObject {
    // 판매 아이템 리스트
    public ShopItemData[] Items;
}
