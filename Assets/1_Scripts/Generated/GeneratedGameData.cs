using System.Collections.Generic;
using UnityEngine;

namespace Generated
{
    public partial class Cutscene
    {
        public int CutsceneGroupId { get; private set; }
        public int CutsceneId { get; private set; }
        public int Order { get; private set; }
        public string CutsceneCommand { get; private set; }
        public string CutsceneMessage { get; private set; }
        public List<int> IntValues { get; private set; }
        public List<string> StringValues { get; private set; }
        public Vector2 Position { get; private set; }

        public Cutscene(int cutsceneGroupId, int cutsceneId, int order, string cutsceneCommand, string cutsceneMessage, List<int> intValues, List<string> stringValues, Vector2 position)
        {
            CutsceneGroupId = cutsceneGroupId;
            CutsceneId = cutsceneId;
            Order = order;
            CutsceneCommand = cutsceneCommand;
            CutsceneMessage = cutsceneMessage;
            IntValues = intValues;
            StringValues = stringValues;
            Position = position;
        }
    }

    public partial class CutsceneGroup
    {
        public int CutsceneGroupId { get; private set; }
        public string CutsceneGroupName { get; private set; }
        public string TriggerRequirementType { get; private set; }
        public List<int> TriggerRequirementValues { get; private set; }
        public int Priority { get; private set; }
        public List<int> RewardIds { get; private set; }
        public List<int> RewardQuantities { get; private set; }
        public bool IsRepeatable { get; private set; }

        public CutsceneGroup(int cutsceneGroupId, string cutsceneGroupName, string triggerRequirementType, List<int> triggerRequirementValues, int priority, List<int> rewardIds, List<int> rewardQuantities, bool isRepeatable)
        {
            CutsceneGroupId = cutsceneGroupId;
            CutsceneGroupName = cutsceneGroupName;
            TriggerRequirementType = triggerRequirementType;
            TriggerRequirementValues = triggerRequirementValues;
            Priority = priority;
            RewardIds = rewardIds;
            RewardQuantities = rewardQuantities;
            IsRepeatable = isRepeatable;
        }
    }

    public partial class ItemData
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string IconResourceKey { get; private set; }

        public ItemData(int id, string name, string iconResourceKey)
        {
            Id = id;
            Name = name;
            IconResourceKey = iconResourceKey;
        }
    }

    public partial class MapNpcInfoData
    {
        public int NpcId { get; private set; }
        public string ShowRequirement { get; private set; }
        public List<int> ShowRequirementValues { get; private set; }
        public string HideRequirement { get; private set; }
        public List<int> HideRequirementValues { get; private set; }

        public MapNpcInfoData(int npcId, string showRequirement, List<int> showRequirementValues, string hideRequirement, List<int> hideRequirementValues)
        {
            NpcId = npcId;
            ShowRequirement = showRequirement;
            ShowRequirementValues = showRequirementValues;
            HideRequirement = hideRequirement;
            HideRequirementValues = hideRequirementValues;
        }
    }

    public partial class MapNpcMenuData
    {
        public int NpcId { get; private set; }
        public string ShowRequirement { get; private set; }
        public List<int> ShowRequirementValues { get; private set; }
        public string HideRequirement { get; private set; }
        public List<int> HideRequirementValues { get; private set; }
        public int Order { get; private set; }
        public string FunctionType { get; private set; }
        public int FunctionValues { get; private set; }
        public string FunctionName { get; private set; }

        public MapNpcMenuData(int npcId, string showRequirement, List<int> showRequirementValues, string hideRequirement, List<int> hideRequirementValues, int order, string functionType, int functionValues, string functionName)
        {
            NpcId = npcId;
            ShowRequirement = showRequirement;
            ShowRequirementValues = showRequirementValues;
            HideRequirement = hideRequirement;
            HideRequirementValues = hideRequirementValues;
            Order = order;
            FunctionType = functionType;
            FunctionValues = functionValues;
            FunctionName = functionName;
        }
    }

    public partial class RequirementInfoData
    {
        public string RequirementType { get; private set; }

        public RequirementInfoData(string requirementType)
        {
            RequirementType = requirementType;
        }
    }
}
