using System;
using Lunaria;
using UnityEngine;

public struct ArtifactCraftPopupParameter : IPopupParameter
{
    public int ArtifactItemId { get; init; }
    public Action OnConfirm { get; init; }
}

public class ArtifactCraftConfirmPopup : Popup<ArtifactCraftPopupParameter>
{
    [SerializeField] private ItemThumbnail _thumbnail;

    private Action _onConfirm;

    protected override void OnShow(ArtifactCraftPopupParameter parameter)
    {
        _thumbnail.SetData(parameter.ArtifactItemId);
        _onConfirm = parameter.OnConfirm;
    }

    protected override void OnHide() { }

    public void OnConfirmButtonClick()
    {
        _onConfirm?.Invoke();
        OnHideButtonClick();
    }

    public void OnCancelButtonClick()
    {
        OnHideButtonClick();
    }
}
