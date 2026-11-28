using System;
using System.Collections.Generic;
using DG.Tweening;
using Generated;
using Lunaria;
using UnityEngine;

public class ArtifactCraftContent : MonoBehaviour
{
    [Serializable]
    private class FlyingThumbnail
    {
        public ItemThumbnail Thumbnail;
        public RectTransform RectTransform;
    }

    [SerializeField] private Text _unlockCountText;
    [SerializeField] private ArtifactCraftTab[] _tabs;
    [SerializeField] private ArtifactCraftMaterialListCell[] _materialListCells;
    [SerializeField] private GameObject _materialEmptyObject;
    [SerializeField] private GameObject _materialExistObject;
    [SerializeField] private GameObject _tooltipObject;
    [SerializeField] private ArtifactCraftInputSlot[] _inputSlots;
    [SerializeField] private DropArea _dropArea;
    [SerializeField] private FlyingThumbnail[] _flyingThumbnails;
    [SerializeField] private GameObject _autoFillButtonObject;
    [SerializeField] private GameObject _autoFillDisabledObject;
    [SerializeField] private ArtifactCraftHandle _handle;

    private const float FlyDurationSeconds = 0.3f;
    private const float FlyIntervalSeconds = 0.1f;

    private readonly List<ArtifactCraftData> _craftDataList = new();
    private readonly List<(int ItemId, long Quantity)> _materialItems = new();
    private readonly HashSet<int> _filledMaterialIds = new();
    private int _selectedCraftId;
    private ArtifactCraftMaterialListCell _tooltipCell;
    private int _draggingItemId;
    private Sequence _autoFillSequence;

    private FlyingThumbnail DragThumbnail => _flyingThumbnails.GetAt(0);

    protected void Awake()
    {
        foreach (var tab in _tabs)
        {
            tab.SetClickAction(OnTabClick);
        }
        foreach (var cell in _materialListCells)
        {
            cell.SetTooltipAction(OnMaterialTooltipShow, OnMaterialTooltipHide);
            cell.SetDragAction(OnMaterialBeginDrag, OnMaterialEndDrag);
        }
        _dropArea.SetDropAction(OnMaterialDrop);
        _handle.SetPulledAction(OnHandlePulled);
    }

    protected void Update()
    {
        if (_draggingItemId == 0) return;
        if (!GlobalManager.Instance.TryGetMouseWorldPosition(DragThumbnail.RectTransform, out var dragWorldPosition)) return;
        DragThumbnail.RectTransform.position = dragWorldPosition;
    }

    public void OnShow()
    {
        gameObject.SetActive(true);

        _craftDataList.Clear();
        _craftDataList.AddRange(GameData.Instance.DTArtifactCraftData.Values);
        _craftDataList.Sort((left, right) => left.Order.CompareTo(right.Order));
        LogManager.Assert(_tabs.Length >= _craftDataList.Count, $"[ArtifactCraftContent] OnShow: Tab count({_tabs.Length}) must be >= craft data count({_craftDataList.Count})");

        _selectedCraftId = _craftDataList.GetAt(0)?.Id ?? 0;
        _filledMaterialIds.Clear();
        _draggingItemId = 0;
        RefreshTabs();
        RefreshMaterialList();
        RefreshInputSlots();

        _tooltipCell = null;
        _tooltipObject.SetActive(false);
        foreach (var flyingThumbnail in _flyingThumbnails)
        {
            flyingThumbnail.Thumbnail.SetActive(false);
        }
    }

    public void OnHide()
    {
        if (_autoFillSequence != null)
        {
            _autoFillSequence.Kill();
            _autoFillSequence = null;
            InputManager.Instance.Unblock(this);
        }

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

    private void RefreshMaterialList()
    {
        _materialItems.Clear();
        _materialItems.AddRange(UserData.Instance.GetItemQuantities(ItemType.RefinedMaterial));
        _materialItems.Sort((left, right) =>
        {
            var leftData = GameData.Instance.GetItemData(left.ItemId);
            var rightData = GameData.Instance.GetItemData(right.ItemId);
            return leftData.Order.CompareTo(rightData.Order);
        });
        LogManager.Assert(_materialListCells.Length >= _materialItems.Count, $"[ArtifactCraftContent] RefreshMaterialList: MaterialListCell count({_materialListCells.Length}) must be >= material item count({_materialItems.Count})");

        var hasMaterial = _materialItems.Count > 0;
        _materialEmptyObject.SetActive(!hasMaterial);
        _materialExistObject.SetActive(hasMaterial);

        _materialListCells.SetActiveAll(false);
        for (var i = 0; i < _materialItems.Count && i < _materialListCells.Length; i++)
        {
            var (itemId, quantity) = _materialItems.GetAt(i);
            var cell = _materialListCells.GetAt(i);
            cell.SetActive(true);
            cell.SetData(itemId, quantity);
        }
    }

    private void RefreshInputSlots()
    {
        var craftData = GameData.Instance.GetArtifactCraftData(_selectedCraftId);
        var materialCount = craftData.MaterialIds.Count;
        LogManager.Assert(_inputSlots.Length >= materialCount, $"[ArtifactCraftContent] RefreshInputSlots: InputSlot count({_inputSlots.Length}) must be >= material count({materialCount}), craftId({_selectedCraftId})");

        var hasAllMaterials = true;
        var isAllMaterialFilled = true;
        _inputSlots.SetActiveAll(false);
        for (var i = 0; i < materialCount && i < _inputSlots.Length; i++)
        {
            var materialId = craftData.MaterialIds.GetAt(i);
            var requiredQuantity = craftData.MaterialQuantities.GetAt(i);
            if (UserData.Instance.GetItemQuantity(materialId) < requiredQuantity)
            {
                hasAllMaterials = false;
            }

            var isFilled = _filledMaterialIds.Contains(materialId);
            if (!isFilled)
            {
                isAllMaterialFilled = false;
            }

            var slot = _inputSlots.GetAt(i);
            slot.SetActive(true);
            slot.SetData(materialId, requiredQuantity, isFilled, _draggingItemId == materialId);
        }

        _autoFillButtonObject.SetActive(hasAllMaterials);
        _autoFillDisabledObject.SetActive(!hasAllMaterials);
        _handle.SetData(craftData.RequiredCoinQuantity, isAllMaterialFilled);
    }

    private void TryFillMaterial(int materialId)
    {
        var craftData = GameData.Instance.GetArtifactCraftData(_selectedCraftId);
        var materialIndex = craftData.MaterialIds.IndexOf(materialId);
        if (materialIndex < 0) return;
        if (_filledMaterialIds.Contains(materialId)) return;

        var requiredQuantity = craftData.MaterialQuantities.GetAt(materialIndex);
        if (UserData.Instance.GetItemQuantity(materialId) < requiredQuantity)
        {
            GlobalManager.Instance.ShowToastMessage("재료가 모자라다는 로컬키 필요"); // TODO(지선)
            return;
        }

        _filledMaterialIds.Add(materialId);
        RefreshInputSlots();
    }

    private void OnMaterialTooltipShow(ArtifactCraftMaterialListCell cell)
    {
        if (_tooltipCell != null && _tooltipCell != cell)
        {
            _tooltipCell.HideTooltip();
        }

        _tooltipCell = cell;
        _tooltipObject.SetActive(true);
    }

    private void OnMaterialTooltipHide(ArtifactCraftMaterialListCell cell)
    {
        if (_tooltipCell != cell) return;

        _tooltipCell = null;
        _tooltipObject.SetActive(false);
    }

    private void OnMaterialBeginDrag(int itemId)
    {
        _draggingItemId = itemId;
        DragThumbnail.Thumbnail.SetData(itemId);
        DragThumbnail.Thumbnail.SetActive(true);
        RefreshInputSlots();
    }

    private void OnMaterialDrop()
    {
        if (_draggingItemId == 0) return;
        TryFillMaterial(_draggingItemId);
    }

    private void OnMaterialEndDrag()
    {
        _draggingItemId = 0;
        DragThumbnail.Thumbnail.SetActive(false);
        RefreshInputSlots();
    }

    private void OnHandlePulled()
    {
        var popup = PopupManager.Instance.ShowPopup(PopupManager.Type.ArtifactCraftConfirm, new ArtifactCraftPopupParameter
        {
            ArtifactItemId = _selectedCraftId,
            OnConfirm = OnCraftConfirm,
        });
        if (popup == null) return;
        popup.SetOnHideAction(_handle.ResetSlider);
    }

    private void OnCraftConfirm()
    {
        var craftData = GameData.Instance.GetArtifactCraftData(_selectedCraftId);
        PopupManager.Instance.ShowPopup(PopupManager.Type.ArtifactCraftPayment, new ArtifactCraftPaymentPopupParameter
        {
            RequiredCoinQuantity = craftData.RequiredCoinQuantity,
        });
    }

    private void OnTabClick(int craftId)
    {
        if (_selectedCraftId != craftId)
        {
            _filledMaterialIds.Clear();
        }

        _selectedCraftId = craftId;
        RefreshTabs();
        RefreshInputSlots();
    }

    public void OnAutoFillButtonClick()
    {
        var craftData = GameData.Instance.GetArtifactCraftData(_selectedCraftId);
        var materialCount = craftData.MaterialIds.Count;
        LogManager.Assert(_flyingThumbnails.Length >= materialCount, $"[ArtifactCraftContent] OnAutoFillButtonClick: FlyingThumbnail count({_flyingThumbnails.Length}) must be >= material count({materialCount}), craftId({_selectedCraftId})");

        var sequence = DOTween.Sequence().SetUpdate(true);
        var flyCount = 0;
        for (var i = 0; i < materialCount && i < _inputSlots.Length; i++)
        {
            var materialId = craftData.MaterialIds.GetAt(i);
            if (_filledMaterialIds.Contains(materialId)) continue;

            ArtifactCraftMaterialListCell startCell = null;
            foreach (var cell in _materialListCells)
            {
                if (!cell.gameObject.activeSelf || cell.ItemDataId != materialId) continue;
                startCell = cell;
                break;
            }
            if (startCell == null) continue;

            var flyingThumbnail = _flyingThumbnails.GetAt(flyCount);
            if (flyingThumbnail == null) break;

            flyingThumbnail.Thumbnail.SetData(materialId);
            flyingThumbnail.RectTransform.position = startCell.RectTransform.position;

            var startTime = flyCount * FlyIntervalSeconds;
            sequence.InsertCallback(startTime, () => flyingThumbnail.Thumbnail.SetActive(true));
            sequence.Insert(startTime, flyingThumbnail.RectTransform
                .DOMove(_inputSlots.GetAt(i).RectTransform.position, FlyDurationSeconds)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    flyingThumbnail.Thumbnail.SetActive(false);
                    TryFillMaterial(materialId);
                }));
            flyCount++;
        }

        if (flyCount == 0)
        {
            sequence.Kill();
            return;
        }

        InputManager.Instance.Block(this);
        _autoFillSequence = sequence.OnComplete(() =>
        {
            _autoFillSequence = null;
            InputManager.Instance.Unblock(this);
        });
    }
}