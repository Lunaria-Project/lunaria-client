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

    public List<MapNpcMenuData> GetActivatedMapNpcMenuDataListByNpcId(int npcDataId)
    {
        var result = new List<MapNpcMenuData>();
        foreach (var data in _dtMapNpcMenuData)
        {
            if (data.NpcId != npcDataId) continue;
            //TODO(지선): Requirement.IsSatisfied 구현하기
            result.Add(data);
        }

        result.Sort((left, right) => left.Order.CompareTo(right.Order));
        return result;
    }
}