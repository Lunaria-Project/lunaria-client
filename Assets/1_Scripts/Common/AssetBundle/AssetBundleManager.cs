using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetBundleManager : Singleton<AssetBundleManager>
{
    public const string AssetBundlePath = "Assets/11_AssetBundles";
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
        if (!_index.TryGetEntry(assetName, out var entry)) return false;

#if UNITY_EDITOR
        asset = AssetDatabase.LoadAssetAtPath<T>(entry.AssetPathInUnity);
        return asset != null;
#else
        var bundle = LoadBundle(entry.AssetBundleName);
        if (bundle == null) return false;

        asset = bundle.LoadAsset<T>(entry.AssetPathInUnity);
        if (asset == null)
        {
            Debug.LogError($"Failed to load asset from bundle. Bundle: {entry.AssetBundleName}, Path: {entry.AssetPathInUnity}");
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
        if (_loadedBundleByName.TryGetValue(assetBundleName, out var cached))
        {
            return cached;
        }

        var bundlePath = Path.Combine(Application.streamingAssetsPath, AssetBundleFolderName, assetBundleName);
        var bundle = AssetBundle.LoadFromFile(bundlePath);
        if (bundle == null)
        {
            Debug.LogError($"Failed to load AssetBundle: {bundlePath}");
            return null;
        }

        _loadedBundleByName[assetBundleName] = bundle;
        return bundle;
    }

#if UNITY_EDITOR
    public static void BuildIndex()
    {
        var index = new AssetBundleIndex();
        var bundleNames = AssetDatabase.GetAllAssetBundleNames();
        foreach (var bundleName in bundleNames)
        {
            var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            foreach (var assetPath in assetPaths)
            {
                if (!assetPath.Contains(AssetBundlePath)) continue;
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
        AssetDatabase.Refresh();

        Debug.Log($"AssetBundle Index built and saved: {indexFilePath}");
    }
#endif

    public static string GetIndexFilePath()
    {
#if UNITY_EDITOR
        var folderPath = Path.Combine(Application.dataPath, AssetBundleFolderName);
        var indexFileBundlePath = Path.Combine(folderPath, IndexBundleName);
        return Path.Combine(indexFileBundlePath, IndexFileName);
#else
        return Path.Combine(Application.streamingAssetsPath, AssetBundleFolderName, IndexBundleName, IndexFileName);
#endif
    }
}