#if UNITY_EDITOR
using System.IO;
using UnityEditor;

public static partial class LunariaMenu
{
    private const string ChangedSuffix = "_changed";

    [MenuItem("Lunaria/Project/Rename AssetBundle Files To LowerCase - Step1")]
    private static void RenameToLowerCaseStep1()
    {
        var assetBundlesPath = AssetBundleManager.AssetBundlePath;
        if (!Directory.Exists(assetBundlesPath))
        {
            LogManager.LogError($"Directory not found: {assetBundlesPath}");
            return;
        }

        var files = Directory.GetFiles(assetBundlesPath, "*.png", SearchOption.AllDirectories);
        var renamedCount = 0;

        foreach (var filePath in files)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (directory == null) continue;
            var fileName = Path.GetFileName(filePath);
            var lowerFileName = fileName.ToLower();
            if (string.Equals(fileName, lowerFileName)) continue;

            // icon.png -> icon_changed.png
            var ext = Path.GetExtension(lowerFileName);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(lowerFileName);
            var newFileName = nameWithoutExt + ChangedSuffix + ext;
            var newPath = Path.Combine(directory, newFileName);

            File.Move(filePath, newPath);
            renamedCount++;
            LogManager.Log($"Step1: {fileName} -> {newFileName}");

            // icon.png.meta -> icon_changed.png.meta
            var metaPath = filePath + ".meta";
            if (File.Exists(metaPath))
            {
                var newMetaPath = newPath + ".meta";
                File.Move(metaPath, newMetaPath);
                LogManager.Log($"Step1: {fileName}.meta -> {newFileName}.meta");
            }
        }

        AssetDatabase.Refresh();
        LogManager.Log($"Step1 complete. {renamedCount} file(s) renamed.");
    }

    [MenuItem("Lunaria/Project/Rename AssetBundle Files To LowerCase - Step2")]
    private static void RenameToLowerCaseStep2()
    {
        var assetBundlesPath = AssetBundleManager.AssetBundlePath;
        if (!Directory.Exists(assetBundlesPath))
        {
            LogManager.LogError($"Directory not found: {assetBundlesPath}");
            return;
        }

        var files = Directory.GetFiles(assetBundlesPath, "*" + ChangedSuffix + ".png", SearchOption.AllDirectories);
        var renamedCount = 0;

        foreach (var filePath in files)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (directory == null) continue;
            var fileName = Path.GetFileName(filePath);

            // icon_changed.png -> icon.png
            var ext = Path.GetExtension(fileName);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var originalName = nameWithoutExt.Substring(0, nameWithoutExt.Length - ChangedSuffix.Length);
            var newFileName = originalName + ext;
            var newPath = Path.Combine(directory, newFileName);

            File.Move(filePath, newPath);
            renamedCount++;
            LogManager.Log($"Step2: {fileName} -> {newFileName}");

            // icon_changed.png.meta -> icon.png.meta
            var metaPath = filePath + ".meta";
            if (File.Exists(metaPath))
            {
                var newMetaPath = newPath + ".meta";
                File.Move(metaPath, newMetaPath);
                LogManager.Log($"Step2: {fileName}.meta -> {newFileName}.meta");
            }
        }

        AssetDatabase.Refresh();
        LogManager.Log($"Step2 complete. {renamedCount} file(s) renamed.");
    }
}
#endif
