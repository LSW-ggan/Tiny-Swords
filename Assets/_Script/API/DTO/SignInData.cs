using System;

[System.Serializable]
public class SignInData {
    public string email;
    public string password;
}

[System.Serializable]
public class SignInResponse {
    public string accessToken;
}