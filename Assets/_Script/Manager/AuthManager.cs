using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

public static class AuthManager {
    public static string AccessToken;

    public static void ApplyAuthHeader(UnityWebRequest request) {
        if (!string.IsNullOrEmpty(AuthManager.AccessToken)) {
            request.SetRequestHeader(
                "authorization",
                $"Bearer {AccessToken}"
            );
        }
    }
}