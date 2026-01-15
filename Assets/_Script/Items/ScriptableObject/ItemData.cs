using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject {
    public enum ItemType {
        ConsumableItem,
        Gear,
        Material,
        Other,
        FieldItem,
    }
    [Header("기본 정보")]
    public int Id;
    public ItemType Type;
    public string Name;
    public int Amount;
    public int SellPrice;
    public bool IsStackable = true;

    [Header("리소스")]
    public Sprite Icon;
    public GameObject ItemPrefab;

    [Header("아이템 설명")]
    public string Description;

    // 아이템 획득 시 처리 로직 정의
    public abstract void GetItem();

    // 아이템 사용 시 처리 로직 정의
    public abstract void Use(InventoryItem item);
}
