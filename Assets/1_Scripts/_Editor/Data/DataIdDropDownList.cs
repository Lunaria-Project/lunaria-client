using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

public static class DataIdDropDownList
{
    #region DataIdMap

    [Serializable]
    public class DataIdMap
    {
        public List<Tag> Tags;

        [Serializable]
        public class Tag
        {
            public string String;
            public int Int;
            public string SheetName;
            public string ColumnName;
        }
    }

    private static DataIdMap _dataIdMap;

    private static void LoadDataIdMap()
    {
        var jsonFilePath = Path.Combine(JsonDataRepositorySetting.GetRepoPath(), "data/_meta/id_map.json");
        if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath))
        {
            LogManager.LogError($"JSON repo not found: {jsonFilePath}");
            return;
        }

        var jsonText = File.ReadAllText(jsonFilePath);
        _dataIdMap = JsonConvert.DeserializeObject<DataIdMap>(jsonText);
    }

    #endregion
    
    private static ValueDropdownList<int> _itemDataIdCache;
    public static ValueDropdownList<int> GetItemDataIds()
    {
        return _itemDataIdCache ??= GetDropdownListFromGameData("Item", "Id");
    }
    
    private static ValueDropdownList<int> _npcDataIdCache;
    public static ValueDropdownList<int> GetNpcDataIds()
    {
        return _npcDataIdCache ??= GetDropdownListFromGameData("MapNpcInfo", "NpcId");
    }

    private static ValueDropdownList<int> GetDropdownListFromGameData(string sheetName, string columnName)
    {
        LoadDataIdMap();

        var list = new ValueDropdownList<int>();
        if (_dataIdMap == null) return list;

        foreach (var tag in _dataIdMap.Tags)
        {
            if (!string.Equals(tag.SheetName, sheetName)) continue;
            if (!string.Equals(tag.ColumnName, columnName)) continue;

            list.Add(new ValueDropdownItem<int>($"[{tag.Int}] - {tag.String})", tag.Int));
        }

        return list;
    }
}