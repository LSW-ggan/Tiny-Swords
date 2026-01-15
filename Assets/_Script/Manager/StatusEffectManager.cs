using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour {
    public static StatusEffectManager Instance;

    public GameObject MonsterTakeDamageText;
    public GameObject MonsterCriticalDamageText;
    public GameObject PlayerTakeDamageText;
    public GameObject LevelUpText;
    public GameObject DefaultText;

    public AudioClip DamageTextSoundClip;
    public AudioClip DefaultTextSoundClip;

    // 활성화된 상태이상 효과 리스트
    private List<(IStatusEffect effect, StatusEffectData data, bool isPlayerStatusEffect)> _activeEffects
        = new List<(IStatusEffect, StatusEffectData, bool)>();

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // String Floating 텍스트 생성
    public void CreateFloatingText(Vector3 worldPosition, string text, TextType type, GameObject target = null) {
        GameObject textObj;
        GameObject floatingText;
        bool isAttached = false;
        bool isPopText = false;
        AudioClip audioClip = null;
        switch (type) {
            case TextType.LevelUp:
                floatingText = LevelUpText;
                isAttached = true;
                isPopText = true;
                break;
            case TextType.PlayerTakeDamage:
                floatingText = PlayerTakeDamageText;
                isPopText = true;
                break;
            default:
                audioClip = DefaultTextSoundClip;
                floatingText = DefaultText;
                break;
        }
        textObj = Instantiate(floatingText, worldPosition, Quaternion.identity);
        if (isAttached && target != null) {
            textObj.transform.SetParent(target.transform);
        }

        textObj.GetComponent<FloatingText>().SetText(text, isPopText);
        if (audioClip != null) {
            AudioManager.Instance.PlayEffectSound(audioClip);
        }
    }

    // Damage Floating 텍스트 생성
    public void CreateFloatingText(Vector3 worldPosition, List<(int, TextType)> nums) {
        StartCoroutine(CoFloating(worldPosition, nums));
    }

    // Damage Floating 텍스트 생성 코루틴
    public IEnumerator CoFloating(Vector3 worldPosition, List<(int, TextType)> nums) {
        GameObject textObj;
        GameObject floatingText;
        AudioClip audioClip = null;
        bool isPopText = false;
        Vector3 space = new Vector3(0, 0.6f, 0);
        // Floating 텍스트의 타입에 따라 다른 스타일을 적용
        for (int i = 0; i < nums.Count; i++) {
            switch (nums[i].Item2) {
                case TextType.MonsterTakeDamage:
                    audioClip = DamageTextSoundClip;
                    floatingText = MonsterTakeDamageText;
                    isPopText = true;
                    break;
                case TextType.PlayerTakeDamage:
                    floatingText = PlayerTakeDamageText;
                    isPopText = true;
                    break;
                case TextType.MonsterCriticalDamage:
                    audioClip = DamageTextSoundClip;
                    floatingText = MonsterCriticalDamageText;
                    isPopText = true;
                    break;
                default:
                    audioClip = DefaultTextSoundClip;
                    floatingText = DefaultText;
                    break;
            }
            // Floating 텍스트 생성
            textObj = Instantiate(floatingText, worldPosition + space * i, Quaternion.identity);
            textObj.GetComponent<FloatingText>().SetText(nums[i].Item1.ToString(), isPopText);
            if(audioClip != null) {
                AudioManager.Instance.PlayEffectSound(audioClip);
            }

            // 데미지와 같이 여러 줄을 출력할 때, 약간의 텀을 두고 출력
            yield return new WaitForSeconds(0.1f);
        }
    }

    // 플레이어 리스폰 시, 기존 버프 이펙트 다시 적용
    public void PlayerReapplyStatusEffect(GameObject player) {
        foreach (var activeEffect in _activeEffects) { 
            if (activeEffect.isPlayerStatusEffect) {
                activeEffect.effect.ReapplyEffect(player, activeEffect.data);
            }
        }
    }

    // 상태 이상 타입에 따라 상태이상 인스턴스 생성 후 리턴
    private IStatusEffect CreateStatusEffectInstance(StatusEffectType type) {
        switch (type) {
            case StatusEffectType.FireAura:
                return new FireAura();
            case StatusEffectType.Freeze:
                return new Freeze();
        }

        return null;
    }

    // 상태 이상 적용
    public void ApplyStatusEffect(StatusEffectData data, GameObject target, float value) {
        IStatusEffect effect = CreateStatusEffectInstance(data.Type);

        if (effect == null) {
            return;
        }

        effect.Apply(target, data, value);

        // 활성화된 상태이상 인스턴스를 _activeEffects 리스트에 추가하여 관리
        _activeEffects.Add((effect, data, target));

        // 상태 이상 효과의 지속시간이 끝난 후에는 자동으로 해제
        StartCoroutine(RemoveAfterDurationTime(effect, data, target));
    }

    // 상태 이상 효과 제거
    private IEnumerator RemoveAfterDurationTime(IStatusEffect effect, StatusEffectData data, GameObject target) {
        // 효과 지속시간 동안 대기
        yield return new WaitForSeconds(data.Duration);

        // 이펙트 효과 제거
        effect.Remove(target, data);
        // 활성 리스트에서 제거
        _activeEffects.RemoveAll(e => e.effect == effect);
    }
}
