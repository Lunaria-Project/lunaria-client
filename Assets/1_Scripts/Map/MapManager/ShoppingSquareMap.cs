using UnityEngine;

public class ShoppingSquareMap : NormalMap
{
    [SerializeField] private Transform _powderShopPosition;
    [SerializeField] private Transform _blanketShopPosition;
    [SerializeField] private Transform _cottonCandyShopPosition;
    [SerializeField] private ShopObject[] _shopObjects;
    
    public ShopObject[] ShopObjects => _shopObjects;

    public Vector2 GetPlayerPosition(ShopType type)
    {
        return type switch
        {
            ShopType.PowderShop      => _powderShopPosition.position,
            ShopType.BlanketShop     => _blanketShopPosition.position,
            ShopType.CottonCandyShop => _cottonCandyShopPosition.position,
            _                        => Vector3.zero,
        };
    }
}