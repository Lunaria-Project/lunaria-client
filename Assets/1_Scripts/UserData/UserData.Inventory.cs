using System.Collections.Generic;

public partial class UserData
{
    public int EquippedArtifactId { get; private set; }

    public long GetItemQuantity(int itemDataId)
    {
        foreach (var (id, quantity) in _userDataInfo.ItemDictionary)
        {
            if (id != itemDataId) continue;
            if (!GameData.Instance.ContainsItemData(id)) continue;
            return quantity;
        }
        return 0;
    }

    public List<(int ItemId, long Qunatity)> GetItemQuantities(ItemType itemType)
    {
        var items = new List<(int ItemId, long Qunatity)>();
        foreach (var (id, quantity) in _userDataInfo.ItemDictionary)
        {
            if (!GameData.Instance.TryGetItemData(id, out var itemData)) continue;
            if (itemData.ItemType != itemType) continue;
            items.Add((id, quantity));
        }
        return items;
    }

    public void SetEquippedArtifact(int itemId)
    {
        EquippedArtifactId = itemId;
    }

    public bool TrySetEquippedArtifact(ArtifactType artifactType)
    {
        var items = GetItemQuantities(ItemType.Artifact);
        foreach (var (id, quantity) in items)
        {
            if (quantity <= 0) continue;
            var data = GameData.Instance.GetArtifactData(id);
            if (data.ArtifactType != artifactType) continue;
            EquippedArtifactId = id;
            return true;
        }
        return false;
    }
}