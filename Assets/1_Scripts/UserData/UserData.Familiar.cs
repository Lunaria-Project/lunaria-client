using System;
using System.Collections.Generic;

public enum FamiliarState
{
    Idle,
    Summoned,
    Working,
}

public class FamiliarInfo
{
    public int FamiliarCallItemId;
    public int CurrentHp;
    public FamiliarState State;
    public MinigameType WorkingMinigameType;
    public long WorkEndGameSeconds;

    public int WorkRemainMinutes
    {
        get
        {
            if (State is not FamiliarState.Working) return 0;
            var remainSeconds = WorkEndGameSeconds - GameTimeManager.Instance.CurrentGameTime.TotalSeconds;
            return remainSeconds <= 0 ? 0 : TimeUtil.SecondsToMinutes(remainSeconds);
        }
    }
}

public partial class UserData // Familiar
{
    public event Action OnFamiliarChanged;

    public IReadOnlyList<FamiliarInfo> Familiars => _userDataInfo.Familiars;
    public int UnlockedFamiliarSlotCount => _userDataInfo.UnlockedFamiliarSlotCount;

    public bool TryAddFamiliar(int familiarCallItemId, bool showToastMessage = true)
    {
        if (!CanAddFamiliar(familiarCallItemId, showToastMessage)) return false;

        _userDataInfo.AddFamiliar(familiarCallItemId);
        OnFamiliarChanged?.Invoke();
        return true;
    }

    public bool CanAddFamiliar(int familiarCallItemId, bool showToastMessage = true)
    {
        if (UnlockedFamiliarSlotCount <= _userDataInfo.Familiars.Count)
        {
            LogManager.Log($"[Familiar] CanAddFamiliar: 패밀리어 슬롯이 가득 참 (familiarCallItemId={familiarCallItemId})");
            if (showToastMessage)
            {
                GlobalManager.Instance.ShowToastMessage(LocalizationKey.Familiar_ExcessCountWarning.Text());
            }
            return false;
        }

        var familiarId = GameData.Instance.GetFamiliarCallData(familiarCallItemId).FamiliarId;
        foreach (var familiar in Familiars)
        {
            if (GameData.Instance.GetFamiliarCallData(familiar.FamiliarCallItemId).FamiliarId != familiarId) continue;
            LogManager.Log($"[Familiar] CanAddFamiliar: 이미 보유한 패밀리어 (familiarCallItemId={familiarCallItemId})");
            if (showToastMessage)
            {
                GlobalManager.Instance.ShowToastMessage(LocalizationKey.Familiar_DuplicationWarning.Text());
            }
            return false;
        }

        return true;
    }

    private void UpdateFamiliarWork()
    {
        var isChanged = false;
        var currentSeconds = GameTimeManager.Instance.CurrentGameTime.TotalSeconds;
        foreach (var familiar in _userDataInfo.Familiars)
        {
            if (familiar.State is not FamiliarState.Working) continue;
            if (familiar.WorkEndGameSeconds > currentSeconds) continue;

            EndFamiliarWork(familiar);
            isChanged = true;
        }

        if (isChanged)
        {
            OnFamiliarChanged?.Invoke();
        }
    }

    private void RefreshFamiliarsOnNewDay()
    {
        // 일시적 패밀리어는 하루가 지나면 사라지고, 근무 중이던 패밀리어는 대기 상태로 돌아간다.
        _userDataInfo.Familiars.RemoveAll(familiar => GameData.Instance.GetFamiliarCallData(familiar.FamiliarCallItemId).IsTemporary);
        foreach (var familiar in _userDataInfo.Familiars)
        {
            if (familiar.State is not FamiliarState.Working) continue;
            EndFamiliarWork(familiar);
        }
        OnFamiliarChanged?.Invoke();
    }

    private static void EndFamiliarWork(FamiliarInfo familiar)
    {
        familiar.State = FamiliarState.Idle;
        familiar.WorkingMinigameType = default;
        familiar.WorkEndGameSeconds = 0;
    }
}