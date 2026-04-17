using Lunaria;
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

    protected override void OnHide()
    {
    }

    private void Refresh()
    {
        var products = GameData.Instance.GetShopProductDataListByShopType(_shopType);
        LogManager.Assert(_cells.Length >= products.Count,
            $"ShopPopup: Cell count({_cells.Length}) must be >= product count({products.Count}) for {_shopType}");

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
        if (_selectedProductId != 0)
        {
            var itemId = GameData.Instance.GetShopProductData(_selectedProductId).ProductItemId;
            _infoCell.SetData(itemId);
        }
    }

    private void OnCellClick(int productId)
    {
        _selectedProductId = productId;
        var itemId = GameData.Instance.GetShopProductData(productId).ProductItemId;
        _infoCell.SetData(itemId);
    }

    public void OnPurchaseButtonClick()
    {
        // TODO: 구매 로직 (재화 차감, 아이템 지급, RecordShopPurchase 호출, Refresh)
    }
}
