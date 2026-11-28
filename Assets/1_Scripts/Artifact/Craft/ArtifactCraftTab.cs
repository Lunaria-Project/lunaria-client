using System;
using Generated;
using Lunaria;
using UnityEngine;

public class ArtifactCraftTab : MonoBehaviour
{
    [Serializable]
    private class MaterialSlot
    {
        public ArtifactCraftMaterial[] Materials;
    }

    [SerializeField] private ItemThumbnail[] _artifactThumbnails;
    [SerializeField] private MaterialSlot[] _materialSlots;
    [SerializeField] private LayoutSwitcher _layoutSwitcher;
    [SerializeField] private GameObject _newObject;

    private const string DisabledLayoutKey = "Disabled";
    private const string NoMaterialLayoutKey = "NoMaterial";
    private const string SelectedLayoutKey = "Selected";
    private const string NormalLayoutKey = "Normal";

    private int _craftId;
    private Action<int> _onClickAction;

    public void SetClickAction(Action<int> onClickAction)
    {
        _onClickAction = onClickAction;
    }

    public void SetData(ArtifactCraftData craftData, bool isUnlocked, bool isSelected)
    {
        _craftId = craftData.Id;

        foreach (var thumbnail in _artifactThumbnails)
        {
            thumbnail.SetData(craftData.Id);
        }
        _newObject.SetActive(isUnlocked && !UserData.Instance.CheckedNewArtifactCraftIds.Contains(_craftId));

        var materialCount = craftData.MaterialIds.Count;
        LogManager.Assert(_materialSlots.Length >= materialCount, $"[ArtifactCraftTab] SetData: MaterialSlot count({_materialSlots.Length}) must be >= material count({materialCount}), craftId({_craftId})");

        var hasAllMaterials = true;
        for (var i = 0; i < _materialSlots.Length; i++)
        {
            var slot = _materialSlots.GetAt(i);
            var isUsed = i < materialCount;
            slot.Materials.SetActiveAll(isUsed);
            if (!isUsed) continue;

            var materialId = craftData.MaterialIds.GetAt(i);
            var requiredQuantity = craftData.MaterialQuantities.GetAt(i);
            var isEnough = UserData.Instance.GetItemQuantity(materialId) >= requiredQuantity;
            if (!isEnough)
            {
                hasAllMaterials = false;
            }

            foreach (var material in slot.Materials)
            {
                material.SetData(materialId, requiredQuantity, isEnough);
            }
        }

        if (!isUnlocked)
        {
            _layoutSwitcher.SetLayout(DisabledLayoutKey);
        }
        else if (!hasAllMaterials)
        {
            _layoutSwitcher.SetLayout(NoMaterialLayoutKey);
        }
        else if (isSelected)
        {
            _layoutSwitcher.SetLayout(SelectedLayoutKey);
        }
        else
        {
            _layoutSwitcher.SetLayout(NormalLayoutKey);
        }
    }

    public void OnButtonClick()
    {
        _onClickAction?.Invoke(_craftId);
    }

    public void OnNoMaterialButtonClick()
    {
        GlobalManager.Instance.ShowToastMessage("재료가 부족하다는 로컬키 추가 필요"); // TODO(지선)
    }

    public void OnDisabledButtonClick()
    {
        GlobalManager.Instance.ShowToastMessage("해금 필요하다는 메세지가 필요할까요?"); // TODO(지선)
    }
}
