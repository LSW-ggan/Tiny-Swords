using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingClub : MonoBehaviour {
    private float _fallHeight = 15f;
    private float _fallSpeed = 12f;
    private int _damage = 20;

    private bool isFalling = false;
    private Vector3 _targetPos;

    public GameObject WarningCirclePrefab;
    public GameObject WarningCircleInstance;
    public GameObject ProgressCircleInstance;
    public AudioClip FallingSoundClip;
    public float WarningTime = 1f;

    public void Init(Vector3 target) {
        _targetPos = target;
        transform.position = target + Vector3.up * _fallHeight;

        WarningCircleInstance = Instantiate(WarningCirclePrefab, _targetPos, Quaternion.identity);

        ProgressCircleInstance = Instantiate(WarningCirclePrefab, _targetPos, Quaternion.identity);

        ProgressCircleInstance.transform.localScale = new Vector3(0f, 0f, 1f);

        StartCoroutine(CoFall());
    }

    private IEnumerator CoFall() {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = new Vector3(4f, 1f, 1f);
        float time = 0f;

        while (time < WarningTime) {
            time += Time.deltaTime;
            float t = time / WarningTime;
            ProgressCircleInstance.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        isFalling = true;
    }

    private void Update() {
        if (!isFalling) return;

        transform.position += Vector3.down * _fallSpeed * Time.deltaTime;

        if (transform.position.y <= _targetPos.y) {
            AudioManager.Instance.PlayEffectSound(FallingSoundClip);
            Hit();
        }
    }

    // 데미지 처리
    private void Hit() {
        isFalling = false;
        WarningCircle circle = WarningCircleInstance.GetComponent<WarningCircle>();
        if (circle.IsPlayerEntered()) {
            PlayerController player = PlayerDataManager.Instance.Player.GetComponent<PlayerController>();
            int damage = DamageCalculator.CalculateMonsterDamage(_damage, player.Defense);
            player.TakeDamege(_damage);
        }
        Destroy(ProgressCircleInstance);
        Destroy(WarningCircleInstance);
        Destroy(gameObject, 1.0f);
    }
}
