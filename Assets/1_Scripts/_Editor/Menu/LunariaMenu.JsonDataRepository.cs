#if UNITY_EDITOR
using UnityEditor;

public static partial class LunariaMenu
{
    [MenuItem("Lunaria/Json Data/[Generate Data from JSON Repository]", priority = 10)]
    public static void TestLoad()
    {
        var sheets = JsonDataLoader.LoadAllSheets();
        DataCodeGenerator.GenerateGameDataCode(sheets);
        DataCodeGenerator.GenerateEnumDataCode(sheets);
        DataCodeGenerator.GenerateGameSettingCode(sheets);
    }

    [MenuItem("Lunaria/Json Data/[Reset DropDownList]", priority = 11)]
    public static void ClearDropDownData()
    {
        DataIdDropDownList.ClearData();
    }
}
#endif