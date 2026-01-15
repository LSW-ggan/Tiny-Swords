using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour {
    private float _upSpeed = 0.8f; 
	private float _destroyTime = 0.6f;
    public TextMeshPro TargetText;
    private void Start() {
        TargetText = gameObject.GetComponent<TextMeshPro>();
        Destroy(gameObject, _destroyTime);
    }

    private void Update() {
        transform.position += Vector3.up * _upSpeed * Time.deltaTime;
    }

    public void SetText(string text, bool isPop = false) {
        TargetText.SetText(text);
        if (isPop) {
            StartCoroutine(PopAnimation());
        }
    }

    private IEnumerator PopAnimation() {
        Transform t = TargetText.transform;

        // 큰 스케일로 시작
        t.localScale = Vector3.one * 3.0f;
        Color color = TargetText.color;
        color.a = 0f;
        TargetText.color = color;

        float time = 0f;

        // 빠르게 등장
        while (time < 0.25f) {
            time += Time.unscaledDeltaTime;
            float p = time / 0.25f;

            t.localScale = Vector3.Lerp(Vector3.one * 3.0f, Vector3.one * 1.0f, EaseOutBack(p));
            color.a = Mathf.Lerp(0f, 1f, p);
            TargetText.color = color;

            yield return null;
        }
    }

    private float EaseOutBack(float x) {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
}
