using System.Collections;
using TMPro;
using UnityEngine;

public class BossIntroUI : MonoBehaviour {
    public static BossIntroUI Instance;

    public TMP_Text nameText;
    public AudioClip BossIntroSoundClip;

    private void Awake() {
        Instance = this;
        nameText.gameObject.SetActive(false);
    }

    public void Show(string bossName) {
        nameText.gameObject.SetActive(true);
        nameText.text = bossName;
        StopAllCoroutines();
        StartCoroutine(PopAnimation());
    }

    public void Hide() {
        nameText.gameObject.SetActive(false);
    }

    private IEnumerator PopAnimation() {
        AudioManager.Instance.MuteBGM();
        AudioManager.Instance.PlayEffectSound(BossIntroSoundClip);
        yield return new WaitForSeconds(0.3f);
        Transform t = nameText.transform;

        // 큰 스케일로 시작
        t.localScale = Vector3.one * 3.0f;
        Color color = nameText.color;
        color.a = 0f;
        nameText.color = color;

        float time = 0f;

        // 빠르게 등장
        while (time < 0.25f) {
            time += Time.unscaledDeltaTime;
            float p = time / 0.25f;

            t.localScale = Vector3.Lerp(Vector3.one * 3.0f, Vector3.one * 1.0f, EaseOutBack(p));
            color.a = Mathf.Lerp(0f, 1f, p);
            nameText.color = color;

            yield return null;
        }
        CameraEffect.Instance.Shake();

        // 잠깐 유지
        yield return new WaitForSecondsRealtime(2.0f);

        // 서서히 사라짐
        time = 0f;
        while (time < 0.7f) {
            time += Time.unscaledDeltaTime;
            float p = time / 0.7f;

            color.a = Mathf.Lerp(1f, 0f, p);
            nameText.color = color;
            yield return null;
        }

        nameText.gameObject.SetActive(false);
        AudioManager.Instance.UnmuteBGM();
    }

    // easing 함수 (살짝 튕기는 느낌 넣어주기)
    private float EaseOutBack(float x) {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
}