using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform player;
    public float smooth = 5f;

    public float minX = 0; // 왼쪽 벽
    public float maxX = 0; // 오른쪽 벽

    private float halfWidth;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate() {
        if (player == null) return;
        if (minX == 0 && maxX == 0) {
            minX = -6.5f;
            maxX = 8.4f;
        }

        Vector3 pos = transform.position;
        pos.x = player.position.x;
        if(pos.x > maxX) {
            pos.x = maxX;
        }
        if (pos.x < minX) {
            pos.x = minX;
        }
        transform.position = pos;
    }

    public void SetBounds(float left, float right) {
        minX = left;
        maxX = right;
    }
}
