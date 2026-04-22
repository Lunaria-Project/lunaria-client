using System;
using Lunaria;
using UnityEngine;

public class ShopCell : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private GameObject _priceObject;
    [SerializeField] private Text _priceText;
    [SerializeField] private GameObject _soldOutObject;
    [SerializeField] private GameObject _remainingStockObject;
    [SerializeField] private Text _remainingStockText;
    [SerializeField] private GameObject _maxPurchasableObject;
    [SerializeField] private Text _maxPurchasableText;

    private int _productId;
    private Action<int> _onClickAction;

    public void SetClickAction(Action<int> onClickAction)
    {
        _onClickAction = onClickAction;
    }

    public void SetData(ShopType shopType, int productId)
    {
        _productId = productId;

        var product = GameData.Instance.GetShopProductData(productId);
        var itemId = product.ProductItemId;
        var itemData = GameData.Instance.GetItemData(itemId);
        _iconImage.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));

        _priceText.SetText(product.PriceQuantity.ToPrice());

        var purchasedToday = UserData.Instance.GetPurchasedCountToday(shopType, itemId);
        var remaining = Mathf.Max(0, product.RefreshAmount - purchasedToday);
        _remainingStockText.SetText(remaining.ToString());

        var hasMaxLimit = product.MaxPurchasableQuantity > 0;
        var purchasedTotal = UserData.Instance.GetPurchasedCountTotal(shopType, itemId);
        var purchasable = Mathf.Max(0, product.MaxPurchasableQuantity - purchasedTotal);
        _maxPurchasableObject.SetActive(hasMaxLimit);
        if (hasMaxLimit)
        {
            _maxPurchasableText.SetText(purchasable.ToString());
        }

        var isSoldOut = remaining <= 0 || (hasMaxLimit && purchasable <= 0);
        _soldOutObject.SetActive(isSoldOut);
        _priceObject.SetActive(!isSoldOut);
        _remainingStockObject.SetActive(!isSoldOut);
    }

    public void OnClickButton()
    {
        _onClickAction?.Invoke(_productId);
    }
}
