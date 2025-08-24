using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public class GameDataCodeGenerator
{
    public static async UniTask Generate(List<SheetInfo> loadedDataList)
    {
        var generatedSb = MakeDataClass(loadedDataList);
        await GoogleSheetCodeGenerator.WriteGameDataToFileAsync(GoogleSheetCodeGenerator.GameDataFilePath, generatedSb);
    }

    private static StringBuilder MakeDataClass(List<SheetInfo> loadedDataList)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace {GoogleSheetCodeGenerator.GeneratedGameDataNameSpace}");
        sb.AppendLine("{");
        for (var i = 0; i < loadedDataList.Count; i++)
        {
            var loadedData = loadedDataList[i];
            sb.AppendIndentedLine($"public class {loadedData.SheetName}", 1);
            sb.AppendIndentedLine("{", 1);

            for (var j = 0; j < loadedData.ColumnNames.Length; j++)
            {
                var columnName = loadedData.ColumnNames[j];
                if (loadedData.ColumnTypes[j].ToLower() == GoogleSheetCodeGenerator.TypeEnum)
                {
                    sb.AppendIndentedLine($"public {columnName} {columnName}" + " { get; private set; }", 2);
                }
                else
                {
                    var columnType = GoogleSheetCodeGenerator.StringToTypes[loadedData.ColumnTypes[j]];
                    sb.AppendIndentedLine($"public {columnType} {columnName}" + " { get; private set; }", 2);
                }
            }

            sb.AppendLine();

            var paramList = GoogleSheetCodeGenerator.MakeParameters(loadedData.ColumnTypes, loadedData.ColumnNames);
            sb.AppendIndentedLine($"public {loadedData.SheetName}({paramList})", 2);
            sb.AppendIndentedLine("{", 2);
            foreach (var columnName in loadedData.ColumnNames)
            {
                sb.AppendIndentedLine($"{columnName} = {columnName.FirstCharacterToLower()};", 3);
            }

            sb.AppendIndentedLine("}", 2);
            sb.AppendIndentedLine("}", 1);
            if (i != loadedDataList.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");
        return sb;
    }
}