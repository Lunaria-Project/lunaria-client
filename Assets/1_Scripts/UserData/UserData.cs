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

public class UserData : Singleton<UserData>
{
    public Dictionary<int, long> ItemDictionary { get; private set; } = new();

    public void Init(UserDataInfo info)
    {
        ItemDictionary = info.ItemDictionary;
    }
}