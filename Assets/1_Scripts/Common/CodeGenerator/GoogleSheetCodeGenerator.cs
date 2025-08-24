using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;

public static class GoogleSheetCodeGenerator
{
    public const string GeneratedGameDataNameSpace = "Generated";
    public static string EnumDataFilePath => $"{GeneratedCodeFilePath}/GeneratedEnumData.cs";
    public static string GameDataFilePath => $"{GeneratedCodeFilePath}/GeneratedGameData.cs";
    private static string GeneratedCodeFilePath => $"{Application.dataPath}/1_Scripts/GeneratedGameData";

    private static Encoding UTF8NoBom => new UTF8Encoding(false);

    public const string TypeEnum = "enum";
    private const string TypeInt = "int";
    private const string TypeString = "string";
    private const string TypeListIne = "list<int>";
    private const string TypeFloat = "float";
    private const string TypeVector2 = "Vector2";

    public static readonly Dictionary<string, string> StringToTypes = new()
    {
        { TypeEnum, "enum" },
        { TypeInt, "int" },
        { TypeString, "string" },
        { TypeListIne, "List<int>" },
        { TypeFloat, "float" },
        { TypeVector2, "Vector2" },
    };

    private static readonly Dictionary<string, string> StringToParse = new()
    {
        { TypeEnum, "Enum.Parse<@>" },
        { TypeInt, "int.Parse" },
        { TypeString, "" },
        { TypeListIne, "List<int>" },
        { TypeFloat, "float.Parse" },
        { TypeVector2, "float.Parse" },
    };

    private const int _attributeRowIndex = 0;
    private const int _dataTypeRowIndex = 1;
    private const int _nameRowIndex = 2;

    public static async UniTask GenerateAll(GoogleSheetConfig config, GoogleSheetInfoResponse sheets)
    {
        if (sheets?.Sheets.IsNullOrEmpty() ?? true)
        {
            Debug.LogWarning("⚠️ 시트 정보가 없습니다.");
            return;
        }

        var dataSheetInfoList = new List<SheetInfo>();
        foreach (var sheet in sheets.Sheets)
        {
            var sheetValues = await GoogleSheetManager.LoadGoogleSheetsData(config, sheet.Properties.SheetName);
            var sheetInfo = GetDataForGenerate(sheet.Properties.SheetName, sheetValues);
            dataSheetInfoList.Add(sheetInfo);
        }

        await EnumDataCodeGenerator.Generate(dataSheetInfoList);
        await GameDataCodeGenerator.Generate(dataSheetInfoList);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("Complete Generate GameData");
#endif
    }

    public static async UniTask WriteGameDataToFileAsync(string filePath, StringBuilder contents)
    {
        var directoryName = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        await File.WriteAllTextAsync(filePath, contents.ToString(), UTF8NoBom);
    }

    private static SheetInfo GetDataForGenerate(string sheetName, SheetValueResponse sheetValue)
    {
        if (sheetValue.Values.Count <= _nameRowIndex)
        {
            Debug.LogWarning($"⚠️ 시트 {sheetName}의 데이터가 부족합니다 ({_nameRowIndex + 1}줄 이상 필요)");
            return default;
        }

        var columnTypes = sheetValue.Values[_dataTypeRowIndex].ToArray();
        var columnNames = sheetValue.Values[_nameRowIndex].ToArray();

        var enumDic = new Dictionary<int, List<string>>();
        for (var j = 0; j < columnTypes.Length; j++)
        {
            if (columnTypes[j].ToLower() != TypeEnum) continue;

            enumDic.Add(j, new List<string>());
        }

        for (var j = 2; j < sheetValue.Values.Count; j++)
        {
            var columns = sheetValue.Values[j].ToArray();
            for (var k = 0; k < columns.Length; k++)
            {
                if (!enumDic.TryGetValue(k, out var enumList)) continue;
                if (enumList.Contains(columns[k])) continue;

                enumDic[k].Add(columns[k]);
            }
        }

        return new SheetInfo
        {
            SheetName = sheetName,
            ColumnNames = columnNames,
            ColumnTypes = columnTypes,
            EnumList = enumDic,
        };
    }

    public static StringBuilder MakeParameters(IReadOnlyList<string> types, IReadOnlyList<string> names)
    {
        var paramList = new StringBuilder();
        for (var i = 0; i < names.Count; i++)
        {
            var columnName = names[i];
            if (types[i].ToLower() == TypeEnum)
            {
                paramList.Append($"{columnName} {columnName.FirstCharacterToLower()}");
            }
            else
            {
                var columnType = StringToTypes[types[i]];
                paramList.Append($"{columnType} {columnName.FirstCharacterToLower()}");
            }


            if (i != names.Count - 1)
            {
                paramList.Append(", ");
            }
        }

        return paramList;
    }
}