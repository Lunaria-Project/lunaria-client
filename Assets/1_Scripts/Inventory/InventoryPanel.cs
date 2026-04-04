using System.Collections.Generic;
using Lunaria;
using UnityEngine;

public class InventoryPanel : Panel<InventoryPanel>
{
    [SerializeField] private InventoryCell[] _cells;
    [SerializeField] private TabGroup _tabGroup;
    [SerializeField] private GameObject _emptyBlock;
    [SerializeField] private InventoryInfoCell _infoCell;

    [SerializeField] TopWalletUI _walletUI;
    [SerializeField] TopTimeUI _timeUI;
    [SerializeField] MyhomeArtifactUI _artifactUI;

    private readonly List<(int ItemId, long Quantity)> _filteredItems = new();
    private InventoryTabType _filterTabType;

    private void Awake()
    {
        _tabGroup.SetTabChangedAction(OnTabChanged);
        foreach (var cell in _cells)
        {
            cell.SetClickAction(OnCellClick);
        }
    }

    protected override void OnShow(params object[] args)
    {
        var maxSlotCount = GameSetting.Instance.MaxInventorySlotCount;
        LogManager.Assert(_cells.Length >= maxSlotCount, $"InventoryPanel: Cell count({_cells.Length}) must be >= MaxInventorySlotCount({maxSlotCount})");

        _tabGroup.Init();

        _timeUI.OnShow();
        _artifactUI.OnShow();
        _walletUI.Refresh();
    }

    protected override void OnHide() { }

    public void OnSortButtonClick()
    {
        UserData.Instance.SortInventory();
        RefreshCells();
    }

    private void OnCellClick(int index)
    {
        if (index < 0 || index >= _filteredItems.Count) return;
        _infoCell.SetData(_filteredItems[index].ItemId);
    }

    private void OnTabChanged(int tabIndex)
    {
        _filterTabType = tabIndex == 0 ? InventoryTabType.None : (InventoryTabType)tabIndex;
        RefreshCells();
        OnCellClick(0);
    }

    private void RefreshCells()
    {
        var maxSlotCount = GameSetting.Instance.MaxInventorySlotCount;
        var unlockedSlotCount = UserData.Instance.UnlockedInventorySlotCount;

        FilterItems();
        LogManager.Assert(_filteredItems.Count <= unlockedSlotCount, $"InventoryPanel: FilteredItem count({_filteredItems.Count}) exceeds UnlockedSlotCount({unlockedSlotCount})");

        _emptyBlock.SetActive(_filterTabType != InventoryTabType.None && _filteredItems.Count == 0);

        var isAllTab = _filterTabType == InventoryTabType.None;
        var visibleSlotCount = isAllTab ? maxSlotCount : _filteredItems.Count;

        _cells.SetActiveAll(false);
        for (var i = 0; i < _cells.Length; i++)
        {
            if (i >= visibleSlotCount) continue;
            _cells[i].gameObject.SetActive(true);
            _cells[i].SetLocked(isAllTab && i >= unlockedSlotCount);

            if (i < _filteredItems.Count)
            {
                var item = _filteredItems[i];
                _cells[i].SetData(i, item.ItemId, item.Quantity);
            }
            else
            {
                _cells[i].SeyEmpty();
            }
        }
    }

    private void FilterItems()
    {
        _filteredItems.Clear();
        var itemList = UserData.Instance.ItemList;

        foreach (var item in itemList)
        {
            var itemData = GameData.Instance.GetItemData(item.ItemId);
            if (!GameData.Instance.TryGetInventoryTabData(itemData.ItemType, out var tabData)) continue;

            if (_filterTabType == InventoryTabType.None || tabData.InventoryTabType == _filterTabType)
            {
                _filteredItems.Add(item);
            }
        }
    }
}