using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {
    public Image NPCIcon;
    public TMP_Text NPCName;
    public TMP_Text DialogueText;
    public GameObject NextText;
    public CanvasGroup DialogueGroup;
    public GameObject ActionButtonParent;
    public GameObject[] ActionButtonPrefab;
    public GameObject ShopUIPrefab;
    public GameObject EnhanceUIPrefab;
    public AudioClip TypingSoundClip;

    public Queue<string> Sentences;

    private NPC _currentNPC;
    private string _currentSentence;
    private float _typingSpeed = 0.03f;
    private bool _isTyping = false;
    private List<GameObject> _actionButtons;
    DialogueManager.DialogueActionType[] actions;

    private void Start() {
        Sentences = new Queue<string>();
        _actionButtons = new List<GameObject>();
    }
    private void Update() {
        if (DialogueText.text == _currentSentence) {
            if(Sentences.Count != 0) {
                NextText.SetActive(true);
            }
            _isTyping = false;
        }
        // 대사 타이핑이 끝난 후, Enter 누르면 다음 스크립트로 진행
        if(!_isTyping && Input.GetKeyDown(KeyCode.Return)) {
            NextSentence();
        }
        // 대화 큐가 빈 경우 대화 종료
        if(Sentences.Count == 0 && Input.GetKeyDown(KeyCode.Escape)) {
            EndDialogue();
        }
    }

    // 대화 시작
    public void OnDialogue(NPC npc, string[] lines, string name, Sprite icon, DialogueManager.DialogueActionType[] actions) {
        // 대화창 세팅
        NPCIcon.sprite = icon;
        NPCName.SetText(name);
        this.actions = actions;
        _currentNPC = npc;
        Sentences.Clear();
        // 대화 스크립트 초기화
        foreach (string line in lines) {
            Sentences.Enqueue(line);
        }
        // 대화창 UI 화면에 표시
        DialogueGroup.alpha = 1.0f;
        DialogueGroup.blocksRaycasts = true;

        NextSentence();
    }

    // 대화 스크립트 진행
    public void NextSentence() {
        if(DialogueGroup.alpha != 1.0f) {
            return;
        }
        // 큐에 남은 대사 스크립트가 0이 될때까지 실행
        if (Sentences.Count != 0) {
            _isTyping = true;
            NextText.SetActive(false);
            _currentSentence = Sentences.Dequeue();
            StartCoroutine(CoTyping(_currentSentence));
        }
        // 큐에 남은 대사가 없다면 기능 버튼 표시
        if(Sentences.Count == 0) { 
            CreateActionButton();
        }
    }

    // 대화 종료 시, 대화창 UI 초기화
    public void EndDialogue() {
        DialogueGroup.alpha = 0.0f;
        DialogueGroup.blocksRaycasts = false;
        foreach(GameObject obj in _actionButtons) {
            Destroy(obj);
        }
    }

    // 대사 스크립트 타이밍 효과
    private IEnumerator CoTyping(string line) {
        string text = "";
        DialogueText.SetText(text);
        foreach (char letter in line.ToCharArray()) {
            text += letter;
            DialogueText.SetText(text);
            AudioManager.Instance.PlayEffectSound(TypingSoundClip);

            yield return new WaitForSeconds(_typingSpeed);
        }
    }

    // 각 기능 버튼 타입별로 Listener연결
    private void CreateActionButton() {
        int index = 0;
        foreach(DialogueManager.DialogueActionType action in actions) {
            GameObject obj = Instantiate(ActionButtonPrefab[index], ActionButtonParent.transform);
            Button btn = obj.GetComponent<Button>();
            TMP_Text text = obj.GetComponentInChildren<TMP_Text>();
            switch (action) {
                case DialogueManager.DialogueActionType.Shop:
                    btn.onClick.AddListener(OnClickShop);
                    text.SetText("상점");
                    break;
                case DialogueManager.DialogueActionType.Exit:
                    btn.onClick.AddListener(OnClickExit);
                    text.SetText("대화 종료");
                    break;
                case DialogueManager.DialogueActionType.Enhancement:
                    btn.onClick.AddListener(OnClickEnhancement);
                    text.SetText("장비 강화");
                    break;
            }
            _actionButtons.Add(obj);
            index++;
        }
    }


    // 상점 기능 버튼 클릭 시
    private void OnClickShop() {
        AudioManager.Instance.PlayButtonSound();
        if (_currentNPC.GetShopDate() != null) {
            GameObject shop = Instantiate(ShopUIPrefab, GlobalUIManager.Instance.GlobalCanvas.transform);
            shop.GetComponent<ShopUI>().Open(NPCName.text, NPCIcon.sprite, _currentNPC.GetShopDate());
        }
        EndDialogue();
    }

    // 강화 기능 버튼 클릭 시
    private void OnClickEnhancement() {
        AudioManager.Instance.PlayButtonSound();
        GameObject enhance = Instantiate(EnhanceUIPrefab, GlobalUIManager.Instance.GlobalCanvas.transform);
        InventoryManager.Instance.SetMode(InventoryManager.InventoryMode.Enhance);
        EndDialogue();
    }

    // 대화 종료 버튼 클릭 시
    private void OnClickExit() {
        EndDialogue();
    }
}
