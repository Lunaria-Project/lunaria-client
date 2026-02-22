using System.Collections.Generic;
using Generated;

public partial class GameData
{
    public List<CutsceneData> GetCutsceneDataListById(int cutsceneId)
    {
        var result = new List<CutsceneData>();
        foreach (var data in DTCutsceneData)
        {
            if (data.CutsceneId != cutsceneId) continue;
            result.Add(data);
        }

        result.Sort((left, right) => left.Order.CompareTo(right.Order));
        return result;
    }

    public List<MapNpcMenuData> GetActivatedMapNpcMenuDataListByNpcId(int npcDataId)
    {
        var result = new List<MapNpcMenuData>();
        foreach (var data in _dtMapNpcMenuData)
        {
            if (data.NpcId != npcDataId) continue;
            if (!RequirementManager.Instance.IsSatisfied(data.ShowRequirement, data.ShowRequirementValues)) continue;

            result.Add(data);
        }

        result.Sort((left, right) => left.Order.CompareTo(right.Order));
        return result;
    }

    public List<LoadingData> GetLoadingDataByLoadingType(LoadingType type)
    {
        var result = new List<LoadingData>();
        foreach (var (_, data) in _dtLoadingData)
        {
            if (data.LoadingType != type) continue;
            result.Add(data);
        }
        return result;
    }
}