using DG.Tweening;
using Lunaria;
using UnityEngine;

public class DailyReceiptPopup : EmptyParamPopup
{
    [SerializeField] private LayoutSwitcher _layoutSwitcher;
    [SerializeField] private Text _dayText;
    [SerializeField] private Text _totalPriceText;
    [SerializeField] private DOTweenAnimation _finishAnimation;
    
    private const string SimpleLayoutKey = "Simple";
    private const string DetailsLayoutKey = "Details";
    
    protected override void OnShow()
    {
        _layoutSwitcher.SetLayout(SimpleLayoutKey);
        _dayText.SetText(LocalizationKey.DayFormat.Text(UserData.Instance.CurrentDay.ToPrice()));
        _totalPriceText.SetText("0"); // TODO(지선)
        // TODO(지선): 섹션 매핑 확정 후 UserData.Instance.DailyMinigameRecords로 셀 채우기
    }

    protected override void OnHide() { }
    
    public void OnShowDetailButtonClick()
    {
        _layoutSwitcher.SetLayout(DetailsLayoutKey);
    }
    
    public void OnShowSimpleButtonClick()
    {
        // TODO(지선): 버튼 추가된 후 연결 필요
        _layoutSwitcher.SetLayout(SimpleLayoutKey);
    }
    
    public void OnFinishButtonClick()
    {
        _finishAnimation.DOPlay();
    }
}
