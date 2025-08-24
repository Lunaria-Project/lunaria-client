using System;
using System.Net;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class GoogleSheetManager
{
    private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string Endpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string SheetBaseUrl = "https://sheets.googleapis.com/v4/spreadsheets";

    private static string GetGoogleSheetUrl(string googleSheetId) => $"{SheetBaseUrl}/{googleSheetId}";
    private static string GetSheetValueUrl(string sheetId, string sheetName) => $"{SheetBaseUrl}/{sheetId}/values/{sheetName}";

    private static HttpListenerContext _httpListenerContext;
    private static HttpListener _httpListener;
    private static CancellationTokenSource _cts;

    public static async UniTask<GoogleSheetInfoResponse> GetSheetsAsync(GoogleSheetConfig config)
    {
        if (!config.IsAccessTokenValid)
        {
            if (config.HasRefreshToken)
            {
                await RefreshAccessTokenAsync(config);
            }
            else
            {
                Debug.Log("🌐 Token이 없음 => 브라우저 인증 대기 중...");
                var isSuccess = await StartOAuth(config);
                if (!isSuccess)
                {
                    Debug.Log("❌ 브라우저 인증 실패");
                    return null;
                }
            }
        }

        var sheetValues = await FetchSheetListAsync(config);
        return sheetValues;
    }

    private static async UniTask<GoogleSheetInfoResponse> FetchSheetListAsync(GoogleSheetConfig config)
    {
        if (string.IsNullOrEmpty(config.GoogleSheetId))
        {
            Debug.LogError("❌ Spreadsheet Id가 설정되지 않았습니다.");
            return null;
        }

        var url = GetGoogleSheetUrl(config.GoogleSheetId);
        var request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"Bearer {config.OAuthTokenCache.AccessToken}");

        await request.SendWebRequest().ToUniTask();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ 시트 목록 요청 실패: {request.error}");
            return null;
        }

        Debug.Log("✅ 시트 목록 불러오기 성공");

        var sheetInfoJson = request.downloadHandler.text;
        var sheetInfo = JsonConvert.DeserializeObject<GoogleSheetInfoResponse>(sheetInfoJson);
        return sheetInfo;
    }

    private static async UniTask<bool> RefreshAccessTokenAsync(GoogleSheetConfig config)
    {
        var form = new WWWForm();
        form.AddField("client_id", config.ClientInfo.Id);
        form.AddField("client_secret", config.ClientInfo.SecretNumber);
        form.AddField("refresh_token", config.OAuthTokenCache.RefreshToken);
        form.AddField("grant_type", "refresh_token");

        var request = UnityWebRequest.Post(TokenEndpoint, form);
        await request.SendWebRequest().ToUniTask();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Refresh 토큰 요청 실패: " + request.error);
            return false;
        }

        var token = JsonConvert.DeserializeObject<OAuthToken>(request.downloadHandler.text);
        config.SetOAuthToken(token);

        Debug.Log("🔁 AccessToken 갱신 완료");
        return true;
    }

    private static async UniTask<bool> StartOAuth(GoogleSheetConfig config)
    {
        try
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _cts.CancelAfter(TimeSpan.FromSeconds(config.CancelTimeSeconds));

            // OAuth 인증 URL 오픈
            var url = GetBuildOAuthUrl();
            Application.OpenURL(url);

            // Unity 로컬 서버(HttpListener)로 인증 코드 받아옴
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(config.RedirectUrl);
            _httpListener.Start();

            _httpListenerContext = await _httpListener.GetContextAsync().AsUniTask().AttachExternalCancellation(_cts.Token);
            var code = _httpListenerContext.Request.QueryString["code"];

            const string responseHtml = "<html><body><h2>Authentication complete. You can now return to Unity.</h2></body></html>";

            var buffer = Encoding.UTF8.GetBytes(responseHtml);
            _httpListenerContext.Response.ContentLength64 = buffer.Length;

            await _httpListenerContext.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length, _cts.Token);

            // 토큰 요청
            var form = new WWWForm();
            form.AddField("code", code);
            form.AddField("client_id", config.ClientInfo.Id);
            form.AddField("client_secret", config.ClientInfo.SecretNumber);
            form.AddField("redirect_uri", config.RedirectUrl);
            form.AddField("grant_type", "authorization_code");

            var request = UnityWebRequest.Post(TokenEndpoint, form);
            await request.SendWebRequest().ToUniTask(cancellationToken: _cts.Token);

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Token 요청 실패: " + request.error);
                return false;
            }

            var token = JsonConvert.DeserializeObject<OAuthToken>(request.downloadHandler.text);
            config.SetOAuthToken(token);

            Debug.Log("✅ Access Token 발급 성공");
            return true;
        }
        catch (OperationCanceledException)
        {
            Debug.LogError("⏱️ 인증 시간 초과");
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
        finally
        {
            _httpListenerContext?.Response.OutputStream?.Close();
            _httpListener.Stop();
        }

        string GetBuildOAuthUrl()
        {
            return new StringBuilder(Endpoint)
                .Append($"?client_id={config.ClientInfo.Id}")
                .Append($"&redirect_uri={config.RedirectUrl}")
                .Append("&response_type=code")
                .Append("&scope=https://www.googleapis.com/auth/spreadsheets.readonly")
                .Append("&access_type=offline&prompt=consent")
                .ToString();
        }
    }

    public static async UniTask<SheetValueResponse> LoadGoogleSheetsData(GoogleSheetConfig config, string sheetName)
    {
        var url = GetSheetValueUrl(config.GoogleSheetId, sheetName);
        var request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"Bearer {config.OAuthTokenCache.AccessToken}");
        await request.SendWebRequest().ToUniTask();
        return JsonConvert.DeserializeObject<SheetValueResponse>(request.downloadHandler.text);
    }
}