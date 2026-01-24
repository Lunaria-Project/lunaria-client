using System.Collections.Generic;
using Generated;

public class RequirementManager : Singleton<RequirementManager>
{
    public bool IsSatisfied(RequirementType type, List<int> requirementValues)
    {
        switch (type)
        {
            case RequirementType.AlwaysTrue: return true;
            case RequirementType.AlwaysFalse: return false;
        }
        LogManager.LogErrorPack("대응되지 않은 타입", type);
        return false;
    }
}