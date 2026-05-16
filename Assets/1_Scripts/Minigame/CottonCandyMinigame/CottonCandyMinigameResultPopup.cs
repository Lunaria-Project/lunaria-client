using System;
using Generated;
using Lunaria;
using UnityEngine;

public struct CottonCandyMinigameResultPopupParameter : IPopupParameter
{
    public int Score { get; init; }
    public Action RetryAction { get; init; }
    public Action HideAction { get; init; }
}

public class CottonCandyMinigameResultPopup : Popup<CottonCandyMinigameResultPopupParameter>
{
    [SerializeField] private Image _rewardImage;
    [SerializeField] private Text _rewardQuantityText;
    [SerializeField] private Text _scoreText;
    [SerializeField] private GameObject _retryButton; //TODO(지선)

    private Action _retryAction;
    private Action _hideAction;
    private (int Id, long Quantity) _reward;

    protected override void OnShow(CottonCandyMinigameResultPopupParameter parameter)
    {
        _retryAction = parameter.RetryAction;
        _hideAction = parameter.HideAction;
        _scoreText.SetText(parameter.Score.ToString());

        var minigameRewardData = GetMinigameRewardData();
        if (minigameRewardData == null) return;

        _reward = (minigameRewardData.RewardIds.GetAt(0), minigameRewardData.RewardQuantities.GetAt(0));
        var itemData = GameData.Instance.GetItemData(_reward.Id);
        _rewardImage.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
        _rewardQuantityText.SetText($"X{_reward.Quantity}"); //TODO(지선)
        return;

        MinigameRewardData GetMinigameRewardData()
        {
            foreach (var data in GameData.Instance.DTMinigameRewardData)
            {
                if (data.MinigameType != MinigameType.CottonCandy) continue;
                return data;
            }
            return null;
        }
    }

    protected override void OnHide()
    {
        UserData.Instance.AddReward(_reward.Id, _reward.Quantity);

        var infoData = GameData.Instance.GetMinigameInfoData(MinigameType.CottonCandy);
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
