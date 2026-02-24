using UnityEngine;

public class ShopMapManager : BaseMapManager
{
    [SerializeField] private Transform _powderShopCameraPosition;
    [SerializeField] private Transform _powderShopStartPosition;

    public void Init(ShopType type)
    {
        switch (type)
        {
            case ShopType.PowderShop:
            {
                InitPowderShop();
                break;
            }
        }
    }

    public Transform GetCameraPosition(ShopType type)
    {
        return type switch
        {
            ShopType.PowderShop => _powderShopCameraPosition,
            _                   => null,
        };
    }

    private void InitPowderShop()
    {
        //Player.transform.position = _powderShopStartPosition.position;
    }
}