using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDeadEffect : MonoBehaviour {
    void Start() {
        Destroy(gameObject, 1.0f);
    }
}
