using System;
using Lunaria;
using UnityEngine;

public class InventoryCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Text _quantity;
    [SerializeField] private GameObject _lockedObject;
    [SerializeField] private GameObject _unlockedObject;

    public int ItemDataId { get; private set; }
    private int _index;
    private Action<int> _onClickAction;

    public void SetClickAction(Action<int> onClickAction)
    {
        _onClickAction = onClickAction;
    }

    public void SetData(int index, int itemId, long quantity)
    {
        _index = index;
        ItemDataId = itemId;

        var itemData = GameData.Instance.GetItemData(itemId);
        _image.SetActive(true);
        _image.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
        _quantity.SetActive(true);
        _quantity.SetText(quantity.ToPrice());
    }

    public void SeyEmpty()
    {
        _index = -1;
        ItemDataId = 0;
        _image.SetActive(false);
        _quantity.SetActive(false);
    }

    public void SetLocked(bool isLocked)
    {
        SeyEmpty();
        _lockedObject.SetActive(isLocked);
        _unlockedObject.SetActive(!isLocked);
    }

    public void OnClickButton()
    {
        _onClickAction?.Invoke(_index);
    }
}