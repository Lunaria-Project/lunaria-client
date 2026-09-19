using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AssetBundleIndexEntry
{
    public string AssetName;
    public string AssetBundleName;
    [FormerlySerializedAs("AssetPathInBundle")] public string AssetPathInUnity;
}

[Serializable]
public class AssetBundleIndex
{
    // 중복 이름의 리소스 체크. 동적으로 불러오는 리소스만 체크하고 있음
    private static readonly HashSet<string> IndexTargetExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".prefab",
        ".png",
        ".spriteatlas",
    };

    [SerializeField] private List<AssetBundleIndexEntry> _entries = new();

    private Dictionary<string, AssetBundleIndexEntry> _entryByAssetName;

    public static bool IsIndexTarget(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath)) return false;

        return IndexTargetExtensions.Contains(Path.GetExtension(assetPath));
    }

    public void AddIndexEntry(AssetBundleIndexEntry entry)
    {
        _entries.Add(entry);
    }

    public void MakeEntryDictionary()
    {
        if (_entryByAssetName != null) return;

        _entryByAssetName = new Dictionary<string, AssetBundleIndexEntry>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in _entries)
        {
            if (entry == null) continue;
            if (string.IsNullOrEmpty(entry.AssetName)) continue;
            if (!IsIndexTarget(entry.AssetPathInUnity)) continue;
            if (_entryByAssetName.ContainsKey(entry.AssetName))
            {
                LogManager.LogWarning($"[AssetBundle] Duplicate asset name in index: {entry.AssetName}");
                continue;
            }

            _entryByAssetName.Add(entry.AssetName, entry);
        }
    }

    public bool TryGetEntry(string assetName, out AssetBundleIndexEntry entry)
    {
        entry = null;
        if (string.IsNullOrEmpty(assetName)) return false;

        if (_entryByAssetName == null)
        {
            MakeEntryDictionary();
        }

        return _entryByAssetName != null && _entryByAssetName.TryGetValue(assetName, out entry);
    }
}