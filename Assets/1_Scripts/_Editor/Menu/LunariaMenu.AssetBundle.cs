#if UNITY_EDITOR
using System.IO;
using UnityEditor;

public static partial class LunariaMenu
{
    [MenuItem("Lunaria/AssetBundles/Build AssetBundle Index")]
    private static void BuildIndex()
    {
        AssetBundleManager.BuildIndex();
    }
    
    [MenuItem("Lunaria/AssetBundles/Check AssetBundle Index File Size")]
    private static void Check()
    {
        var path = AssetBundleManager.GetIndexFilePath();
        if (!File.Exists(path))
        {
            LogManager.LogError($"Index file not found: {path}");
            return;
        }

        var bytes = File.ReadAllBytes(path);
        LogManager.Log($"Index file size = {bytes.Length} bytes");
    }
}
#endif