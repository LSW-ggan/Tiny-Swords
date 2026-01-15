using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour {
    public AudioClip AttackOneSoundClip;
    public AudioClip AttackTwoSoundClip;
    public AudioClip FirstFootStepSoundClip;
    public AudioClip SecondFootStepSoundClip;
    public AudioClip PlayerTakeDamageSoundClip;
    public AudioClip ShieldGaurdSoundClip;

    // animation method
    public void AttackOneSoundPlay() {
        AudioManager.Instance.PlayEffectSound(AttackOneSoundClip);
    }

    // animation method
    public void AttackTwoSoundPlay() {
        AudioManager.Instance.PlayEffectSound(AttackTwoSoundClip);
    }

    // animation method
    public void FirstFootStepSoundPlay() {
        AudioManager.Instance.PlayEffectSound(FirstFootStepSoundClip);
    }

    // animation method
    public void SecondFootStepThreeSoundPlay() {
        AudioManager.Instance.PlayEffectSound(SecondFootStepSoundClip);
    }
}
