using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public partial class GameData
{
    private class LocalStringJson
    {
        public List<LocalString> list = new();
    }

    public class LocalString
    {
        public string ko;
        public string en;
        public string ja;
    }

    private Dictionary<string, LocalString> _localStringDictionaryCache = new();

    private void LoadLocalString()
    {
        var jsonFilePath = Path.Combine(JsonDataRepositorySetting.GetRepoPath(), "data/LocalData/LocalData.json");
        if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath))
        {
            LogManager.LogError($"JSON repo not found: {jsonFilePath}");
            return;
        }

        var jsonText = File.ReadAllText(jsonFilePath);
        _localStringDictionaryCache = JsonConvert.DeserializeObject<Dictionary<string, LocalString>>(jsonText);
    }

    private string GetLocalString(string key, LocalType type)
    {
        if (_localStringDictionaryCache.TryGetValue(key, out var value))
        {
            return type switch
            {
                LocalType.Ko => value.ko,
                LocalType.En => value.en,
                LocalType.Ja => value.ja,
                _ => string.Empty,
            };
        }
        if (string.IsNullOrEmpty(key)) return string.Empty;
        return string.Empty;
    }
}