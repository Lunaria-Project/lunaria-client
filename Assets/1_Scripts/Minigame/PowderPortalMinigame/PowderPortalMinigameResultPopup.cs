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
    [SerializeField] private LayoutSwitcher _layoutSwitcher;
    [SerializeField] private Text _titleText;
    [SerializeField] private Image _rewardImage;
    [SerializeField] private Text _rewardQuantityText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _buttonText;
    [SerializeField] private GameObject _retryButton; //TODO(지선)

    private const string NormalLayoutKey = "Normal";
    private const string NoRewardLayoutKey = "NoReward";

    private Action _retryAction;
    private Action _hideAction;
    private (int Id, long Quantity) _reward;

    protected override void OnShow(PowderPortalMinigameResultPopupParameter parameter)
    {
        _retryAction = parameter.RetryAction;
        _hideAction = parameter.HideAction;
        _scoreText.SetText(parameter.Score.ToString());

        var minigameRewardData = GetMinigameRewardData();
        var hasReward = minigameRewardData != null;
        _layoutSwitcher.SetLayout(hasReward ? NormalLayoutKey : NoRewardLayoutKey);

        _titleText.SetText(hasReward ? LocalizationKey.MinigameResultPopup_Title.Text() : LocalizationKey.MinigameResultPopup_NoRewardTitle.Text());
        _descriptionText.SetText(hasReward ? LocalizationKey.PowderMinigameResultPopup_Description.Text() : LocalizationKey.PowderMinigameResultPopup_NoRewardDescription.Text());
        _buttonText.SetText(hasReward ? LocalizationKey.MinigameResultPopup_RewardButton.Text() : LocalizationKey.MinigameResultPopup_NoRewardButton.Text());

        if (!hasReward) return;

        _reward = (minigameRewardData.RewardIds.GetAt(0), minigameRewardData.RewardQuantities.GetAt(0));
        var itemData = GameData.Instance.GetItemData(_reward.Id);
        _rewardImage.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
        _rewardQuantityText.SetText($"X{_reward.Quantity}"); //TODO(지선)
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

        var infoData = GameData.Instance.GetMinigameInfoData(MinigameType.PowderPortal);
        GameTimeManager.Instance.AddHours(infoData.DurationHours);
    }
    
    protected override bool OnHideByEscapeKey()
    {
        OnHideButtonClick();
        return false;
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