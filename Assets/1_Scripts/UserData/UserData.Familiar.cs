using System;
using System.Collections.Generic;

public enum FamiliarState
{
    Idle,
    Summoned,
    Working,
    NoEnergy,
}

public class FamiliarInfo
{
    public int FamiliarCallItemId;
    public int CurrentHp;
    public FamiliarState State;
    public MinigameType WorkingMinigameType;
    public long WorkEndGameSeconds;
    public long LastHpUpdatedGameSeconds;

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

    private const float NoEnergyReleaseHpRatio = 0.5f;

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

    public void SummonFamiliar(FamiliarInfo familiar)
    {
        if (familiar.State is not FamiliarState.Idle) return;

        familiar.State = FamiliarState.Summoned;
        familiar.LastHpUpdatedGameSeconds = GameTimeManager.Instance.CurrentGameTime.TotalSeconds;
        ConsumeFamiliarHp(familiar, GameSetting.Instance.FamiliarSummonedStartHp);
        OnFamiliarChanged?.Invoke();
    }

    public void UnsummonFamiliar(FamiliarInfo familiar)
    {
        if (familiar.State is not FamiliarState.Summoned) return;

        familiar.State = FamiliarState.Idle;
        familiar.LastHpUpdatedGameSeconds = GameTimeManager.Instance.CurrentGameTime.TotalSeconds;
        OnFamiliarChanged?.Invoke();
    }

    public long GetNoEnergyRemainSeconds(FamiliarInfo familiar)
    {
        if (familiar.State is not FamiliarState.NoEnergy) return 0;

        var maxHp = GameData.Instance.GetFamiliarCallData(familiar.FamiliarCallItemId).MaxHp;
        var releaseHp = (int)Math.Ceiling(maxHp * NoEnergyReleaseHpRatio);
        var requiredHours = (int)Math.Ceiling((float)(releaseHp - familiar.CurrentHp) / GameSetting.Instance.FamiliarAutoRecoveryHpPerHour);
        var releaseGameSeconds = familiar.LastHpUpdatedGameSeconds + requiredHours * TimeUtil.SecondsPerHour;
        return Math.Max(0, releaseGameSeconds - GameTimeManager.Instance.CurrentGameTime.TotalSeconds);
    }

    private void UpdateFamiliarWork()
    {
        var isChanged = false;
        var currentSeconds = GameTimeManager.Instance.CurrentGameTime.TotalSeconds;
        foreach (var familiar in _userDataInfo.Familiars)
        {
            if (familiar.State is not FamiliarState.Working) continue;
            if (familiar.WorkEndGameSeconds > currentSeconds) continue;

            EndFamiliarWork(familiar, currentSeconds);
            isChanged = true;
        }

        if (isChanged)
        {
            OnFamiliarChanged?.Invoke();
        }
    }

    private void UpdateFamiliarHp()
    {
        var isChanged = false;
        var currentSeconds = GameTimeManager.Instance.CurrentGameTime.TotalSeconds;
        foreach (var familiar in _userDataInfo.Familiars)
        {
            if (familiar.State is FamiliarState.Working) continue;

            var elapsedHours = TimeUtil.SecondsToHours(currentSeconds - familiar.LastHpUpdatedGameSeconds);
            if (elapsedHours <= 0) continue;

            familiar.LastHpUpdatedGameSeconds += elapsedHours * TimeUtil.SecondsPerHour;
            if (familiar.State is FamiliarState.Summoned)
            {
                ConsumeFamiliarHp(familiar, elapsedHours * GameSetting.Instance.FamiliarSummonedConsumingHpPerHour);
            }
            else
            {
                RecoverFamiliarHp(familiar, elapsedHours * GameSetting.Instance.FamiliarAutoRecoveryHpPerHour);
            }
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

        var startSeconds = GameSetting.Instance.StartGameTimeSeconds;
        var sleepHours = TimeUtil.SecondsToHours(TimeUtil.SecondsPerDay - GameSetting.Instance.EndGameTimeSeconds + startSeconds);
        foreach (var familiar in _userDataInfo.Familiars)
        {
            if (familiar.State is FamiliarState.Working)
            {
                EndFamiliarWork(familiar, startSeconds);
            }

            RecoverFamiliarHp(familiar, sleepHours * GameSetting.Instance.FamiliarAutoRecoveryHpPerHour);
            familiar.LastHpUpdatedGameSeconds = startSeconds;
        }
        OnFamiliarChanged?.Invoke();
    }

    private static void ConsumeFamiliarHp(FamiliarInfo familiar, int amount)
    {
        familiar.CurrentHp = Math.Max(0, familiar.CurrentHp - amount);
        if (familiar.CurrentHp > 0) return;

        familiar.State = FamiliarState.NoEnergy;
    }

    private static void RecoverFamiliarHp(FamiliarInfo familiar, int amount)
    {
        var maxHp = GameData.Instance.GetFamiliarCallData(familiar.FamiliarCallItemId).MaxHp;
        familiar.CurrentHp = Math.Min(maxHp, familiar.CurrentHp + amount);

        if (familiar.State is not FamiliarState.NoEnergy) return;
        if (familiar.CurrentHp < maxHp * NoEnergyReleaseHpRatio) return;

        familiar.State = FamiliarState.Idle;
    }

    private static void EndFamiliarWork(FamiliarInfo familiar, long endGameSeconds)
    {
        familiar.State = FamiliarState.Idle;
        familiar.WorkingMinigameType = default;
        familiar.WorkEndGameSeconds = 0;
        familiar.LastHpUpdatedGameSeconds = endGameSeconds;
    }
}