using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour {
    // 각 슬롯의 스킬 사용
    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) UseSlot(0);
        if (Input.GetKeyDown(KeyCode.W)) UseSlot(1);
        if (Input.GetKeyDown(KeyCode.E)) UseSlot(2);
        if (Input.GetKeyDown(KeyCode.R)) UseSlot(3);
    }

    // 슬롯 사용 시그널을 SkillManager로 전달 -> SkillManager에서 스킬 시전 처리 
    void UseSlot(int index) {
        SkillManager.Instance.UseSkill(index, gameObject);
    }
}
