using System.Collections.Generic;

public partial class UserData
{
    public Dictionary<int, long> ItemDictionary { get; private set; } = new();
    public int EquippedArtifact { get; private set; }

    public long GetItemQuantity(int itemDataId)
    {
        foreach (var (id, quantity) in ItemDictionary)
        {
            if (id != itemDataId) continue;
            if (!GameData.Instance.ContainsItemData(id)) continue;
            return quantity;
        }
        return 0;
    }

    public List<(int ItemId, long Qunatity)> GetItemQuantities(Generated.ItemType itemType)
    {
        var items = new List<(int ItemId, long Qunatity)>();
        foreach (var (id, quantity) in ItemDictionary)
        {
            if (!GameData.Instance.TryGetItemData(id, out var itemData)) continue;
            if (itemData.ItemType != itemType) continue;
            items.Add((id, quantity));
        }
        return items;
    }

    public void SetEquippedArtifact(int itemId)
    {
        EquippedArtifact = itemId;
    }
}