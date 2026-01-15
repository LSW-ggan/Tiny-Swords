using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpEffect : MonoBehaviour {
    private GameObject _player;

    void Start() {
        _player = PlayerDataManager.Instance.Player;
        gameObject.transform.position = _player.transform.position;
        gameObject.transform.parent = _player.transform;
    }

    // animation event method
    public void OnAnimationEnd() {
        Destroy(gameObject); 
    }
}
