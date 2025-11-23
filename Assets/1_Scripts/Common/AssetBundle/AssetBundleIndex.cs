using System;
using System.Collections.Generic;
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
    [SerializeField] private List<AssetBundleIndexEntry> _entries = new();

    private Dictionary<string, AssetBundleIndexEntry> _entryByAssetName;

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
            if (_entryByAssetName.ContainsKey(entry.AssetName))
            {
                Debug.LogWarning($"Duplicate asset name in index: {entry.AssetName}");
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