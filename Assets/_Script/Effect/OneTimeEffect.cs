using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeEffect : MonoBehaviour {
    // animation event method
    public void OnAnimationEnd() {
        Destroy(gameObject);
    }
}
