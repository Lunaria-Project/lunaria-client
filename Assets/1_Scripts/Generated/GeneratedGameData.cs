using System.Collections.Generic;
using UnityEngine;

namespace Generated
{
    public partial class CharacterInfoData
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string ResourceKey { get; private set; }

        public CharacterInfoData(int id, string name, string resourceKey)
        {
            Id = id;
            Name = name;
            ResourceKey = resourceKey;
        }
    }

    public partial class CutsceneData
    {
        public int CutsceneId { get; private set; }
        public int Order { get; private set; }
        public CutsceneCommand CutsceneCommand { get; private set; }
        public string CutsceneMessage { get; private set; }
        public List<int> IntValues { get; private set; }
        public List<string> StringValues { get; private set; }
        public Vector2 Position { get; private set; }

        public CutsceneData(int cutsceneId, int order, CutsceneCommand cutsceneCommand, string cutsceneMessage, List<int> intValues, List<string> stringValues, Vector2 position)
        {
            CutsceneId = cutsceneId;
            Order = order;
            CutsceneCommand = cutsceneCommand;
            CutsceneMessage = cutsceneMessage;
            IntValues = intValues;
            StringValues = stringValues;
            Position = position;
        }
    }

    public partial class CutsceneInfoData
    {
        public int CutsceneId { get; private set; }
        public string CutsceneName { get; private set; }
        public RequirementType TriggerRequirementType { get; private set; }
        public List<int> TriggerRequirementValues { get; private set; }
        public int Priority { get; private set; }
        public List<int> RewardIds { get; private set; }
        public List<int> RewardQuantities { get; private set; }
        public bool IsRepeatable { get; private set; }

        public CutsceneInfoData(int cutsceneId, string cutsceneName, RequirementType triggerRequirementType, List<int> triggerRequirementValues, int priority, List<int> rewardIds, List<int> rewardQuantities, bool isRepeatable)
        {
            CutsceneId = cutsceneId;
            CutsceneName = cutsceneName;
            TriggerRequirementType = triggerRequirementType;
            TriggerRequirementValues = triggerRequirementValues;
            Priority = priority;
            RewardIds = rewardIds;
            RewardQuantities = rewardQuantities;
            IsRepeatable = isRepeatable;
        }
    }

    public partial class CutsceneSelectionData
    {
        public int SelectionId { get; private set; }
        public RequirementType ShowRequirement { get; private set; }
        public List<int> ShowRequirementValues { get; private set; }
        public RequirementType HideRequirement { get; private set; }
        public List<int> HideRequirementValues { get; private set; }
        public string SelectionTitle { get; private set; }
        public int SelectionCutsceneId { get; private set; }

        public CutsceneSelectionData(int selectionId, RequirementType showRequirement, List<int> showRequirementValues, RequirementType hideRequirement, List<int> hideRequirementValues, string selectionTitle, int selectionCutsceneId)
        {
            SelectionId = selectionId;
            ShowRequirement = showRequirement;
            ShowRequirementValues = showRequirementValues;
            HideRequirement = hideRequirement;
            HideRequirementValues = hideRequirementValues;
            SelectionTitle = selectionTitle;
            SelectionCutsceneId = selectionCutsceneId;
        }
    }

    public partial class EnumData
    {
        public string EnumName { get; private set; }
        public string Order { get; private set; }
        public string Value { get; private set; }
        public string DisplayName { get; private set; }
        public string ResourceKey { get; private set; }

        public EnumData(string enumName, string order, string value, string displayName, string resourceKey)
        {
            EnumName = enumName;
            Order = order;
            Value = value;
            DisplayName = displayName;
            ResourceKey = resourceKey;
        }
    }

    public partial class GameSettingData
    {
        public string DataType { get; private set; }
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Design { get; private set; }

        public GameSettingData(string dataType, string name, string value, string design)
        {
            DataType = dataType;
            Name = name;
            Value = value;
            Design = design;
        }
    }

    public partial class ItemData
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string IconResourceKey { get; private set; }
        public ItemType ItemType { get; private set; }

        public ItemData(int id, string name, string iconResourceKey, ItemType itemType)
        {
            Id = id;
            Name = name;
            IconResourceKey = iconResourceKey;
            ItemType = itemType;
        }
    }

    public partial class MapNpcInfoData
    {
        public int NpcId { get; private set; }
        public RequirementType ShowRequirement { get; private set; }
        public List<int> ShowRequirementValues { get; private set; }
        public RequirementType HideRequirement { get; private set; }
        public List<int> HideRequirementValues { get; private set; }

        public MapNpcInfoData(int npcId, RequirementType showRequirement, List<int> showRequirementValues, RequirementType hideRequirement, List<int> hideRequirementValues)
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
        public string MenuName { get; private set; }
        public RequirementType ShowRequirement { get; private set; }
        public List<int> ShowRequirementValues { get; private set; }
        public RequirementType HideRequirement { get; private set; }
        public List<int> HideRequirementValues { get; private set; }
        public int Order { get; private set; }
        public NpcMenuFunctionType FunctionType { get; private set; }
        public int FunctionValue { get; private set; }

        public MapNpcMenuData(int npcId, string menuName, RequirementType showRequirement, List<int> showRequirementValues, RequirementType hideRequirement, List<int> hideRequirementValues, int order, NpcMenuFunctionType functionType, int functionValue)
        {
            NpcId = npcId;
            MenuName = menuName;
            ShowRequirement = showRequirement;
            ShowRequirementValues = showRequirementValues;
            HideRequirement = hideRequirement;
            HideRequirementValues = hideRequirementValues;
            Order = order;
            FunctionType = functionType;
            FunctionValue = functionValue;
        }
    }

    public partial class RequirementInfoData
    {
        public RequirementType RequirementType { get; private set; }

        public RequirementInfoData(RequirementType requirementType)
        {
            RequirementType = requirementType;
        }
    }
}
