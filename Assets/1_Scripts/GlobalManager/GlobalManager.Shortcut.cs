using Cysharp.Threading.Tasks;
using UnityEngine;

[SerializeEnum]
public enum ShortcutType
{
    Myhome = 1,
    ShoppingSquare = 2,
    PowderShop = 3,
    CottonCandyShop = 4,
    ShoppingSquareByPowderShop = 5,
    ShoppingSquareByBlanketShop = 6,
    ShoppingSquareByCottonCandyShop = 7,
    BlanketShop = 8,
}

public partial class GlobalManager
{
    private bool _isShortcutInvoking;

    public async UniTask ShortcutInvoke(ShortcutType type)
    {
        if (_isShortcutInvoking) return;
        _isShortcutInvoking = true;
        try
        {
            ClearPreviousShortcut();
            await ShortcutInvokeImpl(type);
        }
        finally
        {
            _isShortcutInvoking = false;
        }
    }

    private UniTask ShortcutInvokeImpl(ShortcutType type)
    {
        LogManager.LogColor($"GlobalManager.Shortcut: Invoke type({type})", Color.pink);
        switch (type)
        {
            case ShortcutType.Myhome: return GoToMyhomeAsync();
            case ShortcutType.ShoppingSquare: return GoToShoppingSquareAsync();
            case ShortcutType.PowderShop: return GoToShopAsync(ShopType.PowderShop);
            case ShortcutType.CottonCandyShop: return GoToShopAsync(ShopType.CottonCandyShop);
            case ShortcutType.BlanketShop: return GoToShopAsync(ShopType.BlanketShop);
            case ShortcutType.ShoppingSquareByPowderShop: return GoToShoppingSquareAsync(ShopType.PowderShop);
            case ShortcutType.ShoppingSquareByBlanketShop: return GoToShoppingSquareAsync(ShopType.BlanketShop);
            case ShortcutType.ShoppingSquareByCottonCandyShop: return GoToShoppingSquareAsync(ShopType.CottonCandyShop);
        }
        return UniTask.CompletedTask;
    }

    #region Implement

    private async UniTask GoToShoppingSquareAsync(ShopType type = ShopType.None)
    {
        GameTimeManager.Instance.Pause(this);
        LoadingManager.Instance.ShowLoading(LoadingType.Normal);

        await UniTask.WhenAll(PopupManager.Instance.HideAllPopups(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));

        OnChangeMap(MapType.ShoppingSquare);
        if (type != ShopType.None && MapManager.Instance.CurrentMap is ShoppingSquareMap shoppingSquareMap)
        {
            var position = shoppingSquareMap.GetPlayerPosition(type);
            MapManager.Instance.PlayerObject.Init(position);
        }
        SetCameraSize(1);

        GameTimeManager.Instance.Resume(this);
        LoadingManager.Instance.HideLoading();
    }

    private async UniTask GoToMyhomeAsync()
    {
        GameTimeManager.Instance.Pause(this);
        LoadingManager.Instance.ShowLoading(LoadingType.Normal);

        await UniTask.WhenAll(PopupManager.Instance.HideAllPopups(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));

        OnChangeMap(MapType.Myhome);
        SetCameraSize(1);
        ResetCamaraPosition();

        GameTimeManager.Instance.Resume(this);
        LoadingManager.Instance.HideLoading();
    }

    private async UniTask GoToShopAsync(ShopType shopType)
    {
        GameTimeManager.Instance.Pause(this);
        var loadingType = shopType switch
        {
            ShopType.CottonCandyShop => LoadingType.CottonCandyShop,
            ShopType.PowderShop      => LoadingType.PowderShop,
            ShopType.BlanketShop     => LoadingType.BlanketShop,
            _                        => LoadingType.Normal,
        };
        LoadingManager.Instance.ShowLoading(loadingType);

        await UniTask.WhenAll(PopupManager.Instance.HideAllPopups(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));

        var mapType = shopType switch
        {
            ShopType.PowderShop      => MapType.PowderShop,
            ShopType.CottonCandyShop => MapType.CottonCandyShop,
            ShopType.BlanketShop     => MapType.BlanketShop,
            _                        => MapType.ShoppingSquare,
        };
        OnChangeMap(mapType);
        SetCameraSize(0.5f);
        ResetCamaraPosition();

        GameTimeManager.Instance.Resume(this);
        LoadingManager.Instance.HideLoading();

        _currentShopType = shopType;
    }

    #endregion
}