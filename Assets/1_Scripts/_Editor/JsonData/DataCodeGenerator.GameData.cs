#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

public static partial class DataCodeGenerator
{
    private enum ColumnType
    {
        None,
        Normal,
        Enum,
        ListInt,
        ListString,
        Vector2,
    }

    private const string OutputNamespace = "Generated";
    private const string GameDataPath = "Assets/1_Scripts/Generated/GeneratedGameData.cs";
    private const string GameGetterDataPath = "Assets/1_Scripts/Generated/GameData.GeneratedClass.cs";
    private const string DataLoaderPath = "Assets/1_Scripts/Generated/GameData.GeneratedLoader.cs";
    private const string KeyColumn = ";key";
    private const string IdColumn = ";id";
    private const string DesignColumnType = "design";

    private static readonly Dictionary<string, string> TypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { _intType, _intType },
        { _floatType, _floatType },
        { _doubleType, _doubleType },
        { _longType, _longType },
        { _boolType, _boolType },
        { _stringType, _stringType },
        { _vector2Type, "Vector2" }
        // "enum", "list" 는 별도 처리
    };

    private const string _intType = "int";
    private const string _floatType = "float";
    private const string _doubleType = "double";
    private const string _longType = "long";
    private const string _boolType = "bool";
    private const string _stringType = "string";
    private const string _vector2Type = "vector2";
    private const string _enumType = "enum";

    public static void GenerateGameDataCode(List<SheetInfo> sheets)
    {
        try
        {
            var dataCode = GenerateDataCode(sheets);
            WriteFile(GameDataPath, dataCode);

            LogManager.Log($"[GameDataCodeGenerator] Generated: {GameDataPath}");

            var dataGetterCode = GenerateDataGetterCode(sheets);
            WriteFile(GameGetterDataPath, dataGetterCode);

            var dataLoaderCode = GenerateDataLoaderCode(sheets);
            WriteFile(DataLoaderPath, dataLoaderCode);

            LogManager.Log($"[GameDataCodeGenerator] Generated: {GameGetterDataPath}");
        }
        catch (Exception e)
        {
            LogManager.LogException(e);
            EditorUtility.DisplayDialog("GameData Generation Error", e.Message, "OK");
        }
    }

    private static string GenerateDataCode(List<SheetInfo> sheets)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine($"namespace {OutputNamespace}");
        sb.AppendLine("{");

        for (var i = 0; i < sheets.Count; i++)
        {
            var sheet = sheets[i];

            var className = sheet.SheetName;
            if (!CanGenerateCode(className)) continue;

            sb.AppendIndentedLine($"public partial class {className}", 1);
            sb.AppendIndentedLine("{", 1);

            for (var j = 0; j < sheet.ColumnNames.Length; j++)
            {
                if (!TryGetCsType(sheet.ColumnTypes[j], sheet.ColumnNames[j], out var csType, out _)) continue;
                sb.AppendIndentedLine($"public {csType} {sheet.ColumnNames[j]} {{ get; private set; }}", 2);
            }

            sb.AppendLine();

            var paramList = MakeConstructorParams(sheet);
            sb.AppendIndentedLine($"public {className}({paramList})", 2);
            sb.AppendIndentedLine("{", 2);
            foreach (var raw in sheet.ColumnNames)
            {
                if (!TryGetColumnType(sheet.ColumnTypes[Array.IndexOf(sheet.ColumnNames, raw)], out _)) continue;
                sb.AppendIndentedLine($"{raw} = {ToCamelCase(raw)};", 3);
            }

            sb.AppendIndentedLine("}", 2);
            sb.AppendIndentedLine("}", 1);

            if (i != sheets.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GenerateDataGetterCode(List<SheetInfo> sheets)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Generated;");
        sb.AppendLine();
        sb.AppendLine("public partial class GameData");
        sb.AppendLine("{");

        for (var i = 0; i < sheets.Count; i++)
        {
            var sheet = sheets[i];
            var className = sheet.SheetName;
            if (!CanGenerateCode(className)) continue;

            var keyIndex = FindKeyIndex(sheet);
            var (HasKeyColumn, KeyColumnName) = (keyIndex >= 0, keyIndex >= 0 ? sheet.ColumnNames[keyIndex] : string.Empty);
            if (HasKeyColumn)
            {
                if (!TryGetCsType(sheet.ColumnTypes[keyIndex], sheet.ColumnNames[keyIndex], out var csType, out _)) continue;
                sb.AppendIndentedLine($"// {sheet.SheetName} - {className}, key: {KeyColumnName}", 1);
                sb.AppendIndentedLine($"public IReadOnlyDictionary<{csType}, {className}> DT{className} => _dt{className};", 1);
                sb.AppendIndentedLine($"public bool TryGet{className}({csType} key, out {className} result) => DT{className}.TryGetValue(key, out result);", 1);
                sb.AppendIndentedLine($"public bool Contains{className}({csType} key) => DT{className}.ContainsKey(key);", 1);
                sb.AppendIndentedLine($"private readonly Dictionary<{csType}, {className}> _dt{className} = new();", 1);
            }
            else
            {
                sb.AppendIndentedLine($"// {sheet.SheetName} - {className}", 1);
                sb.AppendIndentedLine($"public IReadOnlyList<{className}> DT{className} => _dt{className};", 1);
                sb.AppendIndentedLine($"private List<{className}> _dt{className} = new();", 1);
            }

            if (i != sheets.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GenerateDataLoaderCode(List<SheetInfo> sheets)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Generated;");
        sb.AppendLine();
        sb.AppendLine("public partial class GameData");
        sb.AppendLine("{");

        foreach (var sheet in sheets)
        {
            var className = sheet.SheetName;
            var fieldName = "_dt" + className;

            if (!CanGenerateCode(className)) continue;

            var keyIndex = FindKeyIndex(sheet);
            var (HasKeyColumn, KeyColumnName) = (keyIndex >= 0, keyIndex >= 0 ? sheet.ColumnNames[keyIndex] : string.Empty);

            var args = new List<string>(sheet.ColumnNames.Length);
            for (var i = 0; i < sheet.ColumnNames.Length; i++)
            {
                if (!TryGetCsType(sheet.ColumnTypes[i], sheet.ColumnNames[i], out var csType, out var type)) continue;
                switch (type)
                {
                    case ColumnType.Enum:
                    {
                        var columnName = sheet.ColumnNames[i];
                        args.Add($"({columnName})Enum.Parse(typeof({columnName}), (string)row[{i}], true)");
                        break;
                    }
                    case ColumnType.ListInt:
                    {
                        args.Add($"(row[{i}] as string).ParseIntList()");
                        break;
                    }
                    case ColumnType.ListString:
                    {
                        args.Add($"(row[{i}] as string).ParseStringList()");
                        break;
                    }
                    case ColumnType.Vector2:
                    {
                        args.Add($"(row[{i}] as string).ParseVector2()");
                        break;
                    }
                    default:
                    {
                        var arg = CastExpr(csType, $"row[{i}]");
                        args.Add(arg);
                        break;
                    }
                }
            }

            sb.AppendIndentedLine($"private void Load{className}(List<object[]> rows)", 1);
            sb.AppendIndentedLine("{", 1);
            sb.AppendIndentedLine("if (rows.IsNullOrEmpty()) return;", 2);
            sb.AppendIndentedLine("foreach (var row in rows)", 2);
            sb.AppendIndentedLine("{", 2);

            sb.AppendIndentedLine($"var newData = new {className}({string.Join(", ", args)});", 3);

            if (HasKeyColumn)
            {
                sb.AppendIndentedLine($"{fieldName}.Add(newData.{KeyColumnName}, newData);", 3);
            }
            else
            {
                sb.AppendIndentedLine($"{fieldName}.Add(newData);", 3);
            }

            sb.AppendIndentedLine("}", 2);
            sb.AppendIndentedLine("}", 1);
            sb.AppendLine();
        }

        sb.AppendIndentedLine("private void InvokeLoadForSheet(string sheetName, List<object[]> rows)", 1);
        sb.AppendIndentedLine("{", 1);
        sb.AppendIndentedLine("switch (sheetName)", 2);
        sb.AppendIndentedLine("{", 2);
        foreach (var sheet in sheets.Select(s => s.SheetName).Distinct())
        {
            if (!CanGenerateCode(sheet)) continue;
            sb.AppendIndentedLine($"case \"{sheet}\": Load{sheet}(rows); break;", 3);
        }

        sb.AppendIndentedLine("}", 2);
        sb.AppendIndentedLine("}", 1);

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string MakeConstructorParams(SheetInfo sheet)
    {
        var parts = new List<string>();
        for (var i = 0; i < sheet.ColumnNames.Length; i++)
        {
            if (!TryGetCsType(sheet.ColumnTypes[i], sheet.ColumnNames[i], out var csType, out _)) continue;

            parts.Add($"{csType} {ToCamelCase(sheet.ColumnNames[i])}");
        }

        return string.Join(", ", parts);
    }

    private static bool CanGenerateCode(string sheetName)
    {
        var isEnumData = string.Equals(sheetName, EnumDataFileName, StringComparison.OrdinalIgnoreCase);
        var isGameSettingData = string.Equals(sheetName, GameSettingDataFileName, StringComparison.OrdinalIgnoreCase);
        return !isEnumData && !isGameSettingData;
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        if (name.Length == 1) return name.ToLowerInvariant();
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    private static bool TryGetColumnType(string rawType, out string columnType)
    {
        rawType = rawType.Trim();
        rawType = rawType.Replace(KeyColumn, string.Empty);
        rawType = rawType.Replace(IdColumn, string.Empty);
        columnType = rawType;
        if (string.Equals(columnType, DesignColumnType, StringComparison.OrdinalIgnoreCase)) return false;
        return true;
    }

    private static bool TryGetCsType(string columnType, string columnName, out string csType, out ColumnType columnCsType)
    {
        csType = string.Empty;
        columnCsType = ColumnType.None;
        if (!TryGetColumnType(columnType, out var columnType2)) return false;

        var isEnum = string.Equals(columnType, _enumType, StringComparison.OrdinalIgnoreCase);
        var isVector2 = string.Equals(columnType, _vector2Type, StringComparison.OrdinalIgnoreCase);
        var isList = columnType.Contains("list<");
        if (isList)
        {
            var typeInList = columnType.Replace("list<", "").Replace(">", "");
            if (!TryGetCsType(typeInList, columnName, out var csType2, out _)) return false;
            csType = $"List<{csType2}>";
            if (csType2 == TypeMap[_intType])
            {
                columnCsType = ColumnType.ListInt;
            }
            else if (csType2 == TypeMap[_stringType])
            {
                columnCsType = ColumnType.ListString;
            }
        }
        else if (isEnum)
        {
            csType = columnName;
            columnCsType = ColumnType.Enum;
        }
        else if (isVector2)
        {
            csType = TypeMap.GetValueOrDefault(columnType2 ?? "", "string");;
            columnCsType = ColumnType.Vector2;
        }
        else
        {
            csType = TypeMap.GetValueOrDefault(columnType2 ?? "", "string");
            columnCsType = ColumnType.Normal;
        }

        return true;
    }

    private static int FindKeyIndex(SheetInfo sheet)
    {
        for (var j = 0; j < sheet.ColumnNames.Length; j++)
        {
            if (sheet.ColumnTypes[j] != null && sheet.ColumnTypes[j].Contains(KeyColumn))
            {
                return j;
            }
        }

        return -1;
    }

    private static string CastExpr(string csType, string srcExpr)
    {
        return csType switch
        {
            _intType => $"Convert.ToInt32({srcExpr})",
            _longType => $"Convert.ToInt64({srcExpr})",
            _floatType => $"Convert.ToSingle({srcExpr})",
            _doubleType => $"Convert.ToDouble({srcExpr})",
            _boolType => $"Convert.ToBoolean({srcExpr})",
            _stringType => $"({srcExpr} as string) ?? string.Empty",
            _ => $"{srcExpr}"
        };
    }

    private static void WriteFile(string path, string content)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        AssetDatabase.Refresh();
    }
}
#endif