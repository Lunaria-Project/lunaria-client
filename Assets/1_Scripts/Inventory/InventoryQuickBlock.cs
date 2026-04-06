using System;
using UnityEngine;

public class InventoryQuickBlock : MonoBehaviour
{
    [SerializeField] private InventoryQuickSlot[] _quickSlots;

    public void SetClickAction(Action<int> onClickAction)
    {
        foreach (var quickSlot in _quickSlots)
        {
            quickSlot.SetClickAction(onClickAction);
        }
    }

    public void SetDragAction(Action<int> onBeginDrag, Action onEndDrag, Action<int> onDropAction)
    {
        foreach (var quickSlot in _quickSlots)
        {
            quickSlot.SetDragAction(onBeginDrag, onEndDrag, onDropAction);
        }
    }

    public void Init()
    {
        var maxSlotCount = GameSetting.Instance.MaxQuickSlotCount;
        LogManager.Assert(_quickSlots.Length >= maxSlotCount, $"InventoryQuickBlock: QuickSlot count({_quickSlots.Length}) must be >= MaxQuickSlotCount({maxSlotCount})");

        for (var i = 0; i < _quickSlots.Length; i++)
        {
            _quickSlots[i].gameObject.SetActive(i < maxSlotCount);
            _quickSlots[i].SetSlotIndex(i);
        }
        Refresh();
    }

    public void Refresh()
    {
        var unlockedCount = UserData.Instance.UnlockedQuickSlotCount;
        var maxSlotCount = GameSetting.Instance.MaxQuickSlotCount;

        for (var i = 0; i < maxSlotCount; i++)
        {
            if (i >= unlockedCount)
            {
                _quickSlots[i].SetLocked(true);
                continue;
            }

            _quickSlots[i].SetLocked(false);
            _quickSlots[i].Refresh();
        }
    }
}
