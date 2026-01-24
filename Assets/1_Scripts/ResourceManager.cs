using System.Collections.Generic;
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

    public Sprite LoadSlimeMinigameSprite(SlimeType type)
    {
        var resourceKey = type switch
        {
            SlimeType.Level1      => "slime_01",
            SlimeType.Level2      => "slime_02",
            SlimeType.Level3      => "slime_03",
            SlimeType.ToxicLevel1 => "toxic_slime_01",
            SlimeType.ToxicLevel2 => "toxic_slime_02",
            SlimeType.ToxicLevel3 => "toxic_slime_03",
            _                     => null,
        };
        return LoadSprite(resourceKey);
    }

    private readonly Dictionary<string, List<Sprite>> _cursorSpriteCache = new();
}