using Cysharp.Threading.Tasks;
using UnityEngine;

public class LunariaDefaultPanel : Panel<LunariaDefaultPanel>
{
    [SerializeField] private TopWalletUI _walletUI;
    [SerializeField] private TopTimeUI _timeUI;
    [SerializeField] private MyhomeArtifactUI _artifactUI;
    [SerializeField] private InventoryQuickBlock _quickBlock;
    [SerializeField] private GameObject _shoppingSquareButton;

    protected void Awake()
    {
        _quickBlock.SetClickAction(OnQuickSlotClick);
    }

    protected override void OnShow(params object[] args)
    {
        var mapType = (MapType)args.GetAtWithError(0);

        _timeUI.OnShow();
        _artifactUI.OnShow();
        _walletUI.Refresh();

        _quickBlock.Init();

        _shoppingSquareButton.SetActive(mapType == MapType.Myhome);
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