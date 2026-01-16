using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour {
    public static DialogueManager Instance;

    public GameObject DialogueUIPrefab;
    public DialogueUI DialoguePanel { get; private set; }

    public enum DialogueActionType {
        Shop,
        Enhancement,
        Exit
    }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start() {
        DialoguePanel.transform.SetAsLastSibling();
    }

    public void OnDestroy() {
        if (Instance == this) Instance = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex != (int)Scenes.BuildNumber.Main && scene.buildIndex != (int)Scenes.BuildNumber.Loading) {
            DialoguePanel = Instantiate(DialogueUIPrefab, GlobalUIManager.Instance.GlobalCanvas.transform).GetComponent<DialogueUI>();
        }
    }
}
