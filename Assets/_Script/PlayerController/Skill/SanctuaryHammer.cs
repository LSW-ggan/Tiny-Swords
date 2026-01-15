using System.Collections;
using UnityEngine;

public class SanctuaryHammer : MonoBehaviour {

    public GameObject iceSpikeSpawnerPrefab;

    private GameObject _player;
    private int _attackBase;
    private Vector3 _offset = new Vector3(5.0f, 0, 0);
    
    public void Init(GameObject player, int attackBase) {
        _player = player;
        _attackBase = attackBase;
    }

    // hammer animation event method
    public void EndHammerImpact() {
        _offset *= _player.transform.localScale.x > 0 ? 1 : -1;

        GameObject spawner = Instantiate(iceSpikeSpawnerPrefab, transform.position + _offset, Quaternion.identity);
        CameraEffect.Instance.Shake(0.25f);

        SanctuaryIceSpawner iceSpawner = spawner.GetComponent<SanctuaryIceSpawner>();
        iceSpawner.Init(_player, _attackBase);

        PlayerController playerController = _player.GetComponent<PlayerController>();
        playerController._isAttacking = false;

        Destroy(gameObject);
    }
}