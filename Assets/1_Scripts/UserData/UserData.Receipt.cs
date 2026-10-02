using System.Collections.Generic;

public class DailyMinigameRecord
{
    public List<(int ItemId, long Quantity)> PlayerRewards = new();
    public List<(int ItemId, long Quantity)> FamiliarRewards = new();
}

public partial class UserData // Receipt
{
    public IReadOnlyDictionary<MinigameType, DailyMinigameRecord> DailyMinigameRecords => _userDataInfo.DailyMinigameRecords;

    public void RecordPlayerMinigameRewards(MinigameType minigameType, List<(int Id, long Quantity)> rewards)
    {
        foreach (var reward in rewards)
        {
            RecordPlayerMinigameReward(minigameType, reward.Id, reward.Quantity);
        }
    }

    public void RecordPlayerMinigameReward(MinigameType minigameType, int itemId, long quantity)
    {
        if (quantity <= 0) return;
        AddRecordItem(GetOrCreateDailyMinigameRecord(minigameType).PlayerRewards, itemId, quantity);
    }

    public void RecordFamiliarMinigameReward(MinigameType minigameType, int itemId, long quantity)
    {
        if (quantity <= 0) return;
        AddRecordItem(GetOrCreateDailyMinigameRecord(minigameType).FamiliarRewards, itemId, quantity);
    }

    public void ClearDailyMinigameRecords()
    {
        _userDataInfo.DailyMinigameRecords.Clear();
    }

    private DailyMinigameRecord GetOrCreateDailyMinigameRecord(MinigameType minigameType)
    {
        if (_userDataInfo.DailyMinigameRecords.TryGetValue(minigameType, out var record)) return record;

        record = new DailyMinigameRecord();
        _userDataInfo.DailyMinigameRecords[minigameType] = record;
        return record;
    }

    private static void AddRecordItem(IList<(int ItemId, long Quantity)> items, int itemId, long quantity)
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (items[i].ItemId != itemId) continue;
            items[i] = (itemId, items[i].Quantity + quantity);
            return;
        }
        items.Add((itemId, quantity));
    }
}
