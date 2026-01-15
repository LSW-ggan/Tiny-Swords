using System.Collections.Generic;
using UnityEngine;

public abstract class GearItem : ItemData {
    public enum GearItemType {
        Helmet,
        Armor,
        Greaves,
        Gauntlet,
        Shoes,
        Shield,
        Sword,
        Ring,
    }

    public enum StatType {
        Hp,
        Mp,
        Attack,
        Defense,
        Speed,
        Critical,
        Balance,
    }

    [Header("장비 정보")]
    public GearItemType GearType;
    public int MaxEnhancementLevel = 10;

    [Header("장비 장착 조건")]
    public int EquipLevel = 0;

    [Header("보유 스탯")]
    public int Hp;
    public int Mp;
    public int Attack;
    public int Defense;
    public float Speed;
    public float Critical;
    public int Balance;

    [Header("강화 재료")]
    public EnhanceMaterialItem EnhanceMaterial;

    // 장비 장착 로직 정의
    public abstract void Equip(InventoryItem item);
    // 장비 장착 해제 로직 정의
    public abstract void Unequip(InventoryItem item);

    // 해당 장비가 가지고 있는 Stat만을 조회 (수치가 0인 요소는 배제하고 리턴)
    public abstract Dictionary<StatType, float> GetStats();
}
