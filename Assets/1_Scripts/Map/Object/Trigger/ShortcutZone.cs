using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShortcutZone : Collider2DTrigger
{
    [SerializeField] private ShortcutType _shortcutType;
    [SerializeField] private bool _isInstantShortcut;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (!other.CompareTag(StringManager.PlayerTagName)) return;
        if (_isInstantShortcut)
        {
            GlobalManager.Instance.ShortcutInvoke(_shortcutType).Forget();
            return;
        }
        var parameter = new ShortcutPopupParameter { ShortcutAction = () => { GlobalManager.Instance.ShortcutInvoke(_shortcutType).Forget(); } };
        PopupManager.Instance.ShowPopup(PopupManager.Type.Shortcut, parameter);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (!GlobalManager.HasInstance) return;
        if (!other.CompareTag(StringManager.PlayerTagName)) return;
        PopupManager.Instance.HideCurrentPopup(PopupManager.Type.Shortcut).Forget();
    }
}