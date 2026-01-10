using Cysharp.Threading.Tasks;
using Generated;
using Lunaria;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyhomeMainPanel : Panel<MyhomeMainPanel>
{
    [SerializeField] Text _walletText;
    [SerializeField] MyhomeTimeUI _timeUI;
    [SerializeField] MyhomeArtifactUI _artifactUI;

    protected override void OnShow(params object[] args)
    {
        _timeUI.OnShow();
        _artifactUI.OnShow();
        OnShowUI();
    }

    protected override void OnHide()
    {
        _timeUI.OnHide();
        _artifactUI.OnHide();
    }

    protected override void OnRefresh() { }

    private void OnShowUI()
    {
        _walletText.SetText(UserData.Instance.GetItemQuantity(ItemType.MainCoin).ToPrice());
    }

    public void OnShoppingSquareButtonClick()
    {
        ShoppingSquareButtonClickAsync().Forget();
        return;

        async UniTask ShoppingSquareButtonClickAsync()
        {
            LoadingManager.Instance.ShowLoading();
            await UniTask.NextFrame();
            await UniTask.WhenAll(SceneManager.LoadSceneAsync("ShoppingSquareScene").ToUniTask(), UniTask.Delay(LoadingManager.DefaultLoadingAwaitMillis, ignoreTimeScale: true));
            LoadingManager.Instance.HideLoading();
        }
    }
}