using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Bootstrap : MonoBehaviour {
    public GameObject NotificationUIPrefab;

    public Image ProgressBar;
    public TMP_Text ProgressText;

    public bool IsCSVDataReady { get; private set; }
    public bool IsPlayerDataReady { get; private set; }
    public bool IsInventoryDataReady { get; private set; }
    public bool IsSkillDataReady { get; private set; }
    public float Progress { get; private set; }

    private void Awake() {
        Progress = 0f;
        IsCSVDataReady = false;
        IsPlayerDataReady = false;
        IsInventoryDataReady = false;
        IsSkillDataReady = false;

        LoadCSVData();
        Progress = 0.2f;
    }

    private void Start() {
        StartCoroutine(LoadGameData());
    }

    private void Update() {
        ProgressBar.fillAmount = Progress;
    }

    // CSV 파일에 저장된 데이터 로딩
    private void LoadCSVData() {
        try {
            ProgressText.SetText("CSV 파일 로드 중...");
            // 강화 확률 테이블 로드
            string enhanceSuccessRateCSV = CSVLoader.Load("CSV/Enhance/ENHANCE_SUCCESS_RATE.csv");
            EnhanceRateTable.LoadFromCSV(enhanceSuccessRateCSV);

            // 강화 스탯 테이블 로드
            string enhanceWeaponStatCSV = CSVLoader.Load("CSV/Enhance/ENHANCE_WEAPON_INCREASE.csv");
            string enhanceArmorStatCSV = CSVLoader.Load("CSV/Enhance/ENHANCE_ARMOR_INCREASE.csv");
            string enhanceAccessoriesStatCSV = CSVLoader.Load("CSV/Enhance/ENHANCE_ACCESSORIES_INCREASE.csv");

            EnhanceWeaponTable.LoadFromCSV(enhanceWeaponStatCSV);
            EnhanceArmorTable.LoadFromCSV(enhanceArmorStatCSV);
            EnhanceAccessoriesTable.LoadFromCSV(enhanceAccessoriesStatCSV);
            Debug.Log("CSV 데이터 로딩 성공");
        } 
        catch(Exception err) {
            Debug.LogError(err);
            StartCoroutine(LoadingError("CSV 파일 로드 에러"));
        }
    }

    // 서버로부터 게임 데이터 요청
    private IEnumerator LoadGameData() {
        yield return new WaitForSeconds(0.5f);

        ProgressText.SetText("플레이어 데이터 로드 중...");
        yield return StartCoroutine(LoadPlayerData());
        Progress = 0.4f;
        if(!IsPlayerDataReady) {
            StartCoroutine(LoadingError("플레이어 데이터를 불러올 수 없습니다."));
            yield break;
        }
        Debug.Log("플레이어 데이터 로딩 성공");

        yield return new WaitForSeconds(0.5f);

        ProgressText.SetText("인벤토리 데이터 로드 중...");
        yield return StartCoroutine(LoadInventoryData());
        Progress = 0.6f;
        if (!IsInventoryDataReady) {
            StartCoroutine(LoadingError("인벤토리 데이터를 불러올 수 없습니다."));
            yield break;
        }
        Debug.Log("인벤토리 데이터 로딩 성공");

        yield return new WaitForSeconds(0.5f);

        ProgressText.SetText("스킬 데이터 로드 중...");
        yield return StartCoroutine(LoadSkillData());
        Progress = 0.8f;
        if (!IsSkillDataReady) {
            StartCoroutine(LoadingError("스킬 데이터를 불러올 수 없습니다."));
            yield break;
        }
        Debug.Log("스킬 데이터 로딩 성공");

        yield return new WaitForSeconds(0.5f);

        ProgressText.SetText("로딩 성공!");
        Progress = 1.0f;
        yield return new WaitForSeconds(1.0f);
        Debug.Log("모든 데이터 로딩 완료");
        SceneManager.LoadScene((int)Scenes.BuildNumber.Town);
    }

    private IEnumerator LoadingError(string text) {
        ConfirmUI _notification = Instantiate(NotificationUIPrefab, GameObject.FindWithTag("Canvas").transform).GetComponent<ConfirmUI>();
        _notification.SetText(text);
        while (!_notification.isClicked) {
            yield return null;
        }
        SceneManager.LoadScene((int)Scenes.BuildNumber.Main);
        Destroy(gameObject);
    }

    private IEnumerator LoadPlayerData() {
        string url = "http://127.0.0.1:8000/api/game/enter";

        UnityWebRequest request = UnityWebRequest.Get(url);
        AuthManager.ApplyAuthHeader(request);

        yield return StartCoroutine(Api.Get(request, (code, json) => {
            switch (code) {
                case 200:
                    // 전달 받은 DTO를 유저 정보 관리 매니저로 넘겨 초기화
                    var data = JsonUtility.FromJson<PlayerData>(json);
                    PlayerDataManager.Instance.InitializeFromServer(data);
                    IsPlayerDataReady = true;
                    break;
                default:
                    IsPlayerDataReady = false;
                    break;
            }
        }));
    }

    private IEnumerator LoadInventoryData() {
        string url = "http://127.0.0.1:8000/api/game/inventory";

        UnityWebRequest request = UnityWebRequest.Get(url);
        AuthManager.ApplyAuthHeader(request);

        yield return StartCoroutine(Api.Get(request, (code, json) => {
            switch (code) {
                case 200:
                    // 전달 받은 DTO를 인벤토리 정보 관리 매니저로 넘겨 초기화
                    var data = JsonUtility.FromJson<InventoryItemBundle>(json);
                    InventoryManager.Instance.InitializeFromServer(data.inventory);
                    IsInventoryDataReady = true;
                    break;
                default:
                    IsInventoryDataReady = false;
                    break;
            }
        }));
    }

    private IEnumerator LoadSkillData() {
        string url = "http://127.0.0.1:8000/api/game/skill";

        UnityWebRequest request = UnityWebRequest.Get(url);
        AuthManager.ApplyAuthHeader(request);

        yield return StartCoroutine(Api.Get(request, (code, json) => {
            switch (code) {
                case 200:
                    // 전달 받은 DTO를 스킬 정보 관리 매니저로 넘겨 초기화
                    var data = JsonUtility.FromJson<SkillDataBundle>(json);
                    SkillManager.Instance.InitializeFromServer(data);
                    IsSkillDataReady = true;
                    break;
                default:
                    IsSkillDataReady = false;
                    break;
            }
        }));
    }
}