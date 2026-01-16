using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
    protected bool IsNearToPlayer;              // 플레이어와 NPC가 충분히 가까운지
    public GameObject InteractIcon;             // 상호작용 아이콘(현재는 키보드 F키 아이콘)
    protected KeyCode InteractKey = KeyCode.F;  // 상호작용 키
    protected KeyCode ExitKey = KeyCode.Escape; // 상호작용 종료 키

    // NPC가 상점 기능을 포함한다면, 자식 클래스에서 재정의하여 사용
    public virtual ShopData GetShopDate() {
        return null;
    }
}
