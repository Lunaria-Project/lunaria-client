using Lunaria;
using UnityEngine;

public class TopTimeUI : MonoBehaviour
{
    [SerializeField] RectTransform _timeBackground;
    [SerializeField] float _defaultTimeBackgroundZRotation;
    [SerializeField] Text _currentTimeText;
    [SerializeField] Text _currentTimeAMPMText;
    
    public void OnShow()
    {
        GameTimeManager.Instance.OnIntervalChanged -= OnTimeChanged;
        GameTimeManager.Instance.OnIntervalChanged += OnTimeChanged;
        GameTimeManager.Instance.OnTimeSecondsChanged -= OnTimeSecondsChanged;
        GameTimeManager.Instance.OnTimeSecondsChanged += OnTimeSecondsChanged;
        OnTimeChanged();
    }

    public void OnHide()
    {
        GameTimeManager.Instance.OnIntervalChanged -= OnTimeChanged;
        GameTimeManager.Instance.OnTimeSecondsChanged -= OnTimeSecondsChanged;
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
        _currentTimeText.SetText(TimeUtil.GameTimeToStringForTopUI(currentGameTime));
        //TODO(지선): 로컬키
        _currentTimeAMPMText.SetText(GameTimeManager.Instance.CurrentGameTime.IsAM ? "AM" : "PM");
    }
}
