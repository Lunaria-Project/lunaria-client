using System;
using System.Collections.Generic;

[Serializable]
public class UserDataInfo
{
    public Dictionary<int, long> ItemDictionary = new();
    public float SlimeGauge;

    public void AddItem(int itemId, long quantity)
    {
        ItemDictionary.TryAdd(itemId, quantity);
    }
}

public partial class UserData : Singleton<UserData>
{
    public float SlimeGauge => _userDataInfo.SlimeGauge;
    private UserDataInfo _userDataInfo;

    public void Init(UserDataInfo info, bool addInitialItems)
    {
        _userDataInfo = info;
        if (addInitialItems)
        {
            foreach (var (id, data) in GameData.Instance.DTInitialItemData)
            {
                _userDataInfo.AddItem(id, data.Quantity);
            }
        }
    }

    //TODO(지선): 10분마다 더해지도록 구현하기
    public void AddSlimeGauge(float amount)
    {
        _userDataInfo.SlimeGauge += amount;
    }
}