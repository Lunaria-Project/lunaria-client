using Lunaria;
using UnityEngine;

public struct MinigameInfoPopupParameter : IPopupParameter
{
    public MinigameType MinigameType { get; init; }
}

public class MinigameInfoPopup : Popup<MinigameInfoPopupParameter>
{
    [SerializeField] private Text _timeText;

    protected override void OnShow(MinigameInfoPopupParameter parameter)
    {
        var infoData = GameData.Instance.GetMinigameInfoData(parameter.MinigameType);
        _timeText.SetText(TimeUtil.SecondsToMinuteSecondString(infoData.MinigameSeconds));
    }

    protected override void OnHide() { }
}
