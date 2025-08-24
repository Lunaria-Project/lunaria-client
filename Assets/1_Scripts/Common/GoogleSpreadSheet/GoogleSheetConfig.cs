using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "SpreadSheetConfig")]
public class GoogleSheetConfig : ScriptableObject
{
    [SerializeField] private string _clientId;
    [SerializeField] private string _clientSecretNumber;
    [SerializeField] private string _googleSheetId;
    [SerializeField] private int _portNumber = 8080;
    [SerializeField] private float _cancelTimeSeconds = 30f;
    [SerializeField, HideInInspector] private OAuthToken _oAuthToken;

    public const string FilePath = "Assets/1_Scripts/Common/GoogleSpreadSheet/GoogleSheetConfig.asset";
    public OAuthToken OAuthTokenCache => _oAuthToken;

    public (string Id, string SecretNumber) ClientInfo => (_clientId, _clientSecretNumber);
    public string GoogleSheetId => _googleSheetId;
    public string RedirectUrl => $"http://localhost:{_portNumber.ToString()}/";
    public bool IsAccessTokenValid => OAuthTokenCache != null && !string.IsNullOrEmpty(OAuthTokenCache.AccessToken) && OAuthTokenCache.NextRefreshTime > DateTime.Now.Ticks;
    public bool HasRefreshToken => OAuthTokenCache != null && !string.IsNullOrEmpty(OAuthTokenCache.RefreshToken);
    public float CancelTimeSeconds => _cancelTimeSeconds;

    public void SetOAuthToken(OAuthToken token)
    {
        _oAuthToken = token;
        token.NextRefreshTime = DateTime.Now.AddSeconds(token.ExpiresIn).Ticks;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}