using Generated;
using UnityEngine;

public class ShoppingSquareMapManager : BaseMapManager
{
    [SerializeField] private Transform _powderShopPosition;
    [SerializeField] private Transform _beddingShopPosition;
    [SerializeField] private Transform _cottonCandyShopPosition;

    public void InitWithShopType(ShopType type)
    {
        var playerPosition = type switch
        {
            ShopType.PowderShop      => _powderShopPosition.position,
            ShopType.BeddingShop     => _beddingShopPosition.position,
            ShopType.CottonCandyShop => _cottonCandyShopPosition.position,
            _                        => Vector3.zero,
        };
        Player.transform.position = playerPosition;
    }
}