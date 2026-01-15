using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryIceSpawner : MonoBehaviour {

    public GameObject iceSpikePrefab;
    private int _iceCount = 3;
    private float _iceSpacing = 2.5f;
    private float _iceDelay = 0.15f;
    private int _attackBase;

    private GameObject _player;
    public HashSet<GameObject> damagedMonsters = new HashSet<GameObject>();

    public void Init(GameObject player, int attackBase) {
        _player = player;
        _attackBase = attackBase;
        StartCoroutine(SpawnIceSequence());
    }

    private IEnumerator SpawnIceSequence() {
        for (int i = 1; i <= _iceCount; i++) {
            GameObject iceObj;
            if (_player.transform.localScale.x > 0) {
                iceObj = Instantiate(iceSpikePrefab, transform.position + Vector3.right * _iceSpacing * i, Quaternion.identity);
            }
            else {
                iceObj = Instantiate(iceSpikePrefab, transform.position + Vector3.left * _iceSpacing * i, Quaternion.Euler(0, 180, 0));
            }
            IceSpike ice = iceObj.GetComponent<IceSpike>();
            ice.Init(this, _player, _attackBase);
            yield return new WaitForSeconds(_iceDelay);
        }

        Destroy(gameObject);
    }
}