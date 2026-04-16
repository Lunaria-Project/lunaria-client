using System;
using Generated;
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
    [SerializeField] private Image _rewardImage;
    [SerializeField] private Text _rewardQuantityText;
    [SerializeField] private Text _scoreText;
    [SerializeField] private GameObject _retryButton;

    private Action _retryAction;
    private Action _hideAction;
    private (int Id, long Quantity) _reward;

    protected override void OnShow(PowderPortalMinigameResultPopupParameter parameter)
    {
        _retryAction = parameter.RetryAction;
        _hideAction = parameter.HideAction;
        _scoreText.SetText(parameter.Score.ToString());

        var minigameRewardData = GetMinigameRewardData();
        if (minigameRewardData == null) return;

        _reward = (minigameRewardData.RewardIds.GetAt(0), minigameRewardData.RewardQuantities.GetAt(0));
        var itemData = GameData.Instance.GetItemData(_reward.Id);
        _rewardImage.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
        _rewardQuantityText.SetText($"X{_reward.Quantity}");
        return;

        MinigameRewardData GetMinigameRewardData()
        {
            foreach (var data in GameData.Instance.DTMinigameRewardData)
            {
                if (data.MinigameType != MinigameType.PowderPortal) continue;
                return data;
            }
            return null;
        }
    }

    protected override void OnHide()
    {
        UserData.Instance.AddReward(_reward.Id, _reward.Quantity);
    }

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