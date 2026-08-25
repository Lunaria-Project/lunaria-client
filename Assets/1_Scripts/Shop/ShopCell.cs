using System;
using Lunaria;
using UnityEngine;

public class ShopCell : MonoBehaviour
{
    [SerializeField] private Image[] _iconImages;
    [SerializeField] private Text _priceText;
    [SerializeField] private Text _remainingStockText;
    [SerializeField] private LayoutSwitcher _layoutSwitcher;

    private int _productId;
    private Action<int> _onClickAction;

    private const string DefaultLayoutKey = "Default";
    private const string BargainLayoutKey = "Bargain"; // TODO(지선)
    private const string AlmostSoldOutLayoutKey = "AlmostSoldOut"; // TODO(지선)
    private const string SoldOutLayoutKey = "SoldOut";
    private const string OwnedLayoutKey = "Owned";

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
        _iconImages.SetSprites(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));

        _priceText.SetText(product.PriceQuantity.ToPrice());

        var purchasedToday = UserData.Instance.GetPurchasedCountToday(shopType, itemId);
        var remaining = Mathf.Max(0, product.RefreshAmount - purchasedToday);
        _remainingStockText.SetText(remaining.ToPrice());

        var hasMaxLimit = product.MaxPurchasableQuantity > 0;
        if (hasMaxLimit)
        {
            _layoutSwitcher.SetLayout(OwnedLayoutKey);
        }
        else if (remaining <= 0)
        {
            _layoutSwitcher.SetLayout(SoldOutLayoutKey);
        }
        else
        {
            _layoutSwitcher.SetLayout(DefaultLayoutKey);
        }
    }

    public void OnClickButton()
    {
        _onClickAction?.Invoke(_productId);
    }
}