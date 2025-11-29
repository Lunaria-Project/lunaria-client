using System.Collections.Generic;
using Generated;

public partial class GameData
{
    // CutsceneData - CutsceneData
    public IReadOnlyList<CutsceneData> DTCutsceneData => _dtCutsceneData;
    private List<CutsceneData> _dtCutsceneData = new();

    // CutsceneGroupData - CutsceneGroupData, key: CutsceneGroupId
    public IReadOnlyDictionary<int, CutsceneGroupData> DTCutsceneGroupData => _dtCutsceneGroupData;
    public bool TryGetCutsceneGroupData(int key, out CutsceneGroupData result) => DTCutsceneGroupData.TryGetValue(key, out result);
    public bool ContainsCutsceneGroupData(int key) => DTCutsceneGroupData.ContainsKey(key);
    private readonly Dictionary<int, CutsceneGroupData> _dtCutsceneGroupData = new();

    // EnumData - EnumData
    public IReadOnlyList<EnumData> DTEnumData => _dtEnumData;
    private List<EnumData> _dtEnumData = new();

    // GameSettingData - GameSettingData
    public IReadOnlyList<GameSettingData> DTGameSettingData => _dtGameSettingData;
    private List<GameSettingData> _dtGameSettingData = new();

    // ItemData - ItemData, key: Id
    public IReadOnlyDictionary<int, ItemData> DTItemData => _dtItemData;
    public bool TryGetItemData(int key, out ItemData result) => DTItemData.TryGetValue(key, out result);
    public bool ContainsItemData(int key) => DTItemData.ContainsKey(key);
    private readonly Dictionary<int, ItemData> _dtItemData = new();

    // MapNpcInfoData - MapNpcInfoData, key: NpcId
    public IReadOnlyDictionary<int, MapNpcInfoData> DTMapNpcInfoData => _dtMapNpcInfoData;
    public bool TryGetMapNpcInfoData(int key, out MapNpcInfoData result) => DTMapNpcInfoData.TryGetValue(key, out result);
    public bool ContainsMapNpcInfoData(int key) => DTMapNpcInfoData.ContainsKey(key);
    private readonly Dictionary<int, MapNpcInfoData> _dtMapNpcInfoData = new();

    // MapNpcMenuData - MapNpcMenuData
    public IReadOnlyList<MapNpcMenuData> DTMapNpcMenuData => _dtMapNpcMenuData;
    private List<MapNpcMenuData> _dtMapNpcMenuData = new();

    // RequirementInfoData - RequirementInfoData, key: RequirementType
    public IReadOnlyDictionary<RequirementType, RequirementInfoData> DTRequirementInfoData => _dtRequirementInfoData;
    public bool TryGetRequirementInfoData(RequirementType key, out RequirementInfoData result) => DTRequirementInfoData.TryGetValue(key, out result);
    public bool ContainsRequirementInfoData(RequirementType key) => DTRequirementInfoData.ContainsKey(key);
    private readonly Dictionary<RequirementType, RequirementInfoData> _dtRequirementInfoData = new();
}
