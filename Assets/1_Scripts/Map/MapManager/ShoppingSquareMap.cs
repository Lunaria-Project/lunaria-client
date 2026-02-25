using UnityEngine;

public class ShoppingSquareMap : NormalMap
{
    [SerializeField] private Transform _powderShopPosition;
    [SerializeField] private Transform _beddingShopPosition;
    [SerializeField] private Transform _cottonCandyShopPosition;
    [SerializeField] private ShopObject[] _shopObjects;
    
    public ShopObject[] ShopObjects => _shopObjects;

    public Vector2 GetPlayerPosition(ShopType type)
    {
        return type switch
        {
            ShopType.PowderShop      => _powderShopPosition.position,
            ShopType.BeddingShop     => _beddingShopPosition.position,
            ShopType.CottonCandyShop => _cottonCandyShopPosition.position,
            _                        => Vector3.zero,
        };
    }
}