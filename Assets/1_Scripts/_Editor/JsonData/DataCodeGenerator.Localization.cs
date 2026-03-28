#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

public static partial class DataCodeGenerator
{
    private const string LocalizationDataPath = "Assets/1_Scripts/Generated/GeneratedLocalizationData.cs";
    private const string LocalizationDataFileName = "Localization";

    public static void GenerateLocalizationCode(List<SheetInfo> sheets)
    {
        try
        {
            foreach (var sheet in sheets)
            {
                if (!string.Equals(sheet.SheetName, LocalizationDataFileName, StringComparison.OrdinalIgnoreCase)) continue;
                var dataCode = GenerateLocalizationCode(sheet.Rows);
                WriteFile(LocalizationDataPath, dataCode);
                LogManager.Log($"[GameDataCodeGenerator] Generated: {LocalizationDataPath}");
                return;
            }
        }
        catch (Exception e)
        {
            LogManager.LogException(e);
            EditorUtility.DisplayDialog("GameData Generation Error", e.Message, "OK");
        }
    }

    private static string GenerateLocalizationCode(List<object[]> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine("public struct Localization");
        sb.AppendLine("{");
        sb.AppendIndentedLine("public string Ko;", 1);
        sb.AppendIndentedLine("public string En;", 1);
        sb.AppendIndentedLine("public string Ja;", 1);
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("public partial class GameData");
        sb.AppendLine("{");
        sb.AppendIndentedLine("private readonly Dictionary<string, Localization> _localizationDictionary = new();", 1);
        sb.AppendLine();
        sb.AppendIndentedLine("public Localization GetLocalization(string key) => _localizationDictionary[key];", 1);
        sb.AppendIndentedLine("public bool TryGetLocalization(string key, out Localization value) => _localizationDictionary.TryGetValue(key, out value);", 1);
        sb.AppendLine();
        sb.AppendIndentedLine("private void LoadLocalization(SheetInfo sheetInfo)", 1);
        sb.AppendIndentedLine("{", 1);
        sb.AppendIndentedLine("foreach (var row in sheetInfo.Rows)", 2);
        sb.AppendIndentedLine("{", 2);
        sb.AppendIndentedLine("var key = (string)row[0];", 3);
        sb.AppendIndentedLine("var localization = new Localization", 3);
        sb.AppendIndentedLine("{", 3);
        sb.AppendIndentedLine("Ko = (string)row[1],", 4);
        sb.AppendIndentedLine("En = (string)row[2],", 4);
        sb.AppendIndentedLine("Ja = (string)row[3],", 4);
        sb.AppendIndentedLine("};", 3);
        sb.AppendIndentedLine("_localizationDictionary[key] = localization;", 3);
        sb.AppendIndentedLine("}", 2);
        sb.AppendIndentedLine("}", 1);
        sb.AppendLine("}");
        return sb.ToString();
    }
}
#endif
