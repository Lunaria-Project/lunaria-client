using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

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
#if UNITY_EDITOR
        var localDataPath = Path.Combine(JsonDataRepositorySetting.GetRepoPath(), "data/LocalData/LocalData.json");
        var localizationPath = Path.Combine(JsonDataRepositorySetting.GetRepoPath(), "data/GameData/Localization.json");
#else
        var localDataPath = Path.Combine(Application.streamingAssetsPath, "data/LocalData/LocalData.json");
        var localizationPath = Path.Combine(Application.streamingAssetsPath, "data/GameData/Localization.json");
#endif
        _localStringDictionaryCache.Clear();
        MergeLocalDataDictionary(localDataPath);
        MergeLocalizationRows(localizationPath);
    }

    private void MergeLocalDataDictionary(string jsonFilePath)
    {
        if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath))
        {
            LogManager.LogError($"JSON repo not found: {jsonFilePath}");
            return;
        }

        var jsonText = File.ReadAllText(jsonFilePath);
        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, LocalString>>(jsonText);
        if (dictionary == null) return;
        foreach (var pair in dictionary)
        {
            _localStringDictionaryCache[pair.Key] = pair.Value;
        }
    }

    private void MergeLocalizationRows(string jsonFilePath)
    {
        if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath))
        {
            LogManager.LogError($"JSON repo not found: {jsonFilePath}");
            return;
        }

        var jsonText = File.ReadAllText(jsonFilePath);
        var root = JObject.Parse(jsonText);
        var rows = (JArray)root["rows"];
        if (rows == null) return;
        foreach (var rowToken in rows)
        {
            var row = (JArray)rowToken;
            var key = (string)row[0];
            if (string.IsNullOrEmpty(key)) continue;
            _localStringDictionaryCache[key] = new LocalString
            {
                ko = (string)row[1],
                en = (string)row[2],
                ja = (string)row[3],
            };
        }
    }

    public string GetLocalString(string key)
    {
        if (_localStringDictionaryCache.TryGetValue(key, out var value))
        {
            return CurrentLocalType switch
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