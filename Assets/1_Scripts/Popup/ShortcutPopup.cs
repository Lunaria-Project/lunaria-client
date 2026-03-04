using System;

public struct ShortcutPopupParameter : IPopupParameter
{
    public Action ShortcutAction { get; init; }
}

public class ShortcutPopup : Popup<ShortcutPopupParameter>
{
    private Action _shortcutAction;

    protected override void OnShow(ShortcutPopupParameter parameter)
    {
        _shortcutAction = parameter.ShortcutAction;
    }
    protected override void OnHide() { }

    public void OnShortcutButtonClick()
    {
        _shortcutAction?.Invoke();
    }
}