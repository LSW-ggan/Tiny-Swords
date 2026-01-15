using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryEffect : MonoBehaviour {
    public AudioClip HammerSwingSoundClip;
    // animation event method
    public void OnAnimationEnd() {
        Destroy(gameObject);
    }

    // animation event method
    public void OutputSwingSound() {
        AudioManager.Instance.PlayEffectSound(HammerSwingSoundClip);
    }
}
