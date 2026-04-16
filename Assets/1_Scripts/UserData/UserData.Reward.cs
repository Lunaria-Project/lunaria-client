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
        _userDataInfo.AddItem(id, quantity);
    }
}