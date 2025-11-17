using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DamageTextManager : MonoBehaviour {
    public static DamageTextManager s_Instance = null;
    public static DamageTextManager Instance {
        get {
            if (s_Instance == null) {
                return null;
            }
            return s_Instance;
        }
    }

    public Camera MainCamera;
    public Canvas Canvas;
    public GameObject DamageText;

    private void Awake() {
        if (s_Instance == null) {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void Update() {
        if (MainCamera == null) MainCamera = Camera.main;
        if (Canvas == null) Canvas = FindObjectOfType<Canvas>();
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene) {
        MainCamera = Camera.main;
        Canvas = FindObjectOfType<Canvas>();
    }

    public void CreateDamageText(Vector3 worldPosition, string damage) {
        Vector3 screenPos = MainCamera.WorldToScreenPoint(worldPosition);

        GameObject obj = Instantiate(DamageText, Canvas.transform);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.position = screenPos;

        DamageText damageText = obj.GetComponent<DamageText>();
        damageText.SetText(damage);
    }
}
