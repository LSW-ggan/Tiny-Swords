using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDragSlotUI : MonoBehaviour {
    public Image ItemIcon;

    public bool IsDragging { get; private set; }

    private void Start() {
        ItemIcon.enabled = false;
        IsDragging = false;
    }

    // 드래그 시작 시, 슬롯 아이템의 sprite를 받아옴
    public void Drag(Sprite sprite) {
        IsDragging = true;
        ItemIcon.enabled = true;
        ItemIcon.sprite = sprite;
    }

    // 드래그 종료 시, 비활성화
    public void EndDrag() {
        IsDragging = false;
        ItemIcon.enabled = false;
    }

    // 드래그 중에 마우스를 따라갈 수 있도록 호출되는 메서드
    public void MouseFollow(Vector3 pos) {
        if (!IsDragging) return;
        ItemIcon.rectTransform.position = pos;
    }
}
