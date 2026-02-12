using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public enum ShortcutType
{
    Myhome,
    ShoppingSquare,
}

public partial class GlobalManager
{
    private enum SceneType
    {
        Title = 0,
        Myhome = 1,
        ShoppingSquare = 2,
    }

    public UniTask ShortcutInvoke(ShortcutType type)
    {
        switch (type)
        {
            case ShortcutType.Myhome: return GoToMyhomeAsync();
            case ShortcutType.ShoppingSquare: return GoToShoppingSquareAsync();
        }
        return UniTask.CompletedTask;
    }

    private async UniTask GoToShoppingSquareAsync()
    {
        GameTimeManager.Instance.Pause();
        LoadingManager.Instance.ShowLoading();
        await PopupManager.Instance.HideAllPopups();
        PanelManager.Instance.ShowPanel(PanelManager.Type.ShoppingSquareMain);
        await UniTask.WhenAll(SceneManager.LoadSceneAsync((int)SceneType.ShoppingSquare).ToUniTask(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));
        FollowPlayer = true;
        GlobalCamera.orthographicSize = 250;
        GameTimeManager.Instance.Resume();
        LoadingManager.Instance.HideLoading();
    }

    private async UniTask GoToMyhomeAsync()
    {
        GameTimeManager.Instance.Pause();
        LoadingManager.Instance.ShowLoading();
        await PopupManager.Instance.HideAllPopups();
        PanelManager.Instance.ShowPanel(PanelManager.Type.MyhomeMain);
        await UniTask.WhenAll(SceneManager.LoadSceneAsync((int)SceneType.Myhome).ToUniTask(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));
        FollowPlayer = false;
        GlobalCamera.orthographicSize = 540;
        ResetCamaraPosition();
        GameTimeManager.Instance.Resume();
        LoadingManager.Instance.HideLoading();
    }
}