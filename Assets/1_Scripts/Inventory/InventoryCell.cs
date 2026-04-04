using Lunaria;
using UnityEngine;

public class InventoryCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Text _quantity;
    [SerializeField] private GameObject _lockedObject;
    [SerializeField] private GameObject _unlockedObject;

    public int ItemDataId { get; private set; }

    public void SetData(int itemId, long quantity)
    {
        ItemDataId = itemId;

        var itemData = GameData.Instance.GetItemData(itemId);
        _image.SetActive(true);
        _image.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
        _quantity.SetActive(true);
        _quantity.SetText(quantity.ToPrice());
    }

    public void SeyEmpty()
    {
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
}