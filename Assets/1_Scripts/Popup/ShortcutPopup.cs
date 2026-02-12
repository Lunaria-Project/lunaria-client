using System;
using Cysharp.Threading.Tasks;

public struct ShortcutPopupParameter : IPopupParameter
{
    public Action ShortcutAction { get; init; }
}

public class ShortcutPopup : Popup<ShortcutPopupParameter>
{
    private Action _shortcutAction;
    private bool _shortcutButtonClicked;

    private UniTaskCompletionSource<bool> _hideTaskCompletionSource;

    public UniTask<bool> GetTask()
    {
        _hideTaskCompletionSource ??= new UniTaskCompletionSource<bool>();
        return _hideTaskCompletionSource.Task;
    }

    protected override void OnShow(ShortcutPopupParameter parameter)
    {
        _shortcutAction = parameter.ShortcutAction;
        _shortcutButtonClicked = false;
    }
    protected override void OnHide()
    {
        TaskUtil.SetResult(ref _hideTaskCompletionSource, _shortcutButtonClicked);
    }

    public void OnShortcutButtonClick()
    {
        _shortcutButtonClicked = true;
        _shortcutAction?.Invoke();
    }
}
