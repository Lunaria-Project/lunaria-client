using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShortcutZone : MonoBehaviour
{
    [SerializeField] private ShortcutType _shortcutType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(GlobalManager.Instance.PlayerTagName)) return;
        var parameter = new ShortcutPopupParameter { ShortcutAction = () => { GlobalManager.Instance.ShortcutInvoke(_shortcutType).Forget(); } };
        PopupManager.Instance.ShowPopup(PopupManager.Type.Shortcut, parameter);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!GlobalManager.HasInstance) return;
        if (!other.CompareTag(GlobalManager.Instance.PlayerTagName)) return;
        PopupManager.Instance.HideCurrentPopup(PopupManager.Type.Shortcut).Forget();
    }
}