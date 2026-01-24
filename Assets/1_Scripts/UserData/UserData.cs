using System;
using System.Collections.Generic;

[Serializable]
public class UserDataInfo
{
    public Dictionary<int, long> ItemDictionary = new();

    public void AddItem(int itemId, long quantity)
    {
        ItemDictionary.TryAdd(itemId, quantity);
    }
}

public partial class UserData : Singleton<UserData>
{
    public void AddInitialItems()
    {
        foreach (var (id, data) in GameData.Instance.DTInitialItemData)
        {
            ItemDictionary.TryAdd(id, data.Quantity);
        }
    }

    public void Init(UserDataInfo info)
    {
        ItemDictionary = info.ItemDictionary;
    }
}