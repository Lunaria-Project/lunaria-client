using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShortcutZone : MonoBehaviour
{
    [SerializeField] private ShortcutType _shortcutType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var parameter = new ShortcutPopupParameter { ShortcutAction = () => { GlobalManager.Instance.ShortcutInvoke(_shortcutType); } };
        PopupManager.Instance.ShowPopup(PopupManager.Type.Shortcut, parameter);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (gameObject == null) return;
        PopupManager.Instance.HideCurrentPopup(PopupManager.Type.Shortcut).Forget();
    }
}