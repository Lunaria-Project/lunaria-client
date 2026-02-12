using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShortcutZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var parameter = new ShortcutPopupParameter { ShortcutAction = () => { GlobalManager.Instance.GoToMyhomeAsync().Forget(); } };
        PopupManager.Instance.ShowPopup(PopupManager.Type.Shortcut, parameter);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (gameObject == null) return;
        //PopupManager.Instance.HideCurrentPopup(PopupManager.Type.Shortcut).Forget();
    }
}