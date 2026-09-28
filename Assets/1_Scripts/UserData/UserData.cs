using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo
{
    public int ItemId;
    public int Quantity;
}

[Serializable]
public class UserDataInfo
{
    public List<(int ItemId, long Quantity)> ItemList = new();
    public int UnlockedInventorySlotCount;
    public int UnlockedQuickSlotCount;
    public int[] QuickSlotItemIds = new int[5];
    public int EquippedArtifactId;
    public float SlimeGauge;
    public HashSet<int> CheckedNewItemIds = new();
    public int CurrentDay;
    public Dictionary<int, Dictionary<ShopType, List<ItemInfo>>> ShopPurchaseRecords = new();
    public List<FamiliarInfo> Familiars = new();
    public int UnlockedFamiliarSlotCount;

    public void AddItem(int itemId, long quantity)
    {
        for (var i = 0; i < ItemList.Count; i++)
        {
            if (ItemList[i].ItemId != itemId) continue;

            // 다 쓴 아이템은 목록에서 제거해 인벤토리 슬롯을 비운다. 퀵슬롯은 아이디를 따로 들고 있어 그대로 유지된다.
            var newQuantity = ItemList[i].Quantity + quantity;
            if (newQuantity <= 0)
            {
                ItemList.RemoveAt(i);
                return;
            }
            ItemList[i] = (itemId, newQuantity);
            return;
        }
        ItemList.Add((itemId, quantity));
    }

    public void AddFamiliar(int familiarCallItemId)
    {
        var familiarCallData = GameData.Instance.GetFamiliarCallData(familiarCallItemId);
        Familiars.Add(new FamiliarInfo
        {
            FamiliarCallItemId = familiarCallItemId,
            CurrentHp = familiarCallData.MaxHp,
            State = FamiliarState.Idle,
        });
    }
}

public partial class UserData : Singleton<UserData>
{
    public List<(int ItemId, long Quantity)> ItemList => _userDataInfo.ItemList;
    public int UnlockedInventorySlotCount => _userDataInfo.UnlockedInventorySlotCount;
    public int UnlockedQuickSlotCount => _userDataInfo.UnlockedQuickSlotCount;
    public int[] QuickSlotItemIds => _userDataInfo.QuickSlotItemIds;
    public int EquippedArtifactId => _userDataInfo.EquippedArtifactId;
    public float SlimeGauge => _userDataInfo.SlimeGauge;
    public HashSet<int> CheckedNewItemIds => _userDataInfo.CheckedNewItemIds;
    public int CurrentDay => _userDataInfo.CurrentDay;

    public void CheckNewItem(int itemId)
    {
        _userDataInfo.CheckedNewItemIds.Add(itemId);
    }
    
    private UserDataInfo _userDataInfo;

    public void Init(UserDataInfo info, bool addInitialItems)
    {
        _userDataInfo = info;
        if (addInitialItems)
        {
            _userDataInfo.UnlockedInventorySlotCount = GameSetting.Instance.InitialInventoryUnlockedSlotCount;
            _userDataInfo.UnlockedQuickSlotCount = GameSetting.Instance.InitialQuickSlotCount;
            _userDataInfo.UnlockedFamiliarSlotCount = GameSetting.Instance.StartFamiliarSlotCount;
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
        UpdateFamiliarWork();
    }

    public void AddSlimeGauge(float amount)
    {
        _userDataInfo.SlimeGauge += amount;
        _userDataInfo.SlimeGauge = Mathf.Clamp(_userDataInfo.SlimeGauge, 0, 100);
    }

    public void AddDay()
    {
        _userDataInfo.CurrentDay++;
        RefreshFamiliarsOnNewDay();
    }
}