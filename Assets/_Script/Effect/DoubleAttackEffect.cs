using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DouvleAttackEffect : MonoBehaviour {
    public AudioClip FirstAttackSoundClip;
    public AudioClip SecondAttackSoundClip;

    // animation event method
    public void OnAnimationEnd() {
        Destroy(gameObject);
    }
    
    // animation event method
    public void OutputFirstAttackSound() {
        AudioManager.Instance.PlayEffectSound(FirstAttackSoundClip);
    }

    // animation event method
    public void OutputSecondAttackSound() {
        AudioManager.Instance.PlayEffectSound(SecondAttackSoundClip);
    }

}
