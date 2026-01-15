using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Portal : MonoBehaviour {
    public Transform Destination;
    public CinemachineVirtualCamera NextCamera;
    public CameraFollow MainCamera;
    public Stage stageController;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            StartCoroutine(CoTeleport(other.transform));
            AudioManager.Instance.PlayPortalSound();
        }
    }

    private IEnumerator CoTeleport(Transform player) {
        CameraEffect.Instance.FadeOut();
        yield return new WaitForSeconds(1.0f);
        player.position = Destination.position;
        MainCamera.SwapVirtualCamera(NextCamera);
        stageController.MoveToNextSection();
        CameraEffect.Instance.FadeIn();
    }
}
