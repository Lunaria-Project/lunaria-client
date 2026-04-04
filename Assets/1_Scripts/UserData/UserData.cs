using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserDataInfo
{
    public List<(int ItemId, long Quantity)> ItemList = new();
    public float SlimeGauge;
    public int EquippedArtifactId;

    public void AddItem(int itemId, long quantity)
    {
        for (var i = 0; i < ItemList.Count; i++)
        {
            if (ItemList[i].ItemId != itemId) continue;
            ItemList[i] = (itemId, ItemList[i].Quantity + quantity);
            return;
        }
        ItemList.Add((itemId, quantity));
    }
}

public partial class UserData : Singleton<UserData>
{
    public float SlimeGauge => _userDataInfo.SlimeGauge;
    public int EquippedArtifactId => _userDataInfo.EquippedArtifactId;
    
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