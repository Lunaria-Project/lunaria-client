using Cysharp.Threading.Tasks;
using UnityEngine;

public enum ShortcutType
{
    Myhome,
    ShoppingSquare,
    PowderShop,
    ShoppingSquareByPowderShop,
    ShoppingSquareByBeddingShop,
    ShoppingSquareByCottonCandyShop,
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
            case ShortcutType.ShoppingSquareByPowderShop: return GoToShoppingSquareAsync(ShopType.PowderShop);
            case ShortcutType.ShoppingSquareByBeddingShop: return GoToShoppingSquareAsync(ShopType.BeddingShop);
            case ShortcutType.ShoppingSquareByCottonCandyShop: return GoToShoppingSquareAsync(ShopType.CottonCandyShop);
        }
        return UniTask.CompletedTask;
    }

    #region Implement

    private async UniTask GoToShoppingSquareAsync(ShopType type = ShopType.None)
    {
        GameTimeManager.Instance.Pause();
        LoadingManager.Instance.ShowLoading(LoadingType.Normal);

        await UniTask.WhenAll(PopupManager.Instance.HideAllPopups(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));
        PanelManager.Instance.ShowPanel(PanelManager.Type.ShoppingSquareMain);

        OnChangeMap(MapType.ShoppingSquare);
        if (type != ShopType.None && MapManager.Instance.CurrentMap is ShoppingSquareMap shoppingSquareMap)
        {
            MapManager.Instance.PlayerObject.Init(shoppingSquareMap.PlayerInitPosition.position);
        }
        SetCameraSize(1);

        GameTimeManager.Instance.Resume();
        LoadingManager.Instance.HideLoading();
    }

    private async UniTask GoToMyhomeAsync()
    {
        GameTimeManager.Instance.Pause();
        LoadingManager.Instance.ShowLoading(LoadingType.Normal);

        await UniTask.WhenAll(PopupManager.Instance.HideAllPopups(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));
        PanelManager.Instance.ShowPanel(PanelManager.Type.MyhomeMain);

        OnChangeMap(MapType.Myhome);
        SetCameraSize(1);
        ResetCamaraPosition();

        GameTimeManager.Instance.Resume();
        LoadingManager.Instance.HideLoading();
    }

    private async UniTask GoToShopAsync(ShopType shopType)
    {
        GameTimeManager.Instance.Pause();
        var loadingType = shopType switch
        {
            ShopType.CottonCandyShop => LoadingType.CottonCandyShop,
            ShopType.PowderShop      => LoadingType.PowderShop,
            ShopType.BeddingShop     => LoadingType.BeddingShop,
            _                        => LoadingType.Normal,
        };
        LoadingManager.Instance.ShowLoading(loadingType);

        await UniTask.WhenAll(PopupManager.Instance.HideAllPopups(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));
        PanelManager.Instance.ShowPanel(PanelManager.Type.Shop);

        var mapType = shopType switch
        {
            ShopType.PowderShop => MapType.PowderShop,
            _                   => MapType.ShoppingSquare,
        };
        OnChangeMap(mapType);
        SetCameraSize(0.6f);
        ResetCamaraPosition();

        GameTimeManager.Instance.Resume();
        LoadingManager.Instance.HideLoading();
    }

    #endregion
}