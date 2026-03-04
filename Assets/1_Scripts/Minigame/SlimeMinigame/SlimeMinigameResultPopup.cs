using System;
using Cysharp.Threading.Tasks;
using Lunaria;
using UnityEngine;

public struct SlimeMinigameResultPopupParameter : IPopupParameter
{
    public int Score { get; init; }
    public Action RetryAction { get; init; }
    public Action HideAction { get; init; }
}

public class SlimeMinigameResultPopup : Popup<SlimeMinigameResultPopupParameter>
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _resultText;
    [SerializeField] private GameObject _retryButton;

    private Action _retryAction;
    private Action _hideAction;

    protected override void OnShow(SlimeMinigameResultPopupParameter parameter)
    {
        _retryAction = parameter.RetryAction;
        _hideAction = parameter.HideAction;

        _scoreText.SetText($@"{parameter.Score.ToPrice()}방울"); // TODO
        _resultText.SetText("여기 작업은 나중에");
        _retryButton.SetActive(RequirementManager.Instance.IsSatisfied(RequirementType.MyhomeSlimeAppeared, null));
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