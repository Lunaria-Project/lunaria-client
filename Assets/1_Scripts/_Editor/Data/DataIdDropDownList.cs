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
        if (_itemDataIdCache.IsNullOrEmpty())
        {
            _itemDataIdCache = GetDropdownListFromGameData("Item", "Id");
        }
        return _itemDataIdCache;
    }

    private static ValueDropdownList<int> _npcDataIdCache;
    public static ValueDropdownList<int> GetNpcDataIds()
    {
        if (_npcDataIdCache.IsNullOrEmpty())
        {
            _npcDataIdCache = GetDropdownListFromGameData("MapNpcInfo", "NpcId");
        }
        return _npcDataIdCache;
    }

    private static ValueDropdownList<int> GetDropdownListFromGameData(string sheetName, string columnName)
    {
        GameData.Instance.LoadGameData();
        LoadDataIdMap();

        var list = new ValueDropdownList<int>();
        if (_dataIdMap == null) return list;

        var sheets = JsonDataLoader.LoadAllSheets();
        foreach (var sheet in sheets)
        {
            if (!sheet.SheetName.Equals(sheetName)) continue;
            var columnIndex = -1;
            for (var i = 0; i < sheet.ColumnNames.Length; i++)
            {
                if (!sheet.ColumnNames[i].Equals(columnName)) continue;
                columnIndex = i;
                break;
            }
            if (columnIndex < 0) return list;
            foreach (var row in sheet.Rows)
            {
                var id = Convert.ToInt32(row.GetAt(columnIndex));
                var tag = GetTagString(id);
                if (string.IsNullOrEmpty(tag)) continue;
                list.Add(new ValueDropdownItem<int>($"[{tag}] - {id}", id));
            }
        }
        return list;

        string GetTagString(int id)
        {
            foreach (var tag in _dataIdMap.Tags)
            {
                if (tag.Int != id) continue;
                return tag.String;
            }
            return string.Empty;
        }
    }
}