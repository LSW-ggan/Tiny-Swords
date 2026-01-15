using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    // 날아가는 방향
    public Vector3 AttackDir = new Vector3(0, 0, 0);
    
    // 속도
    protected abstract float speed { get; }
    
    // 플레이어 오브젝트 컴포넌트 (유도 투사체에서 사용)
    protected PlayerController player;
    protected Transform playerTransform;

    protected virtual void Start() {
        GameObject obj = PlayerDataManager.Instance.Player;
        player = obj.GetComponent<PlayerController>();
        playerTransform = obj.GetComponent<Transform>();
    }
}
