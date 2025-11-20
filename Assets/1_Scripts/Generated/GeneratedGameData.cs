using System.Collections.Generic;

namespace Generated
{
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

        public MapNpcMenuData(int npcId, string showRequirement, List<int> showRequirementValues, string hideRequirement, List<int> hideRequirementValues, int order, string functionType)
        {
            NpcId = npcId;
            ShowRequirement = showRequirement;
            ShowRequirementValues = showRequirementValues;
            HideRequirement = hideRequirement;
            HideRequirementValues = hideRequirementValues;
            Order = order;
            FunctionType = functionType;
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
