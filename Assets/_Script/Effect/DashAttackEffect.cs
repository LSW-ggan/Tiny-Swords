using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttackEffect : MonoBehaviour {
    public AudioClip DashAttackSoundClip;
    // animation event method
    public void OnAnimationEnd() {
        Destroy(gameObject);
    }

    // animation event method
    public void OutputDashAttackSound() {
        AudioManager.Instance.PlayEffectSound(DashAttackSoundClip);
    }
}
