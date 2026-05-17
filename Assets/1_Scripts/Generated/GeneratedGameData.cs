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
        public string SelectionTitle { get; private set; }
        public int SelectionCutsceneId { get; private set; }

        public CutsceneSelectionData(int selectionId, RequirementType showRequirement, List<int> showRequirementValues, string selectionTitle, int selectionCutsceneId)
        {
            SelectionId = selectionId;
            ShowRequirement = showRequirement;
            ShowRequirementValues = showRequirementValues;
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

    public partial class LocalizationData
    {
        public string key { get; private set; }
        public string ko { get; private set; }
        public string en { get; private set; }
        public string ja { get; private set; }
        public bool UseInCode { get; private set; }

        public LocalizationData(string key, string ko, string en, string ja, bool useInCode)
        {
            key = key;
            ko = ko;
            en = en;
            ja = ja;
            UseInCode = useInCode;
        }
    }

    public partial class ArtifactData
    {
        public int Id { get; private set; }
        public ArtifactType ArtifactType { get; private set; }

        public ArtifactData(int id, ArtifactType artifactType)
        {
            Id = id;
            ArtifactType = artifactType;
        }
    }

    public partial class InitialItemData
    {
        public int ItemId { get; private set; }
        public int Quantity { get; private set; }

        public InitialItemData(int itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }

    public partial class InventoryTabData
    {
        public ItemType ItemType { get; private set; }
        public InventoryTabType InventoryTabType { get; private set; }

        public InventoryTabData(ItemType itemType, InventoryTabType inventoryTabType)
        {
            ItemType = itemType;
            InventoryTabType = inventoryTabType;
        }
    }

    public partial class ItemData
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string IconResourceKey { get; private set; }
        public ItemType ItemType { get; private set; }
        public int Order { get; private set; }

        public ItemData(int id, string name, string description, string iconResourceKey, ItemType itemType, int order)
        {
            Id = id;
            Name = name;
            Description = description;
            IconResourceKey = iconResourceKey;
            ItemType = itemType;
            Order = order;
        }
    }

    public partial class LoadingData
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public string ResourceKey { get; private set; }
        public LoadingType LoadingType { get; private set; }

        public LoadingData(int id, string description, string resourceKey, LoadingType loadingType)
        {
            Id = id;
            Description = description;
            ResourceKey = resourceKey;
            LoadingType = loadingType;
        }
    }

    public partial class MapNpcInfoData
    {
        public int NpcId { get; private set; }
        public int CharacterId { get; private set; }
        public List<float> SpritePositionAndScale { get; private set; }
        public float ColliderRadius { get; private set; }
        public List<float> CompassUIPosition { get; private set; }

        public MapNpcInfoData(int npcId, int characterId, List<float> spritePositionAndScale, float colliderRadius, List<float> compassUIPosition)
        {
            NpcId = npcId;
            CharacterId = characterId;
            SpritePositionAndScale = spritePositionAndScale;
            ColliderRadius = colliderRadius;
            CompassUIPosition = compassUIPosition;
        }
    }

    public partial class MapNpcMenuData
    {
        public int NpcId { get; private set; }
        public string MenuName { get; private set; }
        public RequirementType ShowRequirement { get; private set; }
        public List<int> ShowRequirementValues { get; private set; }
        public int Order { get; private set; }
        public int Priority { get; private set; }
        public bool ShowMenuPopup { get; private set; }
        public NpcMenuFunctionType FunctionType { get; private set; }
        public int FunctionValue { get; private set; }

        public MapNpcMenuData(int npcId, string menuName, RequirementType showRequirement, List<int> showRequirementValues, int order, int priority, bool showMenuPopup, NpcMenuFunctionType functionType, int functionValue)
        {
            NpcId = npcId;
            MenuName = menuName;
            ShowRequirement = showRequirement;
            ShowRequirementValues = showRequirementValues;
            Order = order;
            Priority = priority;
            ShowMenuPopup = showMenuPopup;
            FunctionType = functionType;
            FunctionValue = functionValue;
        }
    }

    public partial class MapNpcPositionData
    {
        public int NpcId { get; private set; }
        public RequirementType ShowRequirement { get; private set; }
        public List<int> ShowRequirementValues { get; private set; }
        public MapType MapType { get; private set; }
        public List<float> Positions { get; private set; }

        public MapNpcPositionData(int npcId, RequirementType showRequirement, List<int> showRequirementValues, MapType mapType, List<float> positions)
        {
            NpcId = npcId;
            ShowRequirement = showRequirement;
            ShowRequirementValues = showRequirementValues;
            MapType = mapType;
            Positions = positions;
        }
    }

    public partial class MapStaticNpcMenuData
    {
        public int NpcId { get; private set; }
        public string MenuName { get; private set; }
        public RequirementType ShowRequirement { get; private set; }
        public List<int> ShowRequirementValues { get; private set; }
        public int Order { get; private set; }
        public int Priority { get; private set; }
        public NpcMenuFunctionType FunctionType { get; private set; }
        public int FunctionValue { get; private set; }

        public MapStaticNpcMenuData(int npcId, string menuName, RequirementType showRequirement, List<int> showRequirementValues, int order, int priority, NpcMenuFunctionType functionType, int functionValue)
        {
            NpcId = npcId;
            MenuName = menuName;
            ShowRequirement = showRequirement;
            ShowRequirementValues = showRequirementValues;
            Order = order;
            Priority = priority;
            FunctionType = functionType;
            FunctionValue = functionValue;
        }
    }

    public partial class MinigameInfoData
    {
        public MinigameType MinigameType { get; private set; }
        public int DurationHours { get; private set; }
        public int MinigameSeconds { get; private set; }
        public ArtifactType EquippedArtifactType { get; private set; }

        public MinigameInfoData(MinigameType minigameType, int durationHours, int minigameSeconds, ArtifactType equippedArtifactType)
        {
            MinigameType = minigameType;
            DurationHours = durationHours;
            MinigameSeconds = minigameSeconds;
            EquippedArtifactType = equippedArtifactType;
        }
    }

    public partial class MinigameRewardData
    {
        public MinigameType MinigameType { get; private set; }
        public int ScoreValue { get; private set; }
        public List<int> RewardIds { get; private set; }
        public List<int> RewardQuantities { get; private set; }

        public MinigameRewardData(MinigameType minigameType, int scoreValue, List<int> rewardIds, List<int> rewardQuantities)
        {
            MinigameType = minigameType;
            ScoreValue = scoreValue;
            RewardIds = rewardIds;
            RewardQuantities = rewardQuantities;
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

    public partial class ShopInfoData
    {
        public int ShopId { get; private set; }
        public int StartTime { get; private set; }
        public int EndTime { get; private set; }
        public ShopType ShopType { get; private set; }
        public MinigameType MinigameType { get; private set; }

        public ShopInfoData(int shopId, int startTime, int endTime, ShopType shopType, MinigameType minigameType)
        {
            ShopId = shopId;
            StartTime = startTime;
            EndTime = endTime;
            ShopType = shopType;
            MinigameType = minigameType;
        }
    }

    public partial class ShopProductData
    {
        public int ShopId { get; private set; }
        public int ProductId { get; private set; }
        public int ProductItemId { get; private set; }
        public int RefreshAmount { get; private set; }
        public int MaxPurchasableQuantity { get; private set; }
        public RequirementType SaleRequirementType { get; private set; }
        public List<int> SaleRequirementValues { get; private set; }
        public int PriceItemId { get; private set; }
        public int PriceQuantity { get; private set; }

        public ShopProductData(int shopId, int productId, int productItemId, int refreshAmount, int maxPurchasableQuantity, RequirementType saleRequirementType, List<int> saleRequirementValues, int priceItemId, int priceQuantity)
        {
            ShopId = shopId;
            ProductId = productId;
            ProductItemId = productItemId;
            RefreshAmount = refreshAmount;
            MaxPurchasableQuantity = maxPurchasableQuantity;
            SaleRequirementType = saleRequirementType;
            SaleRequirementValues = saleRequirementValues;
            PriceItemId = priceItemId;
            PriceQuantity = priceQuantity;
        }
    }
}
