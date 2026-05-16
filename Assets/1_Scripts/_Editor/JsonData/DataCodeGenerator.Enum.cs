#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static partial class DataCodeGenerator
{
    private const string EnumDataPath = "Assets/1_Scripts/Generated/GeneratedEnumData.cs";
    private const string EnumExtensionPath = "Assets/1_Scripts/Generated/GeneratedEnumData.Extension.cs";
    private const string EnumDataFileName = "Enum";

    public static void GenerateEnumDataCode(List<SheetInfo> sheets)
    {
        try
        {
            var (enumCode, extensionCode) = GenerateEnumCode(sheets);
            WriteFile(EnumDataPath, enumCode);
            WriteFile(EnumExtensionPath, extensionCode);

            LogManager.Log($"[GameDataCodeGenerator] Generated: {EnumDataPath}");
            LogManager.Log($"[GameDataCodeGenerator] Generated: {EnumExtensionPath}");
        }
        catch (Exception e)
        {
            LogManager.LogException(e);
            EditorUtility.DisplayDialog("GameData Generation Error", e.Message, "OK");
        }
    }

    private static (string enumCode, string extensionCode) GenerateEnumCode(List<SheetInfo> sheets)
    {
        var enumSheets = sheets
            .Where(s => string.Equals(s.FileName, "EnumData", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var byEnum = new Dictionary<string, List<(string Name, string DisplayName, string ResourceKey)>>(StringComparer.Ordinal);

        foreach (var sheet in enumSheets)
        {
            // 필수/선택 컬럼 인덱스
            var idxEnumName = Array.FindIndex(sheet.ColumnNames, c => string.Equals(c, "EnumName", StringComparison.OrdinalIgnoreCase));
            var idxValue = Array.FindIndex(sheet.ColumnNames, c => string.Equals(c, "Value", StringComparison.OrdinalIgnoreCase));
            var idxDisp = Array.FindIndex(sheet.ColumnNames, c => string.Equals(c, "DisplayName", StringComparison.OrdinalIgnoreCase));
            var idxResKey = Array.FindIndex(sheet.ColumnNames, c => string.Equals(c, "ResourceKey", StringComparison.OrdinalIgnoreCase));

            if (idxEnumName < 0 || idxValue < 0)
            {
                LogManager.LogWarning($"[EnumGen] Sheet '{sheet.SheetName}' is missing EnumName/Value columns. Skipped.");
                continue;
            }

            foreach (var row in sheet.Rows)
            {
                if (row == null) continue;
                if (idxEnumName >= row.Length || idxValue >= row.Length) continue;

                var enumName = row[idxEnumName]?.ToString();
                var value = row[idxValue]?.ToString();
                if (string.IsNullOrWhiteSpace(enumName) || string.IsNullOrWhiteSpace(value)) continue;

                var disp = (idxDisp >= 0 && idxDisp < row.Length) ? row[idxDisp]?.ToString() : null;
                var rkey = (idxResKey >= 0 && idxResKey < row.Length) ? row[idxResKey]?.ToString() : null;

                if (!byEnum.TryGetValue(enumName, out var list)) byEnum[enumName] = list = new List<(string, string, string)>();

                list.Add((value.Trim(), string.IsNullOrWhiteSpace(disp) ? null : disp.Trim(),
                    string.IsNullOrWhiteSpace(rkey) ? null : rkey.Trim()));
            }
        }

        var enumSb = new StringBuilder();
        var extensionMethodsSb = new StringBuilder();
        foreach (var (enumName, itemsRaw) in byEnum.OrderBy(k => k.Key, StringComparer.Ordinal))
        {
            var items = itemsRaw
                .GroupBy(x => x.Name, StringComparer.Ordinal)
                .Select(g =>
                (
                    Name: g.Key,
                    DisplayName: g.Select(x => x.DisplayName).FirstOrDefault(v => !string.IsNullOrEmpty(v)),
                    ResourceKey: g.Select(x => x.ResourceKey).FirstOrDefault(v => !string.IsNullOrEmpty(v))
                ))
                .ToList();

            enumSb.AppendIndentedLine($"[SerializeEnum]", 0);
            enumSb.AppendIndentedLine($"public enum {enumName}", 0);
            enumSb.AppendIndentedLine("{", 0);
            enumSb.AppendIndentedLine("None = 0,", 1);
            var next = 1;
            foreach (var it in items)
            {
                enumSb.AppendIndentedLine($"{it.Name} = {next},", 1);
                next++;
            }

            enumSb.AppendIndentedLine("}", 0);
            enumSb.AppendLine();

            // DisplayName 값이 실제로 하나라도 채워져 있어야 true
            var hasDisplay = items.Any(i => !string.IsNullOrWhiteSpace(i.DisplayName));
            // ResourceKey 값이 실제로 하나라도 채워져 있어야 true
            var hasResKey = items.Any(i => !string.IsNullOrWhiteSpace(i.ResourceKey));

            if (!hasDisplay && !hasResKey) continue;

            var maxLeftLen = items.Max(it => $"{enumName}.{it.Name}".Length);

            if (hasDisplay)
            {
                extensionMethodsSb.AppendIndentedLine($"public static string GetDisplayName(this {enumName} value)", 1);
                extensionMethodsSb.AppendIndentedLine("{", 1);
                extensionMethodsSb.AppendIndentedLine("var key = value switch", 2);
                extensionMethodsSb.AppendIndentedLine("{", 2);
                foreach (var it in items)
                {
                    var text = it.DisplayName ?? it.Name;
                    var left = $"{enumName}.{it.Name}".PadRight(maxLeftLen);
                    extensionMethodsSb.AppendIndentedLine($"{left} => \"{Escape(text)}\",", 3);
                }

                extensionMethodsSb.AppendIndentedLine($"{"_".PadRight(maxLeftLen)} => value.ToString(),", 3);
                extensionMethodsSb.AppendIndentedLine("};", 2);
                extensionMethodsSb.AppendIndentedLine("return GameData.Instance.GetLocalString(key);", 2);
                extensionMethodsSb.AppendIndentedLine("}", 1);
                extensionMethodsSb.AppendLine();
            }

            if (hasResKey)
            {
                extensionMethodsSb.AppendIndentedLine($"public static string GetResourceKey(this {enumName} value)", 1);
                extensionMethodsSb.AppendIndentedLine("{", 1);
                extensionMethodsSb.AppendIndentedLine("return value switch", 2);
                extensionMethodsSb.AppendIndentedLine("{", 2);
                foreach (var it in items)
                {
                    var text = it.ResourceKey ?? string.Empty;
                    var left = $"{enumName}.{it.Name}".PadRight(maxLeftLen);
                    extensionMethodsSb.AppendIndentedLine($"{left} => \"{Escape(text)}\",", 3);
                }

                extensionMethodsSb.AppendIndentedLine($"{"_".PadRight(maxLeftLen)} => string.Empty,", 3);
                extensionMethodsSb.AppendIndentedLine("};", 2);
                extensionMethodsSb.AppendIndentedLine("}", 1);
                extensionMethodsSb.AppendLine();
            }
        }

        var extensionSb = new StringBuilder();
        if (extensionMethodsSb.Length > 0)
        {
            extensionSb.AppendIndentedLine("public static class GeneratedEnumExtensions", 0);
            extensionSb.AppendIndentedLine("{", 0);
            extensionSb.Append(extensionMethodsSb);
            extensionSb.AppendIndentedLine("}", 0);
        }

        return (enumSb.ToString(), extensionSb.ToString());

        // 문자열에 \, "가 들어있으면 안 됨
        static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
#endif