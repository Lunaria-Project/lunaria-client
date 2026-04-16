using System.Collections.Generic;
using Lunaria;
using UnityEngine;

public class InventoryPopup : EmptyParamPopup
{
    [SerializeField] private InventoryCell[] _cells;
    [SerializeField] private TabGroup _tabGroup;
    [SerializeField] private GameObject _emptyBlock;
    [SerializeField] private InventoryInfoCell _infoCell;

    [SerializeField] private InventoryQuickBlock _quickBlock;
    [SerializeField] private Image _dragImage;
    [SerializeField] private RectTransform _dragImageRect;

    private readonly List<(int ItemId, long Quantity)> _filteredItems = new();
    private InventoryTabType _filterTabType;
    private bool _isDragging;
    private int _draggingItemId;
    private int _selectedItemId;

    protected void Awake()
    {
        _tabGroup.SetTabChangedAction(OnTabChanged);
        foreach (var cell in _cells)
        {
            cell.SetClickAction(OnCellClick);
            cell.SetDragAction(OnCellBeginDrag, OnCellEndDrag);
        }
        _quickBlock.SetClickAction(OnQuickSlotClick);
        _quickBlock.SetDragAction(OnQuickSlotBeginDrag, OnCellEndDrag, OnQuickSlotDrop);
    }

    protected void Update()
    {
        if (!_isDragging) return;
        _dragImageRect.position = Input.mousePosition;
    }

    protected override void OnShow()
    {
        var maxSlotCount = GameSetting.Instance.MaxInventorySlotCount;
        LogManager.Assert(_cells.Length >= maxSlotCount, $"InventoryPopup: Cell count({_cells.Length}) must be >= MaxInventorySlotCount({maxSlotCount})");

        _tabGroup.Init();
        _quickBlock.Init();
    }

    protected override void OnHide() { }

    private void OnCellClick(int index)
    {
        if (index < 0 || index >= _filteredItems.Count) return;

        _selectedItemId = _filteredItems[index].ItemId;
        _infoCell.SetData(_selectedItemId);
        RefreshCells();
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
        LogManager.Assert(_filteredItems.Count <= unlockedSlotCount, $"InventoryPopup: FilteredItem count({_filteredItems.Count}) exceeds UnlockedSlotCount({unlockedSlotCount})");

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
                _cells[i].SetData(i, item.ItemId, item.Quantity, _selectedItemId == item.ItemId);
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

    private void OnCellBeginDrag(InventoryCell cell)
    {
        _draggingItemId = cell.ItemDataId;
        var itemData = GameData.Instance.GetItemData(_draggingItemId);
        _dragImage.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
        _dragImage.SetActive(true);
        _isDragging = true;
    }

    private void OnQuickSlotBeginDrag(int itemId)
    {
        _draggingItemId = itemId;
        var itemData = GameData.Instance.GetItemData(_draggingItemId);
        _dragImage.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
        _dragImage.SetActive(true);
        _isDragging = true;
    }

    private void OnCellEndDrag()
    {
        _dragImage.SetActive(false);
        _isDragging = false;
        _draggingItemId = 0;
    }

    private void OnQuickSlotDrop(int slotIndex)
    {
        if (_draggingItemId == 0) return;
        UserData.Instance.SetQuickSlot(slotIndex, _draggingItemId);
        _quickBlock.Refresh();
    }

    private void OnQuickSlotClick(int slotIndex)
    {
        var itemId = UserData.Instance.GetQuickSlotItemId(slotIndex);
        if (itemId == 0) return;
        UserData.Instance.ClearQuickSlot(slotIndex);
        _quickBlock.Refresh();
    }
}