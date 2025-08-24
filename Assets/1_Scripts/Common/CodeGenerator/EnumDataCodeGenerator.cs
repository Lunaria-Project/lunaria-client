using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnumDataCodeGenerator
{
    public static async UniTask Generate(List<SheetInfo> sheetInfos)
    {
        var enumSb = MakeEnumDataClass(sheetInfos);
        await GoogleSheetCodeGenerator.WriteGameDataToFileAsync(GoogleSheetCodeGenerator.EnumDataFilePath, enumSb);
    }

    private static StringBuilder MakeEnumDataClass(List<SheetInfo> sheetInfos)
    {
        var enumDic = new Dictionary<string, List<string>>();
        foreach (var sheetInfo in sheetInfos)
        {
            foreach (var (columnIndex, enumList) in sheetInfo.EnumList)
            {
                var enumName = sheetInfo.ColumnNames[columnIndex];
                if (!enumDic.ContainsKey(enumName))
                {
                    enumDic.Add(enumName, new List<string>());
                }

                foreach (var e in enumList)
                {
                    if (string.IsNullOrEmpty(e)) continue;
                    
                    if (!enumDic[enumName].Contains(e))
                    {
                        enumDic[enumName].Add(e);
                    }
                }
            }
        }

        var sb = new StringBuilder();
        sb.AppendLine($"namespace {GoogleSheetCodeGenerator.GeneratedGameDataNameSpace}");
        sb.AppendLine("{");
        foreach (var (enumName, enumList) in enumDic)
        {
            sb.AppendIndentedLine($"public enum {enumName}", 1);
            sb.AppendIndentedLine("{", 1);
            sb.AppendIndentedLine("None,", 2);
            for (var i = 0; i < enumList.Count; i++)
            {
                sb.AppendIndentedLine($"{enumList[i]} = {i + 1},", 2);
            }

            sb.AppendIndentedLine("}", 1);
        }

        sb.AppendLine("}");
        return sb;
    }
}