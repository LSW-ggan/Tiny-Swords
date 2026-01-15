using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour {
    public GameObject Portal = null;

    // 섹션 생존 몬스터 리스트
    private List<Monster> _monsters;
    // Hierachy 창에서 몬스터를 담고있는 빈 오브젝트
    public GameObject MonstersRoot;

    // 섹션 초기화
    public void Initialize() {
        // 포탈 비활성화
        if (Portal != null) {
            Portal.SetActive(false);
        }

        _monsters = new List<Monster>(MonstersRoot.GetComponentsInChildren<Monster>());

        // 섹션 내 몬스터의 OnDead 이벤트 구독
        foreach(var monster in _monsters) {
            monster.OnDead += OnMonsterDead;
        }
    }
    
    // 섹션의 몬스터 처치 시
    private void OnMonsterDead(Monster monster) {
        // 생존 몬스터 리스트에서 제거
        _monsters.Remove(monster);
        // 섹션 클리어 여부 확인
        if(_monsters.Count == 0) {
            Portal.SetActive(true);
        }
    }
}
