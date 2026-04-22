using System.Collections.Generic;

public partial class UserData // Reward 
{
    public void AddRewards(List<(int Id, long Quantity)> rewards)
    {
        foreach (var reward in rewards)
        {
            AddReward(reward.Id, reward.Quantity);
        }
    }
    public void AddReward(int id, long quantity)
    {
        LogManager.Assert(quantity >= 0, $"AddReward: quantity must be >= 0 (id={id}, quantity={quantity})");
        _userDataInfo.AddItem(id, quantity);
        OnItemQuantityChanged?.Invoke(id);
    }

    public void RemoveItem(int id, long quantity)
    {
        LogManager.Assert(quantity >= 0, $"RemoveItem: quantity must be >= 0 (id={id}, quantity={quantity})");
        _userDataInfo.AddItem(id, -quantity);
        OnItemQuantityChanged?.Invoke(id);
    }
}