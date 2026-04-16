using System;
using System.Collections.Generic;
using Generated;
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
    [SerializeField] private Image _rewardImage;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _resultText;
    [SerializeField] private GameObject _retryButton;

    private Action _retryAction;
    private Action _hideAction;
    private readonly List<(int, long)> _rewards = new();

    protected override void OnShow(SlimeMinigameResultPopupParameter parameter)
    {
        _retryAction = parameter.RetryAction;
        _hideAction = parameter.HideAction;

        var rewardScore = 0;
        foreach (var data in GameData.Instance.DTMinigameRewardData)
        {
            if (data.MinigameType != MinigameType.Slime) continue;
            if (parameter.Score < data.ScoreValue || data.ScoreValue < rewardScore) continue;
            rewardScore = data.ScoreValue;
            _rewards.Clear();
            var rewardCount = Mathf.Min(data.RewardIds.Count, data.RewardQuantities.Count);
            for (var i = 0; i < rewardCount; i++)
            {
                _rewards.Add((data.RewardIds[i], data.RewardQuantities[i]));
            }
        }

        var rewardData = GetGlueRewardData();

        _rewardImage.SetSprite(ResourceManager.Instance.LoadSprite(rewardData.IconResourceKey));
        _scoreText.SetText(LocalizationKey.Minigame_Slime_Score.Format(parameter.Score.ToPrice()));
        _resultText.SetText(LocalizationKey.SlimeMinigameResultPopup_Description.Format(rewardData.Name));
        _retryButton.SetActive(RequirementManager.Instance.IsSatisfied(RequirementType.MyhomeSlimeAppeared, null));
        return;

        ItemData GetGlueRewardData()
        {
            foreach (var (id, _) in _rewards)
            {
                var itemData = GameData.Instance.GetItemData(id);
                if (itemData.ItemType != ItemType.Glue) continue;
                return itemData;
            }
            return null;
        }
    }

    protected override void OnHide()
    {
        UserData.Instance.AddRewards(_rewards);

        var infoData = GameData.Instance.GetMinigameInfoData(MinigameType.Slime);
        GameTimeManager.Instance.AddHours(infoData.DurationHours);
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