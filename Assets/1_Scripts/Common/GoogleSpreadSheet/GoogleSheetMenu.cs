#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class GoogleSheetMenu
{
    [MenuItem("Google Sheet/Generate Data Code")]
    public static void GenerateDataCode()
    {
        GenerateDataCodeAsync().Forget();
        return;

        async UniTask GenerateDataCodeAsync()
        {
            var config = AssetDatabase.LoadAssetAtPath<GoogleSheetConfig>(GoogleSheetConfig.FilePath);
            if (!config)
            {
                Debug.LogError("SpreadSheetsConfig not found");
                return;
            }
            var sheets = await GoogleSheetManager.GetSheetsAsync(config);
            await GoogleSheetCodeGenerator.GenerateAll(config, sheets);
        }
    }
}
#endif