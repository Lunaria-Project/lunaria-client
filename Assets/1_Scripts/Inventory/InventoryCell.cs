using System;
using Lunaria;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private GameObject[] _quantityTextObjects;
    [SerializeField] private Text[] _quantityTexts;
    [SerializeField] private GameObject _selectedObject;
    [SerializeField] private GameObject _lockedObject;
    [SerializeField] private GameObject _unlockedObject;

    public int ItemDataId { get; private set; }
    private int _index;
    private Action<int> _onClickAction;
    private Action<InventoryCell> _onBeginDragAction;
    private Action _onEndDragAction;

    public void SetClickAction(Action<int> onClickAction)
    {
        _onClickAction = onClickAction;
    }

    public void SetDragAction(Action<InventoryCell> onBeginDrag, Action onEndDrag)
    {
        _onBeginDragAction = onBeginDrag;
        _onEndDragAction = onEndDrag;
    }

    public void SetData(int index, int itemId, long quantity, bool isSelected)
    {
        _index = index;
        ItemDataId = itemId;

        var itemData = GameData.Instance.GetItemData(itemId);
        _image.SetActive(true);
        _image.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
        _quantityTextObjects.SetActiveAll(true);
        _quantityTexts.SetTexts(quantity.ToPrice());
        
        _selectedObject.SetActive(isSelected);
    }

    public void SeyEmpty()
    {
        _index = -1;
        ItemDataId = 0;
        _image.SetActive(false);
        _quantityTextObjects.SetActiveAll(false);
        _selectedObject.SetActive(false);
    }

    public void SetLocked(bool isLocked)
    {
        SeyEmpty();
        _lockedObject.SetActive(isLocked);
        _unlockedObject.SetActive(!isLocked);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ItemDataId == 0) return;
        _onBeginDragAction?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _onEndDragAction?.Invoke();
    }

    public void OnClickButton()
    {
        _onClickAction?.Invoke(_index);
    }
}