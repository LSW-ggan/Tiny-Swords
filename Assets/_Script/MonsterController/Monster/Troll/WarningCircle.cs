using System.Collections.Generic;
using UnityEngine;

public class WarningCircle : MonoBehaviour {
    private bool _isPlayerEntered = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            _isPlayerEntered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            _isPlayerEntered = false;
        }
    }

    public bool IsPlayerEntered() {
        return _isPlayerEntered;
    }
}