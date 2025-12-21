using System.Collections.Generic;

public partial class UserData
{
    public Dictionary<int, long> ItemDictionary { get; private set; } = new();

    // 해당하는 ItemType의 아이템이 한개만 존재한다고 가정
    public long GetItemQuantity(Generated.ItemType itemType)
    {
        foreach (var (id, quantity) in ItemDictionary)
        {
            if (!GameData.Instance.TryGetItemData(id, out var itemData)) continue;
            if (itemData.ItemType != itemType) continue;
            return quantity;
        }
        return 0;
    }
}
