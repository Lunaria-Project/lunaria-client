using UnityEngine;

public struct ShopPopupParameter : IPopupParameter
{
    public ShopType ShopType { get; init; }
}

public class ShopPopup : Popup<ShopPopupParameter>
{
    [SerializeField] private ShopCell[] _cells;
    [SerializeField] private ShopInfoCell _infoCell;

    private ShopType _shopType;
    private int _selectedProductId;

    protected void Awake()
    {
        foreach (var cell in _cells)
        {
            cell.SetClickAction(OnCellClick);
        }
    }

    protected override void OnShow(ShopPopupParameter parameter)
    {
        _shopType = parameter.ShopType;
        _selectedProductId = 0;
        Refresh();
    }

    protected override void OnHide() { }

    private void Refresh()
    {
        var products = GameData.Instance.GetShopProductDataListByShopType(_shopType);
        LogManager.Assert(_cells.Length >= products.Count, $"ShopPopup: Cell count({_cells.Length}) must be >= product count({products.Count}) for {_shopType}");

        _cells.SetActiveAll(false);
        for (var i = 0; i < products.Count && i < _cells.Length; i++)
        {
            _cells[i].gameObject.SetActive(true);
            _cells[i].SetData(_shopType, products[i].ProductId);
        }

        if (_selectedProductId == 0 && products.Count > 0)
        {
            _selectedProductId = products[0].ProductId;
        }
        var showInfoBlock = _selectedProductId != 0;
        _infoCell.gameObject.SetActive(showInfoBlock);
        if (showInfoBlock)
        {
            var itemId = GameData.Instance.GetShopProductData(_selectedProductId).ProductItemId;
            var canPurchase = UserData.Instance.CanPurchaseShopProduct(_shopType, _selectedProductId);
            _infoCell.SetData(itemId, canPurchase);
        }
    }

    private void OnCellClick(int productId)
    {
        _selectedProductId = productId;
        var itemId = GameData.Instance.GetShopProductData(productId).ProductItemId;
        var canPurchase = UserData.Instance.CanPurchaseShopProduct(_shopType, productId);
        _infoCell.SetData(itemId, canPurchase);
    }

    public void OnPurchaseButtonClick()
    {
        if (_selectedProductId == 0) return;
        var product = GameData.Instance.GetShopProductData(_selectedProductId);
        var itemId = product.ProductItemId;

        var purchasedToday = UserData.Instance.GetPurchasedCountToday(_shopType, itemId);
        if (product.RefreshAmount - purchasedToday <= 0)
        {
            GlobalManager.Instance.ShowToastMessage(LocalizationKey.ShopPopup_PurchasedAllTodayGuide.ToString());
            return;
        }

        if (product.MaxPurchasableQuantity > 0)
        {
            var purchasedTotal = UserData.Instance.GetPurchasedCountTotal(_shopType, itemId);
            if (product.MaxPurchasableQuantity - purchasedTotal <= 0)
            {
                GlobalManager.Instance.ShowToastMessage(LocalizationKey.ShopPopup_PurchasedAllGuide.ToString());
                return;
            }
        }

        if (UserData.Instance.GetItemQuantity(product.PriceItemId) < product.PriceQuantity)
        {
            GlobalManager.Instance.ShowToastMessage(LocalizationKey.ShopPopup_InsufficientPriceItem.ToString());
            return;
        }

        if (!UserData.Instance.PurchaseShopProduct(_shopType, _selectedProductId)) return;

        Refresh();
    }
}