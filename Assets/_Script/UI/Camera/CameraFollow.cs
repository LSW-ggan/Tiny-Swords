using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour {
    public CinemachineVirtualCamera PlayerCamera;

    private void Awake() {
        PlayerDataManager.Instance.OnPlayerSpawned += FollowStart;
    }

    private void OnDestroy() {
        PlayerDataManager.Instance.OnPlayerSpawned -= FollowStart;
    }

    private void Start() {
        FollowStart();
    }

    public void SwapVirtualCamera(CinemachineVirtualCamera camera, bool isPlayerFollow = true) {
        PlayerCamera.Priority = 10;
        camera.Priority = 20;
        if(isPlayerFollow) {
            camera.Follow = PlayerCamera.Follow;
            PlayerCamera = camera;
        }
    }

    private void FollowStart() {
        PlayerCamera.Follow = PlayerDataManager.Instance.Player.transform;
    }
}
