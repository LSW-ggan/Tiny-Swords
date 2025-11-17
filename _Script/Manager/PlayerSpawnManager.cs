using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : MonoBehaviour {
    private Transform _spawnPos;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        MovePlayerToSpawnPos();
    }

    private void MovePlayerToSpawnPos() {
        _spawnPos = GameObject.Find("SpawnPos").GetComponent<Transform>();

        Player.Instance.transform.position = _spawnPos.position;
    }
}
