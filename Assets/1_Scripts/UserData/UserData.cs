using System;
using System.Collections.Generic;
using UnityEngine;

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

        GameTimeManager.Instance.OnIntervalChanged -= OnIntervalChanged;
        GameTimeManager.Instance.OnIntervalChanged += OnIntervalChanged;
    }

    private void OnIntervalChanged()
    {
        var amount = UnityEngine.Random.Range(GameSetting.Instance.MinSlimeGaugePer10Minutes, GameSetting.Instance.MaxSlimeGaugePer10Minutes);
        AddSlimeGauge(amount);
    }

    public void AddSlimeGauge(float amount)
    {
        _userDataInfo.SlimeGauge += amount;
        _userDataInfo.SlimeGauge = Mathf.Clamp(_userDataInfo.SlimeGauge, 0, 100);
    }
}