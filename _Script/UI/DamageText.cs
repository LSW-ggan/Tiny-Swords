using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour {
    private float _upSpeed = 20f; 
	private float _destroyTime = 0.5f;
    public TextMeshProUGUI TextComp;
    private GameObject _uiCanvas;
    private void Start() {
        _uiCanvas = GameObject.FindGameObjectWithTag("Canvas");
        gameObject.transform.SetParent(_uiCanvas.transform);
        Invoke("DestroyText", _destroyTime);
    }

    private void DestroyText() {
        Destroy(gameObject);
    }

    private void Update() {
        transform.position += Vector3.up * _upSpeed * Time.deltaTime;
    }

    public void SetText(string damage) {
        TextComp.text = damage;
    }
}
