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

    public NpcObject _npcObject { get; private set; }
    private bool _hideWhenNotNearByPlayer;
    private bool _isNearByPlayer;

    public void Init()
    {
        _npcObject = null;
        gameObject.SetActive(false);
    }

    public void Show(NpcObject npcObject)
    {
        _npcObject = npcObject;
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
                _isNearByPlayer = npcObject.IsNearByPlayer;
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
        if (_hideWhenNotNearByPlayer && _isNearByPlayer != _npcObject.IsNearByPlayer)
        {
            _isNearByPlayer = _npcObject.IsNearByPlayer;
            gameObject.SetActive(_isNearByPlayer);
        }
        _rectTransform.anchoredPosition = position;
    }

    public void OnCompassUIClick()
    {
        PopupManager.Instance.ShowPopup(PopupManager.Type.NpcSelection, new NpcSelectionPopupParameter { NpcDataId = _npcObject.NpcDataId });
    }

    private MapNpcMenuData GetNpcMenuData()
    {
        var npcDataList = GameData.Instance.GetActivatedMapNpcMenuDataListByNpcId(_npcObject.NpcDataId);
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

    private CompassUIType GetCompassUIType(NpcMenuFunctionType functionType)
    {
        return functionType switch
        {
            NpcMenuFunctionType.PlayFixedCutscene => CompassUIType.ShowTitle,
            NpcMenuFunctionType.PlaySlimeMinigame => CompassUIType.ShowOnlyBubble,
            _                                     => CompassUIType.None,
        };
    }
}