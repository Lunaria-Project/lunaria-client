using Generated;
using Lunaria;
using UnityEngine;

public class MyhomeMainPanel : Panel<MyhomeMainPanel>
{
    [Header("Wallet"), Space(8)]
    [SerializeField] Text _walletText;

    [Header("Time"), Space(8)]
    [SerializeField] RectTransform _timeBackground;
    [SerializeField] float _defaultTimeBackgroundZRotation;
    [SerializeField] Text _currentTimeText;
    [SerializeField] Text _currentTimeAMPMText;

    protected override void OnShow(params object[] args)
    {
        GameTimeManager.Instance.OnIntervalChanged -= OnTimeChanged;
        GameTimeManager.Instance.OnIntervalChanged += OnTimeChanged;
        GameTimeManager.Instance.OnTimeSecondsChanged -= OnTimeSecondsChanged;
        GameTimeManager.Instance.OnTimeSecondsChanged += OnTimeSecondsChanged;
        OnShowUI();
    }

    protected override void OnHide()
    {
        GameTimeManager.Instance.OnIntervalChanged -= OnTimeChanged;
        GameTimeManager.Instance.OnTimeSecondsChanged -= OnTimeSecondsChanged;
    }

    protected override void OnRefresh() { }

    private void OnShowUI()
    {
        _walletText.SetText(UserData.Instance.GetItemQuantity(ItemType.MainCoin).ToPrice());
        OnTimeChanged();
    }

    private void OnTimeSecondsChanged()
    {
        var normalizedTime = GameTimeManager.Instance.CurrentGameTime.TotalSeconds / (float)TimeUtil.SecondsPerDay;
        var rotationZ = normalizedTime * 360f + _defaultTimeBackgroundZRotation;
        _timeBackground.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    private void OnTimeChanged()
    {
        var currentGameTime = GameTimeManager.Instance.CurrentGameTime;
        _currentTimeText.SetText(TimeUtil.GameTimeToString(currentGameTime));
        //TODO(지선): 로컬키
        _currentTimeAMPMText.SetText(GameTimeManager.Instance.CurrentGameTime.IsAM ? "AM" : "PM");
    }
}