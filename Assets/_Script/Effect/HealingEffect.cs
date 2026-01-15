using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingEffect : MonoBehaviour {
    public AudioClip HealingSoundClip;
    // animation event method
    public void OnAnimationEnd() {
        Destroy(gameObject);
    }

    // animation event method
    public void OutputHealingSound() {
        AudioManager.Instance.PlayEffectSound(HealingSoundClip);
    }
}
