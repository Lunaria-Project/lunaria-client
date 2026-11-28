using System;
using Lunaria;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArtifactCraftMaterialListCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ItemThumbnail _thumbnail;
    [SerializeField] private LayoutSwitcher _layoutSwitcher;
    [SerializeField] private RectTransform _rectTransform;

    private const string DefaultLayoutKey = "Default";
    private const string SelectedLayoutKey = "Selected";
    private const string HoverLayoutKey = "Hover";

    private const float TooltipHoverDelaySeconds = 0.5f;

    public int ItemDataId { get; private set; }
    public RectTransform RectTransform => _rectTransform;

    private Action<ArtifactCraftMaterialListCell> _onTooltipShowAction;
    private Action<ArtifactCraftMaterialListCell> _onTooltipHideAction;
    private Action<int> _onBeginDragAction;
    private Action _onEndDragAction;
    private bool _isPointerOver;
    private bool _isDragging;
    private bool _isTooltipShown;
    private float _hoverTimer;

    public void SetTooltipAction(Action<ArtifactCraftMaterialListCell> onTooltipShowAction, Action<ArtifactCraftMaterialListCell> onTooltipHideAction)
    {
        _onTooltipShowAction = onTooltipShowAction;
        _onTooltipHideAction = onTooltipHideAction;
    }

    public void SetDragAction(Action<int> onBeginDragAction, Action onEndDragAction)
    {
        _onBeginDragAction = onBeginDragAction;
        _onEndDragAction = onEndDragAction;
    }

    protected void Update()
    {
        if (!_isPointerOver || _isDragging || _isTooltipShown) return;

        _hoverTimer += Time.unscaledDeltaTime;
        if (_hoverTimer < TooltipHoverDelaySeconds) return;
        ShowTooltip();
    }

    public void SetData(int itemId, long quantity)
    {
        ItemDataId = itemId;
        _thumbnail.SetData(itemId, quantity);

        _isPointerOver = false;
        _isDragging = false;
        _isTooltipShown = false;
        _hoverTimer = 0f;
        RefreshLayout();
    }

    public void HideTooltip()
    {
        _isTooltipShown = false;
        RefreshLayout();
    }

    private void ShowTooltip()
    {
        _isTooltipShown = true;
        RefreshLayout();
        _onTooltipShowAction?.Invoke(this);
    }

    private void HideTooltipByCell()
    {
        if (!_isTooltipShown) return;
        HideTooltip();
        _onTooltipHideAction?.Invoke(this);
    }

    private void RefreshLayout()
    {
        if (_isDragging)
        {
            _layoutSwitcher.SetLayout(SelectedLayoutKey);
        }
        else if (_isTooltipShown)
        {
            _layoutSwitcher.SetLayout(HoverLayoutKey);
        }
        else
        {
            _layoutSwitcher.SetLayout(DefaultLayoutKey);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerOver = true;
        _hoverTimer = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerOver = false;
        HideTooltipByCell();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isDragging || _isTooltipShown) return;
        ShowTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        HideTooltipByCell();
        _isDragging = true;
        RefreshLayout();
        _onBeginDragAction?.Invoke(ItemDataId);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
        _hoverTimer = 0f;
        RefreshLayout();
        _onEndDragAction?.Invoke();
    }
}
