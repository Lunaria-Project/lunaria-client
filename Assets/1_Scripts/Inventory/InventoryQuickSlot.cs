using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryQuickSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    [SerializeField] private InventoryCell _cell;

    private int _slotIndex;
    private Action<int> _onDropAction;
    private Action<int> _onClickAction;
    private Action<int> _onBeginDragAction;
    private Action _onEndDragAction;

    public void SetClickAction(Action<int> onClickAction)
    {
        _onClickAction = onClickAction;
    }

    public void SetDragAction(Action<int> onBeginDrag, Action onEndDrag, Action<int> onDropAction)
    {
        _onBeginDragAction = onBeginDrag;
        _onEndDragAction = onEndDrag;
        _onDropAction = onDropAction;
    }

    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }

    public void Refresh()
    {
        var itemId = UserData.Instance.GetQuickSlotItemId(_slotIndex);
        if (itemId == 0)
        {
            _cell.SeyEmpty();
            return;
        }

        var quantity = UserData.Instance.GetItemQuantity(itemId);
        if (quantity <= 0)
        {
            // TODO(지선): 비우는게 아니라 딤드처리 되어야함
            return;
        }

        _cell.SetData(_slotIndex, itemId, quantity, false); // TODO(지선): 선택된 상태는 어떻게 처리할지 고민
    }

    public void SetLocked(bool isLocked)
    {
        _cell.SetLocked(isLocked);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var itemId = UserData.Instance.GetQuickSlotItemId(_slotIndex);
        if (itemId == 0) return;
        _onBeginDragAction?.Invoke(itemId);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _onEndDragAction?.Invoke();
    }

    public void OnDrop(PointerEventData eventData)
    {
        _onDropAction?.Invoke(_slotIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount < 2) return;
        _onClickAction?.Invoke(_slotIndex);
    }
}
