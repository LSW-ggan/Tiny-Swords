using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class Heal : Skill {
    public Heal(SkillData data) : base(data) {
        LastUsedTime = -data.CoolTime;
    }

    public override void SkillUse(GameObject player) {
        LastUsedTime = Time.time;

        ManaDecrease();

        PlayerDataManager.Instance.Hp += Data.HealAmount[SkillLevel];
        if(Data.EffectPrefab) {
            GameObject effect = GameObject.Instantiate(Data.EffectPrefab, player.transform.position, Quaternion.identity);
            effect.GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder + 3;
            effect.transform.parent = player.transform;
        }
    }
}
