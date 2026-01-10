using System.Collections.Generic;
using UnityEngine;

public partial class PopupManager : SingletonMonoBehaviourDontDestroy<PopupManager>
{
    [SerializeField] private RectTransform _parentRectTransform;

    private readonly Stack<PopupBase> _popupStack = new();

    private void Update()
    {
        if (_popupStack.Count <= 0) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_popupStack.TryPeek(out var currentPopup)) return;
            if (!currentPopup.HideWithEscapeKey()) return;
            HideCurrentPopup();
        }
    }

    public TPopup ShowPopup<TPopup, TPopupParameter>(Type popupType, TPopupParameter parameter) where TPopup : Popup<TPopup, TPopupParameter> where TPopupParameter : IPopupParameter<TPopup>
    {
        var popup = GetPopup(popupType) as TPopup;
        if (popup == null) return null;

        PushPopup(popup);
        popup.OnShow(parameter);

        return popup;
    }

    public void HideCurrentPopup()
    {
        if (_popupStack.Count <= 0) return;

        var popup = _popupStack.Pop();
        HidePopupInternal(popup);
    }

    public void HideAllPopups()
    {
        if (_popupStack.Count <= 0) return;

        while (_popupStack.Count > 0)
        {
            var popup = _popupStack.Pop();
            HidePopupInternal(popup);
        }
    }

    private void HidePopupInternal(PopupBase popup)
    {
        if (popup == null) return;

        Destroy(popup.gameObject);
    }

    private void PushPopup(PopupBase popup)
    {
        if (popup == null) return;

        _popupStack.Push(popup);
        popup.transform.SetAsLastSibling();
    }

    private PopupBase GetPopup(Type popupType)
    {
        if (!_popupResourceKey.TryGetValue(popupType, out var popupResourceKey)) return null;

        var popupPrefab = ResourceManager.Instance.LoadPopupPrefab(popupResourceKey);
        if (popupPrefab == null) return null;

        var popup = Instantiate(popupPrefab, _parentRectTransform);
        popup.gameObject.SetActive(true);
        return popup;
    }
}