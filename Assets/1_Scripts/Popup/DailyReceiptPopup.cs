using DG.Tweening;
using Lunaria;
using UnityEngine;

public class DailyReceiptPopup : EmptyParamPopup
{
    [SerializeField] private LayoutSwitcher _layoutSwitcher;
    [SerializeField] private Text _dayText;
    [SerializeField] private Text _totalPriceText;
    [SerializeField] private DOTweenAnimation _finishAnimation;
    [SerializeField] private DailyReceiptMinigameCell[] _minigameSimpleCells;
    [SerializeField] private DailyReceiptMinigameCell[] _minigameDetailCells;
    
    private const string SimpleLayoutKey = "Simple";
    private const string DetailsLayoutKey = "Details";
    
    protected override void OnShow()
    {
        _layoutSwitcher.SetLayout(SimpleLayoutKey);
        _dayText.SetText(LocalizationKey.DayFormat.Text(UserData.Instance.CurrentDay.ToPrice()));
        _totalPriceText.SetText("0"); // TODO(지선)
        RefreshMinigameCells();
    }

    protected override void OnHide() { }

    private void RefreshMinigameCells()
    {
        _minigameSimpleCells.SetActiveAll(false);
        _minigameDetailCells.SetActiveAll(false);

        var cellIndex = 0;
        foreach (var (minigameType, record) in UserData.Instance.DailyMinigameRecords)
        {
            foreach (var (itemId, quantity) in record.PlayerRewards)
            {
                if (!TryShowCells(cellIndex++, out var simpleCell, out var detailCell)) return;
                simpleCell.SetPlayerRewardData(minigameType, itemId, quantity);
                detailCell.SetPlayerRewardData(minigameType, itemId, quantity);
            }

            foreach (var (familiarCallItemId, itemId, quantity) in record.FamiliarRewards)
            {
                if (!TryShowCells(cellIndex++, out var simpleCell, out var detailCell)) return;
                simpleCell.SetFamiliarRewardData(minigameType, familiarCallItemId, itemId, quantity);
                detailCell.SetFamiliarRewardData(minigameType, familiarCallItemId, itemId, quantity);
            }
        }
    }

    private bool TryShowCells(int index, out DailyReceiptMinigameCell simpleCell, out DailyReceiptMinigameCell detailCell)
    {
        simpleCell = _minigameSimpleCells.GetAt(index);
        detailCell = _minigameDetailCells.GetAt(index);
        if (simpleCell == null || detailCell == null)
        {
            LogManager.LogError($"[DailyReceipt] Popup: 셀 개수 부족 (index={index}, simple={_minigameSimpleCells.Length}, detail={_minigameDetailCells.Length})");
            return false;
        }

        simpleCell.gameObject.SetActive(true);
        detailCell.gameObject.SetActive(true);
        return true;
    }
    
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
