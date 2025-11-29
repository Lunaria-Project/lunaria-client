using System.Collections.Generic;
using Generated;

public partial class GameData
{
    // Cutscene - Cutscene
    public IReadOnlyList<Cutscene> DTCutscene => _dtCutscene;
    private List<Cutscene> _dtCutscene = new();

    // CutsceneGroup - CutsceneGroup
    public IReadOnlyList<CutsceneGroup> DTCutsceneGroup => _dtCutsceneGroup;
    private List<CutsceneGroup> _dtCutsceneGroup = new();

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
    public IReadOnlyDictionary<string, RequirementInfoData> DTRequirementInfoData => _dtRequirementInfoData;
    public bool TryGetRequirementInfoData(string key, out RequirementInfoData result) => DTRequirementInfoData.TryGetValue(key, out result);
    public bool ContainsRequirementInfoData(string key) => DTRequirementInfoData.ContainsKey(key);
    private readonly Dictionary<string, RequirementInfoData> _dtRequirementInfoData = new();
}
