using System;
using System.Collections.Generic;

public class FamiliarInfo
{
    public int FamiliarCallId;
    public int Hp;
}

public partial class UserData // Familiar
{
    public event Action OnFamiliarChanged;

    public IReadOnlyList<FamiliarInfo> Familiars => _userDataInfo.Familiars;

    public bool TrySummonFamiliar(int familiarCallId)
    {
        if (!_userDataInfo.AddFamiliar(familiarCallId))
        {
            LogManager.Log($"[Familiar] TrySummonFamiliar: 패밀리어 슬롯이 가득 참 (familiarCallId={familiarCallId})");
            return false;
        }

        OnFamiliarChanged?.Invoke();
        return true;
    }
}
