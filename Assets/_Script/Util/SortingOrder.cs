using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrder : MonoBehaviour {
    private SpriteRenderer _renderer;
    void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (gameObject.tag == "Projectile") {
            _renderer.sortingOrder = -(int)(transform.position.y * 100) + 150;
        }
        else {
            _renderer.sortingOrder = -(int)(transform.position.y * 100);
        }
    }
}
