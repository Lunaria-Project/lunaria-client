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

    public PopupBase LoadPopupPrefab(string resourceKey)
    {
        return LoadPrefab<PopupBase>(resourceKey);
    }

    public MapConfig LoadMapConfig()
    {
        return LoadScriptableObject<MapConfig>("map_config");
    }
    
    public Sprite LoadCutsceneCharacterSprite(string resourceKey)
    {
        var cutsceneResourceKey = $"{resourceKey}_cutscene";
        return LoadSprite(cutsceneResourceKey);
    }

    public Sprite LoadSlimeMinigameSprite(int order)
    {
        return LoadSprite($"slime_{order:D2}");
    }
}