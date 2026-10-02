using System;
using Lunaria;
using UnityEngine;

public enum MinigameFamiliarSelectCellState
{
    Locked,
    Empty,
    Idle,
    Selected,
    CannotUse,
}

public class MinigameFamiliarSelectCell : MonoBehaviour
{
    [SerializeField] private LayoutSwitcher _layoutSwitcher;
    [SerializeField] private Image _familiarImage;
    [SerializeField] private Image _remainTimeFillImage;
    [SerializeField] private Text _remainTimeText;

    private const string IdleLayoutKey = "Idle";
    private const string SelectedLayoutKey = "Selected";
    private const string CannotUseLayoutKey = "CannotUse";
    private const string LockedLayoutKey = "Locked";
    private const string EmptyLayoutKey = "Empty";

    private const int FamiliarImageFrameNumber = 1;

    private int _index;
    private MinigameFamiliarSelectCellState _state;
    private Action<int> _onClickAction;

    public void SetClickAction(Action<int> onClickAction)
    {
        _onClickAction = onClickAction;
    }

    public void SetData(int index, MinigameFamiliarSelectCellState state, FamiliarInfo familiar)
    {
        _index = index;
        _state = state;
        _layoutSwitcher.SetLayout(GetLayoutKey(state));

        if (familiar == null) return;
        var familiarCallData = GameData.Instance.GetFamiliarCallData(familiar.FamiliarCallItemId);
        _familiarImage.SetSprite(ResourceManager.Instance.LoadFamiliarSprite(familiarCallData.ResourceKey, true, FamiliarImageFrameNumber));

        if (state is MinigameFamiliarSelectCellState.CannotUse)
        {
            RefreshRemainTime(familiar);
        }
    }

    private void RefreshRemainTime(FamiliarInfo familiar)
    {
        long remainSeconds;
        long totalSeconds;
        switch (familiar.State)
        {
            case FamiliarState.Working:
            {
                var durationHours = GameData.Instance.GetMinigameInfoData(familiar.WorkingMinigameType).FamiliarDurationHours;
                remainSeconds = UserData.Instance.GetWorkRemainSeconds(familiar);
                totalSeconds = durationHours * TimeUtil.SecondsPerHour;
                break;
            }
            case FamiliarState.NoEnergy:
            {
                remainSeconds = UserData.Instance.GetNoEnergyRemainSeconds(familiar);
                totalSeconds = GameSetting.Instance.FamiliarNoEnergyRecoveryHpHour * TimeUtil.SecondsPerHour;
                break;
            }
            default:
            {
                return;
            }
        }

        _remainTimeText.SetText(TimeUtil.SecondsToHourMinuteString(TimeUtil.CeilToTenMinuteInterval(remainSeconds)));
        _remainTimeFillImage.fillAmount = totalSeconds <= 0 ? 0 : (float)remainSeconds / totalSeconds;
    }

    public void OnButtonClick()
    {
        if (_state is not MinigameFamiliarSelectCellState.Idle) return;
        _onClickAction?.Invoke(_index);
    }

    private static string GetLayoutKey(MinigameFamiliarSelectCellState state)
    {
        return state switch
        {
            MinigameFamiliarSelectCellState.Locked    => LockedLayoutKey,
            MinigameFamiliarSelectCellState.Empty     => EmptyLayoutKey,
            MinigameFamiliarSelectCellState.Idle      => IdleLayoutKey,
            MinigameFamiliarSelectCellState.Selected  => SelectedLayoutKey,
            MinigameFamiliarSelectCellState.CannotUse => CannotUseLayoutKey,
            _                                         => EmptyLayoutKey,
        };
    }
}
