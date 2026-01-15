using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour {
    public static SpawnManager Instance;
    
    public GameObject PlayerPrefab;     // 플레이어 프리펩

    private Transform _spawnPos;        // 플레이어 스폰 위치

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 전환 이벤트에 등록
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        MovePlayerToSpawnPos();
    }

    // 각 지역의 스폰 포인트에 플레이어 스폰
    private void MovePlayerToSpawnPos() {
        _spawnPos = GameObject.Find("SpawnPos").GetComponent<Transform>();

        GameObject player = Instantiate(PlayerPrefab, _spawnPos.position, Quaternion.identity);
        
        // 플레이어 데이터 매니저에 플레이어 인스턴스 전달
        PlayerDataManager.Instance.Player = player;
    }
}
