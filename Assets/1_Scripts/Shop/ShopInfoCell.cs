using Lunaria;
using UnityEngine;

public class ShopInfoCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Text _titleText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private GameObject _purchaseButton;

    public void SetData(int itemId, bool canPurchase)
    {
        var itemData = GameData.Instance.GetItemData(itemId);
        _image.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
        _titleText.SetText(itemData.Name);
        _descriptionText.SetText(itemData.Description);
        _purchaseButton.SetActive(canPurchase);
    }
}
