using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class QuantityUI : MonoBehaviour {
    public int _itemAmount = 0;
    public TMP_InputField QuantityInputField;
    public TMP_Text DescriptionText;

    public bool isClicked = false;
    public bool Answer = false;
    public int ResultAmount = 0;

    public void SetText(string text) {
        DescriptionText.SetText(text);
    }

    public void SetItemCount(int amount) {
        _itemAmount = amount;
    }
    public void OnClickMin() {
        AudioManager.Instance.PlayButtonSound();
        QuantityInputField.SetTextWithoutNotify("1");
    }

    public void OnClickMinusOne() {
        AudioManager.Instance.PlayButtonSound();
        int num = int.Parse(QuantityInputField.text);
        if (num > 1) {
            QuantityInputField.SetTextWithoutNotify($"{num - 1}");
        }
    }

    public void OnClickPlusOne() {
        AudioManager.Instance.PlayButtonSound();
        int num = int.Parse(QuantityInputField.text);
        if (num < _itemAmount) {
            QuantityInputField.SetTextWithoutNotify($"{num + 1}");
        }
    }

    public void OnClickMax() {
        AudioManager.Instance.PlayButtonSound();
        QuantityInputField.SetTextWithoutNotify($"{_itemAmount}");
    }

    public void OnClickConfirmButton() {
        AudioManager.Instance.PlayButtonSound();
        ResultAmount = int.Parse(QuantityInputField.text);
        if(ResultAmount > _itemAmount) {
            return;
        }
        isClicked = true;
        Answer = true;
    }

    public void OnClickCancelButton() {
        AudioManager.Instance.PlayButtonSound();
        isClicked = true;
        Answer = false;
        ResultAmount = 0;
    }
}
