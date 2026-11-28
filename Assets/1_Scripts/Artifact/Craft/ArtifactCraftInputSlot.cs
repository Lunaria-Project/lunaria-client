using Lunaria;
using UnityEngine;

public class ArtifactCraftInputSlot : MonoBehaviour
{
    [SerializeField] private ItemThumbnail[] _thumbnails;
    [SerializeField] private LayoutSwitcher _layoutSwitcher;
    [SerializeField] private RectTransform _rectTransform;

    private const string NormalLayoutKey = "Normal";
    private const string EmptyLayoutKey = "Empty";
    private const string HoverLayoutKey = "Hover";

    public int MaterialId { get; private set; }
    public RectTransform RectTransform => _rectTransform;

    public void SetData(int materialId, long requiredQuantity, bool isFilled, bool isDragging)
    {
        MaterialId = materialId;
        foreach (var thumbnail in _thumbnails)
        {
            thumbnail.SetData(materialId, requiredQuantity);
        }

        if (isFilled)
        {
            _layoutSwitcher.SetLayout(NormalLayoutKey);
        }
        else if (isDragging)
        {
            _layoutSwitcher.SetLayout(HoverLayoutKey);
        }
        else
        {
            _layoutSwitcher.SetLayout(EmptyLayoutKey);
        }
    }
}