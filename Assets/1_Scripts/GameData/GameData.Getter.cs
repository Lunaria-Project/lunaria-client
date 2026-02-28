using System.Collections.Generic;
using Generated;
using JetBrains.Annotations;

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

    [CanBeNull]
    public MapNpcMenuData GetNpcMenuData(int npcDataId)
    {
        var menuDataList = GetActivatedMapNpcMenuDataListByNpcId(npcDataId);
        var priority = int.MinValue;
        MapNpcMenuData outData = default;
        foreach (var data in menuDataList)
        {
            if (priority >= data.Priority) continue;
            priority = data.Priority;
            outData = data;
        }
        return outData;
    }

    [CanBeNull]
    public MapStaticNpcMenuData GetStaticNpcMenuData(int npcDataId)
    {
        foreach (var data in _dtMapStaticNpcMenuData)
        {
            if (data.NpcId != npcDataId) continue;
            if (!RequirementManager.Instance.IsSatisfied(data.ShowRequirement, data.ShowRequirementValues)) continue;
            return data;
        }
        return null;
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