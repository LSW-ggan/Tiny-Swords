using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class GlobalUIManager : MonoBehaviour {
    public static GlobalUIManager Instance;

    public GameObject AbilityUIPrefab;
    public GameObject SkillUIPrefab;
    public GameObject InventoryUIPrefab;

    private GameObject _abilityUIInstance = null;
    private GameObject _skillUIInstance = null;
    private GameObject _inventoryUIInstance = null;

    public GameObject TestUIPrefab;
    private GameObject _testUIInstance = null;

    public GameObject PlayerUIPrefab;
    public GameObject QuickSlotUIPrefab;
    public GameObject SkillSlotUIPrefab;

    public GameObject DungeonClearUIPrefab;
    public GameObject DungeonFailUIPrefab;

    public GameObject AlertUIPrefab;
    public GameObject ConfirmUIPrefab;
    public GameObject QuantityUIPrefab;

    public Canvas GlobalCanvas { get; private set; }
    public Canvas SceneCanvas { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        GlobalCanvas = GameObject.FindWithTag("GlobalUI").GetComponent<Canvas>();
        SceneCanvas = GameObject.FindWithTag("SceneUI").GetComponent<Canvas>();

        Instantiate(PlayerUIPrefab, GlobalCanvas.transform);
        Instantiate(QuickSlotUIPrefab, GlobalCanvas.transform);
        Instantiate(SkillSlotUIPrefab, GlobalCanvas.transform);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            if(_skillUIInstance == null) {
                _skillUIInstance = Instantiate(SkillUIPrefab, GlobalCanvas.transform);
            }
            else {
                _skillUIInstance.SetActive(!_skillUIInstance.activeSelf);
            }
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            if (_abilityUIInstance == null) {
                _abilityUIInstance = Instantiate(AbilityUIPrefab, GlobalCanvas.transform);
            }
            else {
                _abilityUIInstance.SetActive(!_abilityUIInstance.activeSelf);
            }
        }
        if(Input.GetKeyDown(KeyCode.I)) {
            if (_inventoryUIInstance == null) {
                _inventoryUIInstance = Instantiate(InventoryUIPrefab, GlobalCanvas.transform);
            }
            else {
                _inventoryUIInstance.SetActive(!_inventoryUIInstance.activeSelf);
            }
        }
        if(Input.GetKeyDown(KeyCode.T)) {
            InventoryManager.Instance.SetMode(InventoryManager.InventoryMode.Enhance);
            if (_testUIInstance == null) {
                _testUIInstance = Instantiate(TestUIPrefab, GlobalCanvas.transform);
            }
            else {
                _testUIInstance.SetActive(!_testUIInstance.activeSelf);
            }
        }
    }

    public void CreateDungeonClearUI(float startTime, int dungeonNum, int townNum) {
        ClearUI clearUI = Instantiate(DungeonClearUIPrefab, GlobalCanvas.transform).GetComponent<ClearUI>();
        clearUI.Init(startTime, dungeonNum, townNum);
    }

    public void CreateDungeonFailUI(float startTime, int dungeonNum, int townNum) {
        FailUI failUI = Instantiate(DungeonFailUIPrefab, GlobalCanvas.transform).GetComponent<FailUI>();
        failUI.Init(startTime, dungeonNum, townNum);
    }

    public void CreateAlertUI(string text) {
        StartCoroutine(CoAlertUI(text));
    }

    private IEnumerator CoAlertUI(string text) {
        GameObject AlertUIObject = Instantiate(AlertUIPrefab, SceneCanvas.transform);
        ConfirmUI AlertUIInstance = AlertUIObject.GetComponent<ConfirmUI>();
        AlertUIInstance.SetText(text);
        while (!AlertUIInstance.isClicked) {
            yield return null;
        }
        Destroy(AlertUIObject);
    }
    public IEnumerator CreateConfirmUI(string text, Action<bool> callback) {
        GameObject confirmUIObject = Instantiate(ConfirmUIPrefab, SceneCanvas.transform);
        ConfirmUI confirmUIInstance = confirmUIObject.GetComponent<ConfirmUI>();
        confirmUIInstance.SetText(text);

        while (!confirmUIInstance.isClicked) {
            yield return null;
        }

        callback?.Invoke(confirmUIInstance.Answer);
        Destroy(confirmUIObject);
    }

    public IEnumerator CreateQuantityUI(string text, int maxCount, Action<bool, int> callback) {
        GameObject quantityUIObject = Instantiate(QuantityUIPrefab, SceneCanvas.transform);
        QuantityUI quantityUIInstance = quantityUIObject.GetComponent<QuantityUI>();
        quantityUIInstance.SetItemCount(maxCount);
        quantityUIInstance.SetText(text);

        while (!quantityUIInstance.isClicked) {
            yield return null;
        }

        callback?.Invoke(quantityUIInstance.Answer, quantityUIInstance.ResultAmount);
        Destroy(quantityUIObject);
    }
}
