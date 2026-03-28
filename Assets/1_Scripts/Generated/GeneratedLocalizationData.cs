using System.Collections.Generic;

public struct Localization
{
    public string Ko;
    public string En;
    public string Ja;
}

public partial class GameData
{
    private readonly Dictionary<string, Localization> _localizationDictionary = new();

    public Localization GetLocalization(string key) => _localizationDictionary[key];
    public bool TryGetLocalization(string key, out Localization value) => _localizationDictionary.TryGetValue(key, out value);

    private void LoadLocalization(SheetInfo sheetInfo)
    {
        foreach (var row in sheetInfo.Rows)
        {
            var key = (string)row[0];
            var localization = new Localization
            {
                Ko = (string)row[1],
                En = (string)row[2],
                Ja = (string)row[3],
            };
            _localizationDictionary[key] = localization;
        }
    }
}

public static class LocalizationKey
{
    public static readonly LocalKey GameTime_Am = new("GameTime.Am");
    public static readonly LocalKey GameTime_Pm = new("GameTime.Pm");
    public static readonly LocalKey Minigame_Slime_Score = new("Minigame.Slime.Score");
    public static readonly LocalKey FamiliarSelectPopup_BottomGuide = new("FamiliarSelectPopup.BottomGuide");
    public static readonly LocalKey SlimeMinigameResultPopup_Description = new("SlimeMinigameResultPopup.Description");
}
