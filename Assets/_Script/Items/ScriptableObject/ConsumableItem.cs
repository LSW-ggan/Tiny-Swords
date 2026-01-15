using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableItem : ItemData {
    // 소모 아이템 공통 로직 정의
    [Header("회복 수치")]
    public int Hp;
    public int Mp;
}
