using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShopMainPanel : Panel<ShopMainPanel>
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
}