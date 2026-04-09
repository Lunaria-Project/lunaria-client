using System;
using Lunaria;
using UnityEngine;

public abstract class SystemButtonPopupParameter : IPopupParameter
{
    public LocalKey Description { get; init; }
    public LocalKey ConfirmButtonText { get; init; }
    public Action OnConfirm { get; init; }
}

public class SystemOneButtonParameter : SystemButtonPopupParameter { }

public class SystemTwoButtonParameter : SystemButtonPopupParameter
{
    public LocalKey CancelButtonText { get; init; }
    public Action OnCancel { get; init; }
}

public class SystemButtonPopup : Popup<SystemButtonPopupParameter>
{
    [SerializeField] private Text _descriptionText;
    [SerializeField] private Text _confirmButtonText;
    [SerializeField] private Text _cancelButtonText;
    [SerializeField] private GameObject _cancelButton;

    private Action _onConfirm;
    private Action _onCancel;

    protected override void OnShow(SystemButtonPopupParameter parameter)
    {
        _descriptionText.SetText(parameter.Description);
        _confirmButtonText.SetText(parameter.ConfirmButtonText);
        _onConfirm = parameter.OnConfirm;

        switch (parameter)
        {
            case SystemOneButtonParameter:
            {
                _cancelButton.SetActive(false);
                break;
            }
            case SystemTwoButtonParameter twoButton:
            {
                _cancelButton.SetActive(true);
                _cancelButtonText.SetText(twoButton.CancelButtonText);
                _onCancel = twoButton.OnCancel;
                break;
            }
        }
    }

    protected override void OnHide() { }

    public void OnConfirmButtonClick()
    {
        _onConfirm?.Invoke();
        OnHideButtonClick();
    }

    public void OnCancelButtonClick()
    {
        _onCancel?.Invoke();
        OnHideButtonClick();
    }
}