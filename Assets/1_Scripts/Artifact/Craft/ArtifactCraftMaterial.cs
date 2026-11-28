using Lunaria;
using UnityEngine;

public class ArtifactCraftMaterial : MonoBehaviour
{
    [SerializeField] private ItemThumbnail _thumbnail;
    [SerializeField] private LayoutSwitcher _layoutSwitcher;

    private const string NormalLayoutKey = "Normal";
    private const string NotEnoughLayoutKey = "NotEnough";

    public void SetData(int materialId, long requiredQuantity, bool isEnough)
    {
        _thumbnail.SetData(materialId, requiredQuantity);
        _layoutSwitcher.SetLayout(isEnough ? NormalLayoutKey : NotEnoughLayoutKey);
    }
}
