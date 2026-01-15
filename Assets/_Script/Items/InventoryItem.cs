using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem {
    public ItemData Data;
    public int Amount;
    public int EnhanceLevel = 0;

    public bool IsEquipped = false;
    public int QuickSlotIndex = -1;
    public bool IsQuickEquipped => QuickSlotIndex != -1;


    public int EnhanceHp = 0;
    public int EnhanceAttack = 0;
    public int EnhanceDefense = 0;
    public float EnhanceSpeed = 0;
    public float EnhanceCritical = 0f;
    public int EnhanceBalance = 0;

    public InventoryItem(ItemData data, int amount) {
        this.Data = data;
        this.Amount = amount;
    }

    public Dictionary<GearItem.StatType, float> GetTotalStats() {

        Dictionary<GearItem.StatType, float> result = new();

        if (Data is not GearItem gear)
            return result;

        void Add(GearItem.StatType type, float value) {
            if (value == 0) return;

            if (!result.ContainsKey(type))
                result[type] = 0;

            result[type] += value;
        }

        // 장비 기본 스탯
        foreach (var dict in gear.GetStats()) {
            Add(dict.Key, dict.Value);
        }

        // 강화 보너스 스탯
        Add(GearItem.StatType.Hp, EnhanceHp);
        Add(GearItem.StatType.Attack, EnhanceAttack);
        Add(GearItem.StatType.Defense, EnhanceDefense);
        Add(GearItem.StatType.Speed, EnhanceSpeed);
        Add(GearItem.StatType.Critical, EnhanceCritical);
        Add(GearItem.StatType.Balance, EnhanceBalance);

        return result;
    }

    public void CalculateEnhanceBonus() {
        EnhanceHp = 0;
        EnhanceAttack = 0;
        EnhanceDefense = 0;
        EnhanceSpeed = 0;
        EnhanceCritical = 0f;
        EnhanceBalance = 0;

        if (EnhanceLevel <= 0) return;
        if (Data is not GearItem gear) return;

        for (int i = 1; i <= EnhanceLevel; i++) {
            var bonus = EnhanceStatProvider.GetBonus(gear.GearType, i);
            EnhanceHp += bonus.MaxHp;
            EnhanceAttack += bonus.Attack;
            EnhanceSpeed += bonus.Speed;
            EnhanceDefense += bonus.Defense;
            EnhanceCritical += bonus.Critical;
            EnhanceBalance += bonus.Balance;
        }
    }
}
