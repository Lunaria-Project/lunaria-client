#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;

public class AssetBundleAutoAssigner : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths
    )
    {
        if (importedAssets != null)
        {
            foreach (var assetPath in importedAssets)
            {
                ApplyBundleNameFromFolderLabel(assetPath);
            }
        }

        if (movedAssets != null)
        {
            foreach (var assetPath in movedAssets)
            {
                ApplyBundleNameFromFolderLabel(assetPath);
            }
        }
    }

    private static void ApplyBundleNameFromFolderLabel(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
        {
            return;
        }

        if (!assetPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (Directory.Exists(assetPath))
        {
            return;
        }

        var importer = AssetImporter.GetAtPath(assetPath);
        if (importer == null)
        {
            return;
        }

        var bundleName = GetBundleNameFromFolderHierarchy(assetPath);

        if (string.IsNullOrEmpty(bundleName))
        {
            if (!string.IsNullOrEmpty(importer.assetBundleName))
            {
                importer.assetBundleName = string.Empty;
            }

            return;
        }

        if (importer.assetBundleName != bundleName)
        {
            importer.assetBundleName = bundleName;
        }
    }

    private static string GetBundleNameFromFolderHierarchy(string assetPath)
    {
        var directoryPath = Path.GetDirectoryName(assetPath);
        if (string.IsNullOrEmpty(directoryPath))
        {
            return null;
        }

        directoryPath = directoryPath.Replace('\\', '/');

        while (!string.IsNullOrEmpty(directoryPath))
        {
            var folderObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(directoryPath);
            if (folderObject != null)
            {
                var labels = AssetDatabase.GetLabels(folderObject);
                foreach (var bundleName in labels)
                {
                    if (string.IsNullOrEmpty(bundleName)) continue;
                    return bundleName;
                }
            }

            var lastSlashIndex = directoryPath.LastIndexOf('/');
            if (lastSlashIndex <= 0)
            {
                break;
            }

            directoryPath = directoryPath.Substring(0, lastSlashIndex);
        }

        return null;
    }
}
#endif