using Generated;
using UnityEngine;
using Text = Lunaria.Text;

public class NpcCompassUI : MonoBehaviour
{
    private enum CompassUIType
    {
        None,
        ShowTitle,
        ShowOnlyBubble,
    }

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Text _content;
    [SerializeField] private GameObject _titleBlock;
    [SerializeField] private GameObject _bubbleBlock;

    public NpcInfo NpcInfo { get; private set; }
    private bool _hideWhenNotNearByPlayer;
    private bool _isNearByPlayer;

    public void Init()
    {
        NpcInfo = null;
        gameObject.SetActive(false);
    }

    public void Show(NpcInfo npcInfo)
    {
        NpcInfo = npcInfo;
        gameObject.SetActive(false);
        _titleBlock.SetActive(false);
        _bubbleBlock.SetActive(false);
        _hideWhenNotNearByPlayer = false;

        var npcData = GetNpcMenuData();
        var type = GetCompassUIType(npcData.FunctionType);
        switch (type)
        {
            case CompassUIType.None: return;
            case CompassUIType.ShowTitle:
            {
                _hideWhenNotNearByPlayer = true;
                _titleBlock.SetActive(true);
                _content.SetText(npcData.MenuName);
                _isNearByPlayer = NpcInfo.IsNearByPlayer;
                if (!_isNearByPlayer) return;
                break;
            }
            case CompassUIType.ShowOnlyBubble:
            {
                _bubbleBlock.SetActive(true);
                _content.SetText("...");
                break;
            }
        }
        gameObject.SetActive(true);
    }

    public void UpdatePosition(Vector2 position)
    {
        if (_hideWhenNotNearByPlayer && _isNearByPlayer != NpcInfo.IsNearByPlayer)
        {
            _isNearByPlayer = NpcInfo.IsNearByPlayer;
            gameObject.SetActive(_isNearByPlayer);
        }
        _rectTransform.anchoredPosition = position;
    }

    public void OnCompassUIClick()
    {
        if (!NpcInfo.IsNearByPlayer)
        {
            GlobalManager.Instance.ShowToastMessage("좀 더 가까이 가주세염"); // TODO
            return;
        }
        PopupManager.Instance.ShowPopup(PopupManager.Type.NpcSelection, new NpcSelectionPopupParameter { NpcDataId = NpcInfo.NpcDataId });
    }

    private MapNpcMenuData GetNpcMenuData()
    {
        var npcDataList = GameData.Instance.GetActivatedMapNpcMenuDataListByNpcId(NpcInfo.NpcDataId);
        var currentPriority = -1;
        MapNpcMenuData data = null;
        foreach (var npcData in npcDataList)
        {
            if (!RequirementManager.Instance.IsSatisfied(npcData.ShowRequirement, npcData.ShowRequirementValues)) continue;
            if (RequirementManager.Instance.IsSatisfied(npcData.HideRequirement, npcData.HideRequirementValues)) continue;
            if (npcData.Priority <= currentPriority) continue;
            currentPriority = npcData.Priority;
            data = npcData;
        }
        return data;
    }

    private static CompassUIType GetCompassUIType(NpcMenuFunctionType functionType)
    {
        return functionType switch
        {
            NpcMenuFunctionType.PlayFixedCutscene => CompassUIType.ShowTitle,
            NpcMenuFunctionType.PlaySlimeMinigame => CompassUIType.ShowOnlyBubble,
            _                                     => CompassUIType.None,
        };
    }
}