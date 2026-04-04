using System.Collections.Generic;

public partial class UserData
{
    public long GetItemQuantity(int itemDataId)
    {
        foreach (var (id, quantity) in _userDataInfo.ItemList)
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
        foreach (var (id, quantity) in _userDataInfo.ItemList)
        {
            if (!GameData.Instance.TryGetItemData(id, out var itemData)) continue;
            if (itemData.ItemType != itemType) continue;
            items.Add((id, quantity));
        }
        return items;
    }

    public void SortInventory()
    {
        _userDataInfo.ItemList.Sort((a, b) =>
        {
            var aItemData = GameData.Instance.GetItemData(a.ItemId);
            var bItemData = GameData.Instance.GetItemData(b.ItemId);
            var typeCompare = ((int)aItemData.ItemType).CompareTo((int)bItemData.ItemType);
            if (typeCompare != 0) return typeCompare;
            return aItemData.Order.CompareTo(bItemData.Order);
        });
    }

    public void SetEquippedArtifact(int itemId)
    {
        _userDataInfo.EquippedArtifactId = itemId;
    }

    public bool TrySetEquippedArtifact(ArtifactType artifactType)
    {
        var items = GetItemQuantities(ItemType.Artifact);
        foreach (var (id, quantity) in items)
        {
            if (quantity <= 0) continue;
            var data = GameData.Instance.GetArtifactData(id);
            if (data.ArtifactType != artifactType) continue;
            _userDataInfo.EquippedArtifactId = id;
            return true;
        }
        return false;
    }
}