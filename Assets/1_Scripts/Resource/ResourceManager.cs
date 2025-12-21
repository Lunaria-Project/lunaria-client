using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public Sprite LoadSprite(string resourceKey)
    {
        return AssetBundleManager.Instance.TryLoadAsset<Sprite>(resourceKey, out var sprite) ? sprite : null;
    }

    public TPrefab LoadPrefab<TPrefab>(string resourceKey) where TPrefab : MonoBehaviour
    {
        return AssetBundleManager.Instance.TryLoadAsset<TPrefab>(resourceKey, out var prefab) ? prefab : Resources.Load<TPrefab>(resourceKey);
    }

    public TData LoadScriptableObject<TData>(string resourceKey) where TData : ScriptableObject
    {
        return AssetBundleManager.Instance.TryLoadAsset<TData>(resourceKey, out var data) ? data : Resources.Load<TData>(resourceKey);
    }
}