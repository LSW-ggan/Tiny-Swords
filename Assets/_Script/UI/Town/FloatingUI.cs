using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUI : MonoBehaviour {
    private Vector3 _headPos;
    private float _range = 0.1f;
    private float _floatingSpeed = 2f;


    void Start() {
        _headPos = transform.localPosition;
    }

    void Update() {
        float y = Mathf.Sin(Time.time * _floatingSpeed) * _range;
        transform.localPosition = _headPos + new Vector3(0, y, 0);
    }
}
