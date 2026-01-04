using Generated;
using Lunaria;
using UnityEngine;

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
        
    }
}