using System.Collections.Generic;
using UnityEngine;

public class FamiliarSlotGroup : MonoBehaviour
{
    [SerializeField] private FamiliarSlot[] _slots;

    public void OnShow()
    {
        UserData.Instance.OnFamiliarChanged -= Refresh;
        UserData.Instance.OnFamiliarChanged += Refresh;
        GameTimeManager.Instance.OnIntervalChanged -= Refresh;
        GameTimeManager.Instance.OnIntervalChanged += Refresh;
        Refresh();
    }

    public void OnHide()
    {
        UserData.Instance.OnFamiliarChanged -= Refresh;
        GameTimeManager.Instance.OnIntervalChanged -= Refresh;
    }

    private void Refresh()
    {
        var unlockedSlotCount = UserData.Instance.UnlockedFamiliarSlotCount;
        var familiars = UserData.Instance.Familiars;
        for (var i = 0; i < _slots.Length; i++)
        {
            var familiar = i < familiars.Count ? familiars[i] : null;
            _slots[i].SetData(GetSlotState(i, unlockedSlotCount, familiars), familiar);
        }
    }

    private static FamiliarSlotState GetSlotState(int slotIndex, int unlockedSlotCount, IReadOnlyList<FamiliarInfo> familiars)
    {
        if (slotIndex >= unlockedSlotCount) return FamiliarSlotState.Locked;
        if (slotIndex >= familiars.Count) return FamiliarSlotState.Empty;

        return familiars[slotIndex].State switch
        {
            FamiliarState.Summoned => FamiliarSlotState.Summoned,
            FamiliarState.Working  => FamiliarSlotState.Working,
            FamiliarState.NoEnergy => FamiliarSlotState.NoEnergy,
            _                      => FamiliarSlotState.Idle,
        };
    }
}
