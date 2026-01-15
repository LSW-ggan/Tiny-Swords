using UnityEngine;
using TMPro;

public class GeneralStoreNPC : NPC {
    // Npc 정보
    public TMP_Text Name;
    private Sprite _icon;

    // NPC ShopData SO
    public ShopData ItemShop;

    // 대사 스크립트
    private string[] _sentences = {
        "필요한 건 다 여기 있어요. 천천히 둘러보세요."
    };

    // 해당 NPC 고유 기능 리스트
    // 스크립트에서 기능 버튼 생성 시, 보유 기능 버튼만 활성화할 수 있도록
    private DialogueManager.DialogueActionType[] _actions = { 
        DialogueManager.DialogueActionType.Exit,
        DialogueManager.DialogueActionType.Shop,
    }; 

    private void Start() {
        _icon = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    // 플레이어가 가까이 접근하면 상호작용 아이콘 표시
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            InteractIcon.SetActive(true);
            IsNearToPlayer = true;
        }
    }

    // 플레이어가 멀어지면 상호작용 아이콘 비활성화
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            InteractIcon.SetActive(false);
            IsNearToPlayer = false;
            // 진행 중이던 대화도 종료되도록
            DialogueManager.Instance.DialoguePanel.EndDialogue();
        }
    }

    // NPC가 보유한 ShopData SO 반환
    public override ShopData GetShopDate() {
        return ItemShop;
    }

    private void Update() {
        // 가까이 접근한 상태에서 상호작용 키를 누른다면 대화 활성화
        if(IsNearToPlayer && Input.GetKeyDown(InteractKey)) {
            DialogueManager.Instance.DialoguePanel.OnDialogue(this, _sentences, Name.text, _icon, _actions);
        }
    }
}
