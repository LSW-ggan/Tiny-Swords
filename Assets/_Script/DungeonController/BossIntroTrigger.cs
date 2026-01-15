using UnityEngine;
using Cinemachine;

public class BossIntroTrigger : MonoBehaviour {

    public Transform bossTransform;
    public CinemachineVirtualCamera introCamera;
    public string bossName = "SHAMAN";

    private bool _played = false;

    private void OnTriggerEnter2D(Collider2D col) {
        if (_played) return;
        if (!col.CompareTag("Player")) return;

        _played = true;

        CameraEffect.Instance.PlayBossIntroCutscene(introCamera, bossTransform, bossName);
    }
}
