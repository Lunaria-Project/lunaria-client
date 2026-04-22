using System;
using System.Collections.Generic;

public partial class UserData // Inventory
{
    public event Action<int> OnItemQuantityChanged;

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

    #region Artifact

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

    #endregion

    #region QuickSlot

    public void SetQuickSlot(int slotIndex, int itemId)
    {
        if (slotIndex < 0 || slotIndex >= _userDataInfo.QuickSlotItemIds.Length) return;
        if (slotIndex >= _userDataInfo.UnlockedQuickSlotCount) return;

        var prevItemId = _userDataInfo.QuickSlotItemIds[slotIndex];

        for (var i = 0; i < _userDataInfo.QuickSlotItemIds.Length; i++)
        {
            if (_userDataInfo.QuickSlotItemIds[i] != itemId) continue;
            _userDataInfo.QuickSlotItemIds[i] = prevItemId;
            break;
        }

        _userDataInfo.QuickSlotItemIds[slotIndex] = itemId;
    }

    public void ClearQuickSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _userDataInfo.QuickSlotItemIds.Length) return;
        _userDataInfo.QuickSlotItemIds[slotIndex] = 0;
    }

    public int GetQuickSlotItemId(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _userDataInfo.QuickSlotItemIds.Length) return 0;
        return _userDataInfo.QuickSlotItemIds[slotIndex];
    }

    #endregion
}