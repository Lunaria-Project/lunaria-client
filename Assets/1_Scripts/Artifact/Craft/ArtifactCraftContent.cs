using System.Collections.Generic;
using Generated;
using Lunaria;
using UnityEngine;

public class ArtifactCraftContent : MonoBehaviour
{
    [SerializeField] private Text _unlockCountText;
    [SerializeField] private ArtifactCraftTab[] _tabs;

    private readonly List<ArtifactCraftData> _craftDataList = new();
    private int _selectedCraftId;

    protected void Awake()
    {
        foreach (var tab in _tabs)
        {
            tab.SetClickAction(OnTabClick);
        }
    }

    public void OnShow()
    {
        gameObject.SetActive(true);

        _craftDataList.Clear();
        _craftDataList.AddRange(GameData.Instance.DTArtifactCraftData.Values);
        _craftDataList.Sort((left, right) => left.Order.CompareTo(right.Order));
        LogManager.Assert(_tabs.Length >= _craftDataList.Count, $"[ArtifactCraftContent] OnShow: Tab count({_tabs.Length}) must be >= craft data count({_craftDataList.Count})");

        _selectedCraftId = _craftDataList.GetAt(0)?.Id ?? 0;
        RefreshTabs();
    }

    public void OnHide()
    {
        foreach (var craftData in _craftDataList)
        {
            if (!RequirementManager.Instance.IsSatisfied(craftData.RequirementType, craftData.RequirementValues)) continue;
            UserData.Instance.CheckNewArtifactCraft(craftData.Id);
        }

        gameObject.SetActive(false);
    }

    private void RefreshTabs()
    {
        var unlockedCount = 0;

        _tabs.SetActiveAll(false);
        for (var i = 0; i < _craftDataList.Count && i < _tabs.Length; i++)
        {
            var craftData = _craftDataList.GetAt(i);
            var isUnlocked = RequirementManager.Instance.IsSatisfied(craftData.RequirementType, craftData.RequirementValues);
            if (isUnlocked)
            {
                unlockedCount++;
            }

            var tab = _tabs.GetAt(i);
            tab.SetActive(true);
            tab.SetData(craftData, isUnlocked, craftData.Id == _selectedCraftId);
        }

        _unlockCountText.SetText($"{unlockedCount}/{_craftDataList.Count}");
    }

    private void OnTabClick(int craftId)
    {
        _selectedCraftId = craftId;
        RefreshTabs();
    }
}
