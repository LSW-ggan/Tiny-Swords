using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Stage : MonoBehaviour {
    public List<Section> Sections;  // 스테이지를 구성하는 섹션 리스트
    public Monster BossMonster;     // 스테이지 보스 몬스터
    public int StageBuildNumber;    // 스테이지 빌드 인덱스
    public int TownBuildNumber;     // 해당 스테이지가 속한 마을 씬의 빌드 인덱스
    private float _startTime;       // 플레이 타임 계산용 변수

    private int _currentIndex = 0;
    private bool _isEnd = false;

    private void Start() { 
        StartSection(_currentIndex);
        _startTime = Time.time;

        // 보스 몬스터 사망 이벤트 구독
        BossMonster.OnDead += StageClear;
        // 플레이어 사망 이벤트 구독
        PlayerDataManager.Instance.OnPlayerDead += StageFail;
    }

    private void OnDestroy() {
        // 플레이어 사망 이벤트 구독 해제
        PlayerDataManager.Instance.OnPlayerDead -= StageFail;
    }

    // 다음 섹션으로 이동 처리
    public void MoveToNextSection() {
        _currentIndex++;
        
        StartSection(_currentIndex);
    }

    // 섹션 init
    public void StartSection(int sectionIndex) {
        Sections[sectionIndex].Initialize();
    }

    // 스테이지 클리어
    public void StageClear(Monster monster) {
        if (!_isEnd) {
            StartCoroutine(ShowClearUIAfterDelay());
        }
    }

    // 공략 실패
    public void StageFail() {
        if(!_isEnd) {
            StartCoroutine(ShowFailUIAfterDelay());
            _isEnd = true;
        }
    }

    IEnumerator ShowClearUIAfterDelay() {
        yield return new WaitForSeconds(0.5f);
        GlobalUIManager.Instance.CreateDungeonClearUI(_startTime, StageBuildNumber, TownBuildNumber);
    }

    IEnumerator ShowFailUIAfterDelay() {
        yield return new WaitForSeconds(0.5f);
        GlobalUIManager.Instance.CreateDungeonFailUI(_startTime, StageBuildNumber, TownBuildNumber);
    }

    public void OnClickGiveUpButton() {
        StartCoroutine(CoGiveUp());
    }

    public IEnumerator CoGiveUp() {
        yield return GlobalUIManager.Instance.CreateConfirmUI("공략을 포기하시겠습니까?", (answer) => {
        if (answer) {
                StageFail();
            }
        });
    }
}
