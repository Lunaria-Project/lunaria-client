using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyhomeMainPanel : Panel<MyhomeMainPanel>
{
    [SerializeField] TopWalletUI _walletUI;
    [SerializeField] TopTimeUI _timeUI;
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
        _walletUI.Refresh();
    }

    public void OnShoppingSquareButtonClick()
    {
        GlobalManager.Instance.GoToShoppingSquareAsync().Forget();
    }
}