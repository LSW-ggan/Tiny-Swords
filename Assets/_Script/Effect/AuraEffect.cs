using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraEffect : MonoBehaviour {
    public AudioClip AuraExplosionSoundClip;
    // animation event method
    public void OnAnimationEnd() {
        Destroy(gameObject);
    }

    // animation event method
    public void OutputExplosionSound() {
        AudioManager.Instance.PlayEffectSound(AuraExplosionSoundClip);
    }
}
