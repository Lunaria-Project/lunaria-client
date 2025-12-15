using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public Sprite LoadSprite(string resourceKey)
    {
        return AssetBundleManager.Instance.TryLoadAsset<Sprite>(resourceKey, out var sprite) ? sprite : null;
    }
}
