using System;

public partial class GameData : Singleton<GameData>
{
    public enum LocalType
    {
        Ko, En, Ja,
    }
    
    public LocalType CurrentLocalType { get; private set; } = LocalType.Ko;
    
    public void LoadGameData()
    {
        var sheets = JsonDataLoader.LoadAllSheets();
        if (sheets.IsNullOrEmpty())
        {
            LogManager.LogWarning("[GameDataRuntimeLoader] No sheets found.");
            return;
        }

        LoadLocalString();
        
        const string EnumDataFileName = "Enum";
        const string GameSettingDataFileName = "GameSetting";
        const string LocalizationDataFileName = "Localization";

        foreach (var sheetInfo in sheets)
        {
            var isEnumData = string.Equals(sheetInfo.SheetName, EnumDataFileName, StringComparison.OrdinalIgnoreCase);
            if (isEnumData) continue;
            var isGameSettingData = string.Equals(sheetInfo.SheetName, GameSettingDataFileName, StringComparison.OrdinalIgnoreCase);
            if (isGameSettingData)
            {
                GameSetting.Instance.InvokeLoadForSheet(sheetInfo);
                continue;
            }
            var isLocalizationData = string.Equals(sheetInfo.SheetName, LocalizationDataFileName, StringComparison.OrdinalIgnoreCase);
            if (isLocalizationData)
            {
                LoadLocalization(sheetInfo);
                continue;
            }
            InvokeLoadForSheet(sheetInfo.SheetName, sheetInfo.Rows);
        }
    }
}