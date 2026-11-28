using Lunaria;
using UnityEngine;

public struct ArtifactCraftPaymentPopupParameter : IPopupParameter
{
    public long RequiredCoinQuantity { get; init; }
}

public class ArtifactCraftPaymentPopup : Popup<ArtifactCraftPaymentPopupParameter>
{
    [SerializeField] private Text[] _coinTexts;
    [SerializeField] private LayoutSwitcher _coinLayoutSwitcher;
    [SerializeField] private LayoutSwitcher _cardLayoutSwitcher;

    private const string CoinAbleLayoutKey = "CoinAble";
    private const string CoinDisableLayoutKey = "CoinDisable";
    private const string CardAbleLayoutKey = "CardAble";
    private const string CardDisableLayoutKey = "CardDisable";

    protected override void OnShow(ArtifactCraftPaymentPopupParameter parameter)
    {
        var requiredCoinQuantity = parameter.RequiredCoinQuantity;
        _coinTexts.SetTexts(requiredCoinQuantity.ToPrice());

        var hasEnoughCoin = UserData.Instance.GetMainCoinCount() >= requiredCoinQuantity;
        _coinLayoutSwitcher.SetLayout(hasEnoughCoin ? CoinAbleLayoutKey : CoinDisableLayoutKey);

        // TODO(지선): 카드 결제 작업 전까지 항상 비활성
        _cardLayoutSwitcher.SetLayout(CardDisableLayoutKey);
    }

    protected override void OnHide() { }

    public void OnCoinPaymentButtonClick()
    {
        // TODO(지선): 코인 결제 처리
    }

    public void OnCardPaymentButtonClick()
    {
        // TODO(지선): 카드 결제 처리
    }
}
