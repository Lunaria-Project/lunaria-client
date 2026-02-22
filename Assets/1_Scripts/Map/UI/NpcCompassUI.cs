using Lunaria;
using UnityEngine;
using Generated;

public class NpcCompassUI : MonoBehaviour
{
    private enum CompassUIType
    {
        None,
        ShowTitleWhenNearby,
        ShowOnlyBubble,
    }

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Text _content;
    [SerializeField] private GameObject _titleBlock;
    [SerializeField] private GameObject _bubbleBlock;

    private int _npcDataId;
    private CompassUIType _currentType;
    private bool _isNearByPlayer;

    public void Init(Vector2 position, int npcDataId)
    {
        _rectTransform.anchoredPosition = position;
        _npcDataId = npcDataId;
        _isNearByPlayer = false;

        GameTimeManager.Instance.OnIntervalChanged -= OnIntervalChanged;
        GameTimeManager.Instance.OnIntervalChanged += OnIntervalChanged;
        OnIntervalChanged();
    }

    public void SetIsNearBy(bool isNearByPlayer)
    {
        if (_isNearByPlayer == isNearByPlayer) return;
        _isNearByPlayer = isNearByPlayer;
        Refresh();
    }

    public void OnHide()
    {
        GameTimeManager.Instance.OnIntervalChanged -= OnIntervalChanged;
    }

    private void Refresh()
    {
        gameObject.SetActive(false);
        _titleBlock.SetActive(false);
        _bubbleBlock.SetActive(false);
        switch (_currentType)
        {
            case CompassUIType.None: break;
            case CompassUIType.ShowTitleWhenNearby:
            {
                if (!_isNearByPlayer) return;
                gameObject.SetActive(true);
                _titleBlock.SetActive(true);
                
                var menuData = GetNpcMenuData();
                _content.SetText(menuData.MenuName);
                break;
            }
            case CompassUIType.ShowOnlyBubble:
            {
                gameObject.SetActive(true);
                _bubbleBlock.SetActive(true);
                _content.SetText("...");
                break;
            }
        }
    }

    private void OnIntervalChanged()
    {
        var npcData = GetNpcMenuData();
        _currentType = GetCompassUIType(npcData.FunctionType);
        Refresh();
    }

    private MapNpcMenuData GetNpcMenuData()
    {
        var menuDataList = GameData.Instance.GetActivatedMapNpcMenuDataListByNpcId(_npcDataId);
        var priority = int.MinValue;
        MapNpcMenuData outData = default;
        foreach (var data in menuDataList)
        {
            if (priority >= data.Priority) continue;
            priority = data.Priority;
            outData = data;
        }
        return outData;
    }

    public void OnCompassUIClick()
    {
        if (!_isNearByPlayer)
        {
            GlobalManager.Instance.ShowToastMessage("좀 더 가까이 가주세염"); // TODO
            return;
        }
        var data = GetNpcMenuData();
        if (!data.ShowMenuPopup)
        {
            NpcSelectionPopup.SelectNpcMenu(data.FunctionType, data.FunctionValue);
            return;
        }
        PopupManager.Instance.ShowPopup(PopupManager.Type.NpcSelection, new NpcSelectionPopupParameter { NpcDataId =  _npcDataId });
    }

    private static CompassUIType GetCompassUIType(NpcMenuFunctionType functionType)
    {
        return functionType switch
        {
            NpcMenuFunctionType.PlayFixedCutscene        => CompassUIType.ShowTitleWhenNearby,
            NpcMenuFunctionType.PlaySlimeMinigame        => CompassUIType.ShowOnlyBubble,
            NpcMenuFunctionType.PlayPowderPortalMinigame => CompassUIType.ShowOnlyBubble,
            _                                            => CompassUIType.None,
        };
    }
}