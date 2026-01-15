using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public static class SaveManager {
    // 유닛 데이터 저장 리퀘스트 생성 -> Api 클래스에서 서버로 리퀘스트 전달
    public static IEnumerator SavePlayerData(PlayerData data, Action<bool> callback = null) {
        string url = "http://127.0.0.1:8000/api/game/unit/save";
        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        // JWT 토큰 담기
        AuthManager.ApplyAuthHeader(request);

        yield return Api.Post(request, json, (code, json) => {
            switch (code) {
                case 200:
                    Debug.Log("플레이어 데이터 저장 성공");
                    callback?.Invoke(true);
                    break;
                default:
                    Debug.LogError("플레이어 데이터 저장 실패");
                    callback.Invoke(false);
                    break;
            }
        });
    }

    // 인벤토리 데이터 저장 요청 -> Api 클래스에서 서버로 리퀘스트 전달
    public static IEnumerator SaveInventoryData(InventoryItemBundle data, Action<bool> callback = null) {
        string url = "http://127.0.0.1:8000/api/game/inventory/save";
        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        // JWT 토큰 담기
        AuthManager.ApplyAuthHeader(request);
        yield return Api.Post(request, json, (code, json) => {
            switch (code) {
                case 200:
                    Debug.Log("인벤토리 데이터 저장 성공");
                    callback?.Invoke(true);
                    break;
                default:
                    Debug.LogError("인벤토리 데이터 저장 실패");
                    callback?.Invoke(false);
                    break;
            }
        });
    }

    // 스킬 데이터 저장 요청
    public static IEnumerator SaveSkillData(SkillDataBundle data, Action<bool> callback = null) {
        string url = "http://127.0.0.1:8000/api/game/skill/save";
        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        // JWT 토큰 담기
        AuthManager.ApplyAuthHeader(request);
        yield return Api.Post(request, json, (code, json) => {
            switch (code) {
                case 200:
                    Debug.Log("스킬 데이터 저장 성공");
                    callback?.Invoke(true);
                    break;
                default:
                    Debug.LogError("스킬 데이터 저장 실패");
                    callback?.Invoke(false);
                    break;
            }
        });
    }
}