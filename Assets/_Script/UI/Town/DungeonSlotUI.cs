using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSlotUI : MonoBehaviour {
    public GameObject DungeonBoard;

    private Vector3 _preScale;

    public void OnEventTriggerPointerEnter() {
        _preScale = DungeonBoard.transform.localScale;
        DungeonBoard.transform.localScale = _preScale * 1.2f;
    }

    public void OnEventTriggerPointerExit() {
        DungeonBoard.transform.localScale = _preScale;
    }
}
