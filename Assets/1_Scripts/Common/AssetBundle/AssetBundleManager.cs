using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleManager : Singleton<AssetBundleManager>
{
    private const string AssetBundleFolderName = "11_AssetBundles";
    private const string IndexBundleName = "0_index";
    private const string IndexFileName = "asset_bundle_index.bytes";
    private const string EncryptionKey = "your-secret-key"; // TODO(지선) : 뭐로 할까?

    private AssetBundleIndex _index;
    private readonly Dictionary<string, AssetBundle> _loadedBundleByName = new();

    protected override void OnInit()
    {
        base.OnInit();

        var indexFilePath = GetIndexFilePath();
        if (!File.Exists(indexFilePath))
        {
            Debug.LogError($"AssetBundle index file not found. Path: {indexFilePath}");
            return;
        }

        var encryptedBytes = File.ReadAllBytes(indexFilePath);
        var json = SimpleEncryptor.DecryptToString(encryptedBytes, EncryptionKey);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Failed to decrypt AssetBundle index.");
            return;
        }

        _index = JsonUtility.FromJson<AssetBundleIndex>(json);
        if (_index == null)
        {
            Debug.LogError("Failed to deserialize AssetBundle index.");
            return;
        }

        _index.MakeEntryDictionary();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnloadAllBundles(true);
    }

    public bool TryLoadAsset<T>(string assetName, out T asset) where T : Object
    {
        asset = null;
        if (string.IsNullOrEmpty(assetName)) return false;
        if (!_index.TryGetEntry(assetName, out var entry))
        {
            Debug.LogError($"Asset not found in index. AssetName: {assetName}");
            return false;
        }

#if UNITY_EDITOR
        asset = AssetDatabase.LoadAssetAtPath<T>(entry.AssetPathInUnity);
        return asset != null;
#else
        var bundle = LoadBundle(entry.AssetBundleName);
        if (bundle == null) return false;

        asset = bundle.LoadAsset<T>(entry.AssetPathInBundle);
        if (asset == null)
        {
            Debug.LogError($"Failed to load asset from bundle. Bundle: {entry.AssetBundleName}, Asset Path: {entry.AssetPathInBundle}, Type: {typeof(T).Name}");
            return false;
        }

        return true;
#endif
    }

    public void UnloadAllBundles(bool unloadAllLoadedObjects)
    {
        foreach (var pair in _loadedBundleByName)
        {
            var bundle = pair.Value;
            if (bundle != null)
            {
                bundle.Unload(unloadAllLoadedObjects);
            }
        }

        _loadedBundleByName.Clear();
    }

    private AssetBundle LoadBundle(string assetBundleName)
    {
        //TODO(지선) : 이후에 작업하기
        return null;
    }

    public static void BuildIndex()
    {
        var index = new AssetBundleIndex();
        var bundleNames = AssetDatabase.GetAllAssetBundleNames();
        foreach (var bundleName in bundleNames)
        {
            var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            foreach (var assetPath in assetPaths)
            {
                var mainType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                if (mainType == null) continue;

                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                if (string.IsNullOrEmpty(fileName)) continue;

                var entry = new AssetBundleIndexEntry
                {
                    AssetName = fileName.ToLowerInvariant(),
                    AssetBundleName = bundleName,
                    AssetPathInUnity = assetPath,
                };
                index.AddIndexEntry(entry);
            }
        }

        var json = JsonUtility.ToJson(index, true);

        var encryptedBytes = SimpleEncryptor.EncryptToBytes(json, EncryptionKey);

        var editorRootPath = Application.dataPath;
        var bundleRootPath = Path.Combine(editorRootPath, AssetBundleFolderName);
        if (!Directory.Exists(bundleRootPath))
        {
            Directory.CreateDirectory(bundleRootPath);
        }

        var indexFilePath = GetIndexFilePath();
        File.WriteAllBytes(indexFilePath, encryptedBytes);

        Debug.Log($"AssetBundle Index built and saved: {indexFilePath}");
    }

    public static string GetIndexFilePath()
    {
#if UNITY_EDITOR
        var folderPath = Path.Combine(Application.dataPath, AssetBundleFolderName);
        var indexFileBundlePath = Path.Combine(folderPath, IndexBundleName);
        var indexFilePath = Path.Combine(indexFileBundlePath, IndexFileName);
        return indexFilePath;
#endif
        // TODO(지선)
        return string.Empty;
    }
}