using TMPro;
using UnityEngine;

public class GearStoreNPC : NPC {
    public TMP_Text Name;
    public ShopData ItemShop;

    private Sprite _icon;
    private string[] _sentences = {
        "오, 손님이군!",
        "무기 상태가 썩 좋아 보이진 않네.\r\n새 장비 하나 장만하는 게 어때?"
    };
    private DialogueManager.DialogueActionType[] _actions = {
        DialogueManager.DialogueActionType.Exit,
        DialogueManager.DialogueActionType.Shop,
    };

    private void Start() {
        _icon = gameObject.GetComponent<SpriteRenderer>().sprite;
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            InteractIcon.SetActive(true);
            IsNearToPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            InteractIcon.SetActive(false);
            IsNearToPlayer = false;
            DialogueManager.Instance.DialoguePanel.EndDialogue();
        }
    }

    public override ShopData GetShopDate() {
        return ItemShop;
    }

    private void Update() {
        if (IsNearToPlayer && Input.GetKeyDown(KeyCode.F)) {
            DialogueManager.Instance.DialoguePanel.OnDialogue(this, _sentences, Name.text, _icon, _actions);
        }
    }
}
