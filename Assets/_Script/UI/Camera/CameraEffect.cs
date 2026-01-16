using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraEffect : MonoBehaviour {
    public static CameraEffect Instance;

    public CameraFollow MainCamera;                 // 메인 카메라 (가상 카메라 Brain)
    private CinemachineImpulseSource ImpulseSource; // 카메라 흔들림 구현
    public CinemachineVirtualCamera BossCamera;     // 보스 연출용
    public GameObject _loadingPanel;
    private Image _panelImage;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        ImpulseSource = gameObject.GetComponent<CinemachineImpulseSource>();
    }

    // 씬 로딩 시
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        _loadingPanel = GameObject.FindWithTag("FadePanel");

        // 메인 카메라 찾아서 참조
        GameObject MainCameraObject = GameObject.FindWithTag("MainCamera");
        MainCamera = MainCameraObject.GetComponent<CameraFollow>();

        // 보스 카메라 찾아서 참조 (스테이지 아닌 곳에서는 없을 수도 있음 NULL)
        GameObject bossCamObj = GameObject.FindWithTag("BossCamera");
        if (bossCamObj != null) {
            BossCamera = bossCamObj.GetComponent<CinemachineVirtualCamera>();
        }

        // 새로운 씬 시작 시 페이드 인
        FadeIn();
    }

    private void OnDestroy() {
        if (Instance == this) Instance = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopAllCoroutines();
    }

    public void FadeOut() {
        StartCoroutine(CoFadeOut());
    }

    public void FadeIn() {
        StartCoroutine(CoFadeIn());
    }

    public IEnumerator CoFadeOut() {
        _panelImage = _loadingPanel.GetComponent<Image>();
        Vector4 color = new Vector4(0, 0, 0, 0.0f);
        _panelImage.color = color;

        while (color.w < 1.0f) {
            yield return new WaitForSeconds(0.02f);
            color.w += 0.05f;
            _panelImage.color = color;
        }
    }

    public IEnumerator CoFadeIn() {
        _panelImage = _loadingPanel.GetComponent<Image>();
        Vector4 color = new Vector4(0, 0, 0, 1.0f);
        _panelImage.color = color;

        while (color.w > 0.0f) {
            yield return new WaitForSeconds(0.02f);
            color.w -= 0.05f;
            _panelImage.color = color;
        }
    }

    // 보스 사망시 카메라 연출
    public void PlayBossDeathEffect(Transform boss, CinemachineVirtualCamera bossCam) {
        StartCoroutine(CoBossDeathEffect(boss, bossCam));
    }

    private IEnumerator CoBossDeathEffect(Transform boss, CinemachineVirtualCamera bossCam) {
        CinemachineVirtualCamera prevCamera = MainCamera.PlayerCamera;

        if (bossCam == null) yield break;

        bossCam.Follow = boss;
        bossCam.LookAt = boss;

        // 카메라 전환
        MainCamera.SwapVirtualCamera(bossCam);

        // 슬로우
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(3.5f);

        // 원래 플레이어 카메라 복귀
        MainCamera.SwapVirtualCamera(prevCamera);

        // 슬로우 해제
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    public void PlayBossIntroCutscene(CinemachineVirtualCamera introCam, Transform boss, string bossName) {
        StartCoroutine(CoBossIntro(introCam, boss, bossName));
    }

    private IEnumerator CoBossIntro(CinemachineVirtualCamera introCam, Transform boss, string bossName) {
        CinemachineVirtualCamera prevCamera = MainCamera.PlayerCamera;

        // 입력 정지
        PlayerController controller = PlayerDataManager.Instance.Player.GetComponent<PlayerController>();
        controller.SetInput(false);

        // 카메라 전환
        MainCamera.SwapVirtualCamera(introCam, false);

        // 카메라 블렌드 시간만큼 대기
        yield return new WaitForSecondsRealtime(0.8f);
        
        // 보스 이름 UI 등장
        BossIntroUI.Instance.Show(bossName);

        yield return new WaitForSecondsRealtime(1.5f);

        // UI 숨김
        BossIntroUI.Instance.Hide();

        // 플레이어 카메라 복귀
        MainCamera.SwapVirtualCamera(prevCamera);

        // 입력 재개
        controller.SetInput(true);
    }

    // 화면 흔들림 연출
    public void Shake(float value = 1f) {
        if (ImpulseSource == null) return;
        ImpulseSource.GenerateImpulse(Vector3.up * value);
    }
}
