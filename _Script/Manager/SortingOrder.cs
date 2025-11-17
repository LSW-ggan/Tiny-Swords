using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrder : MonoBehaviour {
    private SpriteRenderer _renderer;
    void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        _renderer.sortingOrder = -(int)(transform.position.y * 100);
    }
}
