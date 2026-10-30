using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public partial class UserData // Save
{
    private const int CurrentSaveVersion = 1;
    private const string SaveFileName = "save.json";
    private const string TempFileExtension = ".tmp";
    
    private static readonly JsonSerializerSettings SaveSerializerSettings = new()
    {
        ObjectCreationHandling = ObjectCreationHandling.Replace, // 필드 초기값이 있는 컬렉션에서 로드 데이터에 초기값을 붙이지 않도록
        Converters = { new StringEnumConverter() }, // enum은 이름으로 저장해 enum 항목 순서가 바뀌어도 세이브가 깨지지 않게
    };

    // TODO(지선): Steamworks 연동 시 계정별 분리를 위해 SteamID 하위 폴더로 변경 (예: .../<SteamID>/save.json)
    // TODO(지선): Steamworks 파트너 사이트에서 Steam Cloud Auto-Cloud 설정 (루트 WinAppDataLocalLow, 경로 <CompanyName>/<ProductName>, 패턴 save.json / Mac·Linux 루트 오버라이드)
    private static string SaveFilePath =>Path.Combine(Application.persistentDataPath, SaveFileName);

    public bool HasSaveData => File.Exists(SaveFilePath);

    public void Save()
    {
        _userDataInfo.SaveVersion = CurrentSaveVersion;
        var tempFilePath = SaveFilePath + TempFileExtension;
        try
        {
            // 저장 중 종료돼도 기존 세이브가 깨지지 않도록 임시 파일에 먼저 쓰고 교체한다.
            File.WriteAllText(tempFilePath, JsonConvert.SerializeObject(_userDataInfo, SaveSerializerSettings));
            if (File.Exists(SaveFilePath))
            {
                File.Replace(tempFilePath, SaveFilePath, null);
            }
            else
            {
                File.Move(tempFilePath, SaveFilePath);
            }
            LogManager.Log($"[Save] Save: 저장 완료 (day={CurrentDay}, path={SaveFilePath})");
        }
        catch (Exception e)
        {
            LogManager.LogError($"[Save] Save: 저장 실패 (path={SaveFilePath})");
            LogManager.LogException(e);
        }
    }

    public bool TryLoad(out UserDataInfo info)
    {
        info = null;
        if (!HasSaveData) return false;

        try
        {
            info = JsonConvert.DeserializeObject<UserDataInfo>(File.ReadAllText(SaveFilePath), SaveSerializerSettings);
        }
        catch (Exception e)
        {
            LogManager.LogError($"[Save] TryLoad: 로드 실패 (path={SaveFilePath})");
            LogManager.LogException(e);
            return false;
        }

        if (info == null)
        {
            LogManager.LogError($"[Save] TryLoad: 세이브 데이터가 비어있음 (path={SaveFilePath})");
            return false;
        }

        LogManager.Log($"[Save] TryLoad: 로드 완료 (day={info.CurrentDay}, version={info.SaveVersion})");
        return true;
    }
}
