using Cysharp.Threading.Tasks;
using UnityEngine;

public class LunariaDefaultPanel : Panel<LunariaDefaultPanel>
{
    [SerializeField] TopWalletUI _walletUI;
    [SerializeField] TopTimeUI _timeUI;
    [SerializeField] MyhomeArtifactUI _artifactUI;
    [SerializeField] private InventoryQuickBlock _quickBlock;

    protected void Awake()
    {
        _quickBlock.SetClickAction(OnQuickSlotClick);
    }

    protected override void OnShow(params object[] args)
    {
        _timeUI.OnShow();
        _artifactUI.OnShow();
        _walletUI.Refresh();
        
        _quickBlock.Init();
    }

    protected override void OnHide()
    {
        _timeUI.OnHide();
        _artifactUI.OnHide();
    }

    public void OnShoppingSquareButtonClick()
    {
        GlobalManager.Instance.ShortcutInvoke(ShortcutType.ShoppingSquare).Forget();
    }

    private void OnQuickSlotClick(int index) { }
}