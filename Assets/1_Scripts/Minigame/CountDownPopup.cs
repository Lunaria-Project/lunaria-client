using System;
using Lunaria;
using UnityEngine;

public struct CountDownPopupParameter : IPopupParameter
{
    public int CountDownSeconds { get; init; }
    public Action OnLastSecondAction { get; init; }
}

public class CountDownPopup : Popup<CountDownPopupParameter>
{
    private const float LastSecond = 1f;

    [SerializeField] private Text _countDownText;

    private float _remainTime;
    private Action _onLastSecondAction;

    private void Update()
    {
        _remainTime -= Time.deltaTime;
        _countDownText.SetText(Mathf.RoundToInt(_remainTime).ToString());
        if (_onLastSecondAction != null && _remainTime <= LastSecond)
        {
            var action = _onLastSecondAction;
            _onLastSecondAction = null;
            action.Invoke();
        }
        if (_remainTime <= 0)
        {
            OnHideButtonClick();
        }
    }

    protected override void OnShow(CountDownPopupParameter parameter)
    {
        _remainTime = parameter.CountDownSeconds;
        _onLastSecondAction = parameter.OnLastSecondAction;
        _countDownText.SetText(parameter.CountDownSeconds.ToString());
    }

    protected override void OnHide() { }
}