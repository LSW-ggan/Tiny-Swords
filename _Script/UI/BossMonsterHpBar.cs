using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossMonsterHpBar : MonoBehaviour {
    public List<Image> _hpBars = null;
    public TMP_Text text;

    private void Start() {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.position = new Vector3(0, -210, 0);
        this.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").transform, false);
        for (int i = 0; i < _hpBars.Count; i++) {
            _hpBars[i].fillAmount = 1.0f;
            _hpBars[i].transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void Awake() {
    }

    public void UpdateHpBar(float currentHp, float maxHp) {
        float ratio = (currentHp / maxHp) * 10f;
        int line = (int)ratio;
        for(int i = line + 1; i < _hpBars.Count; i++) {
            _hpBars[i].fillAmount = 0.0f;
        }

        _hpBars[line].fillAmount = ratio - (float)line;
        if(ratio == 0.0f) {
            Destroy(gameObject);
        }
        text.SetText($"x{line + 1}");
    }
}
