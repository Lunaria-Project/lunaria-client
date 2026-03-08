using System;
using Lunaria;
using UnityEngine;

public struct PowderPortalMinigameResultPopupParameter : IPopupParameter
{
    public int Score { get; init; }
    public Action RetryAction { get; init; }
    public Action HideAction { get; init; }
}

public class PowderPortalMinigameResultPopup : Popup<PowderPortalMinigameResultPopupParameter>
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private GameObject _retryButton;

    private Action _retryAction;
    private Action _hideAction;

    protected override void OnShow(PowderPortalMinigameResultPopupParameter parameter)
    {
        _retryAction = parameter.RetryAction;
        _hideAction = parameter.HideAction;
        _scoreText.SetText(parameter.Score.ToString());
    }

    protected override void OnHide() { }

    public void OnRetryButtonClick()
    {
        _retryAction?.Invoke();
        OnHideButtonClick();
    }

    public void OnCloseButtonClick()
    {
        _hideAction?.Invoke();
        OnHideButtonClick();
    }
}
