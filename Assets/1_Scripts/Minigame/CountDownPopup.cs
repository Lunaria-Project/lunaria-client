using Lunaria;
using UnityEngine;

public struct CountDownPopupParameter : IPopupParameter
{
    public int CountDownSeconds { get; init; }
}

public class CountDownPopup : Popup<CountDownPopupParameter>
{
    [SerializeField] private Text _countDownText;

    private float _remainTime;

    private void Update()
    {
        _remainTime -= Time.deltaTime;
        _countDownText.SetText(Mathf.RoundToInt(_remainTime).ToString());
        if (_remainTime <= 0)
        {
            OnHideButtonClick();
        }
    }

    protected override void OnShow(CountDownPopupParameter parameter)
    {
        _remainTime = parameter.CountDownSeconds;
        _countDownText.SetText(parameter.CountDownSeconds.ToString());
    }

    protected override void OnHide() { }
}