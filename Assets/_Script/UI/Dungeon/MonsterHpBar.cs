using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHpBar : MonoBehaviour {
    public Transform HeadPos;

    void LateUpdate() {
        transform.position = new Vector3(HeadPos.position.x - 1.2f + gameObject.transform.localScale.x, HeadPos.position.y, HeadPos.position.z);
    }
    public void UpdateHpBar(float currentHp, float maxHp) {
        float ratio = currentHp / maxHp * 0.6f;
        Vector3 hpScale = transform.localScale;

        if (ratio < 0) ratio = 0;
        transform.localScale = new Vector3(ratio, hpScale.y, hpScale.z);
    }
}
