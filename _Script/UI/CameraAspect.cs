using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspect : MonoBehaviour {
    public float targetAspect = 16f / 9f;

    void Start() {
        UpdateMask();
    }

    void Update() {
        // 창 크기 변하면 자동 업데이트
        if (Screen.width != prevW || Screen.height != prevH)
            UpdateMask();
    }

    private int prevW, prevH;

    void UpdateMask() {
        prevW = Screen.width;
        prevH = Screen.height;

        Camera cam = GetComponent<Camera>();
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f) {
            // Letterbox (위/아래)
            Rect rect = cam.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;
            cam.rect = rect;
        }
        else {
            // Pillarbox (좌/우)
            float scaleWidth = 1f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;
            cam.rect = rect;
        }
    }
}
