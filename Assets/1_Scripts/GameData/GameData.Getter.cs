using System.Collections.Generic;
using Generated;

public partial class GameData
{
    public List<CutsceneData> GetCutsceneGroupDataListByGroupId(int cutsceneGroupId)
    {
        var result = new List<CutsceneData>();
        foreach (var data in DTCutsceneData)
        {
            if (data.CutsceneGroupId != cutsceneGroupId) continue;
            result.Add(data);
        }

        result.Sort((left, right) => left.Order.CompareTo(right.Order));
        return result;
    }
}