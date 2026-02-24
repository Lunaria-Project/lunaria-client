using System.Collections.Generic;

public class RequirementManager : Singleton<RequirementManager>
{
    public bool IsSatisfied(RequirementType type, List<int> requirementValues)
    {
        switch (type)
        {
            case RequirementType.AlwaysTrue: return true;
            case RequirementType.AlwaysFalse: return false;
            case RequirementType.ActivateDuration:
            {
                var startInclusiveTime = requirementValues.GetAt(0);
                var endExclusiveTime = requirementValues.GetAt(1);
                var currentTime = GameTimeManager.Instance.CurrentGameTime;
                var currentHHMM = currentTime.Hours * 100 + currentTime.MinutesForUI;
                return startInclusiveTime <= currentHHMM && currentHHMM < endExclusiveTime;
            }
            case RequirementType.MyhomeSlimeAppeared: return UserData.Instance.SlimeGauge >= 100f;
        }
        LogManager.LogErrorPack("대응되지 않은 타입", type);
        return false;
    }
}