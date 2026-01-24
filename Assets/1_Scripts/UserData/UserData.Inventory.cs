using System.Collections.Generic;

public partial class UserData
{
    public Dictionary<int, long> ItemDictionary { get; private set; } = new();

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
}