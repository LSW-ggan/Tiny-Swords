using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionManager : MonoBehaviour {
    public GameObject Portal;
    private List<Monster> _monsters;
    private StageManager _stageManager;

    public float minX;
    public float maxX;

    public void Initialize(StageManager stageManager) {
        _stageManager = stageManager;
        Portal.SetActive(false);

        _stageManager.cameraFollow.SetBounds(minX, maxX);

        _monsters = new List<Monster>(GetComponentsInChildren<Monster>());

        foreach(var monster in _monsters) {
            monster.OnDead += OnMonsterDead;
        }
    }
    
    private void OnMonsterDead(Monster monster) {
        _monsters.Remove(monster);
        if(_monsters.Count == 0) {
            Portal.SetActive(true);
        }
    }
}
