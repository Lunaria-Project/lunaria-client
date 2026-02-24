using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

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
    private enum SceneType
    {
        Title = 0,
        Myhome = 1,
        ShoppingSquare = 2,
        Shop = 3,
    }

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
        //ClearCompassUI();
        GameTimeManager.Instance.Pause();
        LoadingManager.Instance.ShowLoading(LoadingType.Normal);

        await PopupManager.Instance.HideAllPopups();
        await UniTask.WhenAll(SceneManager.LoadSceneAsync((int)SceneType.ShoppingSquare).ToUniTask(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));
        PanelManager.Instance.ShowPanel(PanelManager.Type.ShoppingSquareMain);

        if (type != ShopType.None)
        {
            var mapManager = FindAnyObjectByType<ShoppingSquareMapManager>();
            mapManager.InitWithShopType(type);
        }
        SetCameraSize(1);

        GameTimeManager.Instance.Resume();
        LoadingManager.Instance.HideLoading();
    }

    private async UniTask GoToMyhomeAsync()
    {
        //ClearCompassUI();
        GameTimeManager.Instance.Pause();
        LoadingManager.Instance.ShowLoading(LoadingType.Normal);

        await PopupManager.Instance.HideAllPopups();
        await UniTask.WhenAll(SceneManager.LoadSceneAsync((int)SceneType.Myhome).ToUniTask(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));
        PanelManager.Instance.ShowPanel(PanelManager.Type.MyhomeMain);

        SetCameraSize(1);
        ResetCamaraPosition();

        GameTimeManager.Instance.Resume();
        LoadingManager.Instance.HideLoading();
    }

    private async UniTask GoToShopAsync(ShopType shopType)
    {
        //ClearCompassUI();
        GameTimeManager.Instance.Pause();
        var loadingType = shopType switch
        {
            ShopType.CottonCandyShop => LoadingType.CottonCandyShop,
            ShopType.PowderShop      => LoadingType.PowderShop,
            ShopType.BeddingShop     => LoadingType.BeddingShop,
            _                        => LoadingType.Normal,
        };
        LoadingManager.Instance.ShowLoading(loadingType);

        await PopupManager.Instance.HideAllPopups();
        await UniTask.WhenAll(SceneManager.LoadSceneAsync((int)SceneType.Shop).ToUniTask(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));
        PanelManager.Instance.ShowPanel(PanelManager.Type.Shop);

        SetCameraSize(0.6f);
        var mapManager = FindAnyObjectByType<ShopMapManager>();
        mapManager.Init(shopType);
        var cameraPosition = mapManager.GetCameraPosition(shopType).position;
        SetCamaraPosition(cameraPosition.x, cameraPosition.y);

        GameTimeManager.Instance.Resume();
        LoadingManager.Instance.HideLoading();
    }

    #endregion
}