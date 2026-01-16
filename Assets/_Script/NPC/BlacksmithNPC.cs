using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlacksmithNPC : NPC {
    public TMP_Text Name;

    public Sprite Icon;
    private string[] _sentences = {
        "어이쿠, 손이 미끄러졌네! 하지만 내 탓이 아니오, 망치 끝에 걸린 달빛이 너무 아름다워서..."
    };
    private DialogueManager.DialogueActionType[] _actions = {
        DialogueManager.DialogueActionType.Exit,
        DialogueManager.DialogueActionType.Enhancement,
    };

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

    private void Update() {
        if (IsNearToPlayer && Input.GetKeyDown(InteractKey)) {
            DialogueManager.Instance.DialoguePanel.OnDialogue(this, _sentences, Name.text, Icon, _actions);
        }
        if(Input.GetKeyDown(ExitKey)) {
            DialogueManager.Instance.DialoguePanel.EndDialogue();
        }
    }
}
