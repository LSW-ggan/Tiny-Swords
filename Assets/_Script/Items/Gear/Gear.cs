using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(menuName = "Item/Gear")]
public class Gear : GearItem {
    // 아이템 획득
    public override void GetItem() {
        IsStackable = false;
        // 인벤토리에 추가
        InventoryManager.Instance.AddItem(this, 1);
    }

    // 장비 장착 
    public override void Use(InventoryItem item) {
        // 인벤토리 매니저 호출 -> 장비 장착 요청 전달 / 기존 장비 해제, 새로운 장비 장착, UI반영 등의 순차 처리 위함
        InventoryManager.Instance.RequestEquip(item, (int)GearType);
    }

    // 장비 장착
    public override void Equip(InventoryItem item) {
        // 장비의 스탯을 플레이어의 보너스 스탯에 반영
        PlayerDataManager.Instance.BonusMaxHp += (Hp + item.EnhanceHp);
        PlayerDataManager.Instance.Hp += (Hp + item.EnhanceHp);
        PlayerDataManager.Instance.BonusMaxMp += Mp;
        PlayerDataManager.Instance.Mp += Mp;
        PlayerDataManager.Instance.BonusAttack += (Attack + item.EnhanceAttack);
        PlayerDataManager.Instance.BonusSpeed += (Speed + item.EnhanceSpeed);
        PlayerDataManager.Instance.BonusCritical += (Critical + item.EnhanceCritical);
        PlayerDataManager.Instance.BonusBalance += (Balance + item.EnhanceBalance);
        PlayerDataManager.Instance.BonusDefense += (Defense + item.EnhanceDefense);
    }

    // 장비 장착 해제
    public override void Unequip(InventoryItem item) {
        // 현재 장비가 가지고 있는 스탯을 플레이어의 보너스 스탯에서 경감
        PlayerDataManager.Instance.BonusMaxHp -= (Hp + item.EnhanceHp);
        PlayerDataManager.Instance.BonusMaxMp -= Mp;

        PlayerDataManager.Instance.BonusAttack -= (Attack + item.EnhanceAttack);
        PlayerDataManager.Instance.BonusDefense -= (Defense + item.EnhanceDefense);
        PlayerDataManager.Instance.BonusSpeed -= (Speed + item.EnhanceSpeed);
        PlayerDataManager.Instance.BonusCritical -= (Critical + item.EnhanceCritical);
        PlayerDataManager.Instance.BonusBalance -= (Balance + item.EnhanceBalance);
    }

    // 현재 장비가 보유한 스탯을 리턴
    public override Dictionary<StatType, float> GetStats() {
        Dictionary<StatType, float> stats = new();

        if (Hp != 0) stats[StatType.Hp] = Hp;
        if (Mp != 0) stats[StatType.Mp] = Mp;
        if (Attack != 0) stats[StatType.Attack] = Attack;
        if (Defense != 0) stats[StatType.Defense] = Defense;
        if (Speed != 0) stats[StatType.Speed] = Speed;
        if (Critical != 0) stats[StatType.Critical] = Critical;
        if (Balance != 0) stats[StatType.Balance] = Balance;

        return stats;
    }
}