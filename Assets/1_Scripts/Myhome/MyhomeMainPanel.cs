using Generated;
using Lunaria;
using UnityEngine;

public class MyhomeMainPanel : Panel<MyhomeMainPanel>
{
    [SerializeField] Text _walletText;
    [SerializeField] Text _currentTimeText;
    [SerializeField] Text _currentTimeAMPMText;

    protected override void OnShow(params object[] args)
    {
        GameTimeManager.Instance.OnIntervalChanged -= OnTimeChanged;
        GameTimeManager.Instance.OnIntervalChanged += OnTimeChanged;
        OnShowUI();
    }

    protected override void OnHide()
    {
        GameTimeManager.Instance.OnIntervalChanged -= OnTimeChanged;
    }

    protected override void OnRefresh() { }

    private void OnShowUI()
    {
        _walletText.SetText(UserData.Instance.GetItemQuantity(ItemType.MainCoin).ToPrice());
        OnTimeChanged();
    }

    private void OnTimeChanged()
    {
        var currentGameTime = GameTimeManager.Instance.CurrentGameTime;
        _currentTimeText.SetText(TimeUtil.GameTimeToString(currentGameTime));
        //TODO(지선): 로컬키
        _currentTimeAMPMText.SetText(GameTimeManager.Instance.CurrentGameTime.IsAM ? "AM" : "PM");
    }
}