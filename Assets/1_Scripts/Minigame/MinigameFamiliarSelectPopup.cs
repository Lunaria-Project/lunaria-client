using System.Collections.Generic;
using Lunaria;
using UnityEngine;

public struct MinigameFamiliarSelectPopupParameter : IPopupParameter
{
    public MinigameType MinigameType { get; init; }
    public PopupManager.Type ReadyPopupType { get; init; }
}

public class MinigameFamiliarSelectPopup : Popup<MinigameFamiliarSelectPopupParameter>
{
    [SerializeField] private MinigameFamiliarSelectCell[] _cells;
    [SerializeField] private Text _workingTimeText;

    private const int NoSelectedIndex = -1;

    private MinigameType _minigameType;
    private PopupManager.Type _readyPopupType;
    private int _selectedIndex = NoSelectedIndex;

    protected void Awake()
    {
        foreach (var cell in _cells)
        {
            cell.SetClickAction(OnCellClick);
        }
    }

    protected override void OnShow(MinigameFamiliarSelectPopupParameter parameter)
    {
        _minigameType = parameter.MinigameType;
        _readyPopupType = parameter.ReadyPopupType;
        _selectedIndex = NoSelectedIndex;

        var infoData = GameData.Instance.GetMinigameInfoData(_minigameType);
        _workingTimeText.SetText(LocalizationKey.FamiliarSelectPopup_WorkingTimeMessage.Text(infoData.FamiliarDurationHours));
        Refresh();
    }

    protected override void OnHide() { }

    private void Refresh()
    {
        var unlockedSlotCount = UserData.Instance.UnlockedFamiliarSlotCount;
        var familiars = UserData.Instance.Familiars;
        for (var i = 0; i < _cells.Length; i++)
        {
            var familiar = i < familiars.Count ? familiars[i] : null;
            _cells[i].SetData(i, GetCellState(i, _selectedIndex, unlockedSlotCount, familiars), familiar);
        }
    }

    private void OnCellClick(int index)
    {
        _selectedIndex = index;
        Refresh();
    }

    public void OnConfirmButtonClick()
    {
        if (_selectedIndex == NoSelectedIndex)
        {
            GlobalManager.Instance.ShowToastMessage(LocalizationKey.FamiliarSelectPopup_NotSelectMessage.Text());
            return;
        }

        UserData.Instance.StartFamiliarWork(UserData.Instance.Familiars[_selectedIndex], _minigameType);
        OnHideButtonClick();
    }

    public void OnCancelButtonClick()
    {
        OnHideButtonClick();
        PopupManager.Instance.ShowPopupWithEmptyParameter(_readyPopupType);
    }

    private static MinigameFamiliarSelectCellState GetCellState(int slotIndex, int selectedIndex, int unlockedSlotCount, IReadOnlyList<FamiliarInfo> familiars)
    {
        if (slotIndex >= unlockedSlotCount) return MinigameFamiliarSelectCellState.Locked;
        if (slotIndex >= familiars.Count) return MinigameFamiliarSelectCellState.Empty;
        if (!UserData.Instance.CanStartFamiliarWork(familiars[slotIndex])) return MinigameFamiliarSelectCellState.CannotUse;

        return slotIndex == selectedIndex ? MinigameFamiliarSelectCellState.Selected : MinigameFamiliarSelectCellState.Idle;
    }
}
