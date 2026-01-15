using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class Api {
    // POST 요청
    public static IEnumerator Post(UnityWebRequest request, string jsonData, Action<long, string> callback) {
        byte[] body = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        callback?.Invoke(
            request.responseCode,
            request.downloadHandler.text
        );
    }

    // GET 요청
    public static IEnumerator Get(UnityWebRequest request, Action<long, string> callback) {
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        callback?.Invoke(
            request.responseCode,
            request.downloadHandler.text
        );
    }
}
