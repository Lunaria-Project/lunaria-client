using Generated;
using Lunaria;
using UnityEngine;

public class MyhomeMainPanel : Panel<MyhomeMainPanel>
{
    [SerializeField] Text _walletText;
    
    protected override void OnShow(params object[] args)
    {
        OnShowUI();
    }

    protected override void OnHide()
    {
    }

    protected override void OnRefresh()
    {
    }

    private void OnShowUI()
    {
        _walletText.SetText(UserData.Instance.GetItemQuantity(ItemType.MainCoin).ToPrice());
    }
}
