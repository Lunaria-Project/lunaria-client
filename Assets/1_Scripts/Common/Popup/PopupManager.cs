using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class PopupManager : SingletonMonoBehaviourDontDestroy<PopupManager>
{
    [SerializeField] private RectTransform _parentRectTransform;

    private readonly Stack<PopupBase> _popupStack = new();

    private void Update()
    {
        if (_popupStack.Count <= 0) return;

        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (!_popupStack.TryPeek(out var currentPopup)) return;
        if (!currentPopup.HideWithEscapeKey()) return;

        HideCurrentPopup().Forget();
    }

    public PopupBase ShowPopup(Type popupType, IPopupParameter parameter)
    {
        var popup = CreatePopupInstance(popupType);
        if (popup == null) return null;

        PushPopup(popup);
        popup.ShowInternal(popupType, parameter);

        return popup;
    }

    public PopupBase ShowPopup(Type popupType)
    {
        return ShowPopup(popupType, new PopupEmptyParameter());
    }

    public PopupBase GetCurrentPopup()
    {
        if (_popupStack.Count <= 0) return null;
        return _popupStack.Peek();
    }

    public async UniTask HideCurrentPopup(Type type = Type.None)
    {
        if (_popupStack.Count <= 0) return;

        var popup = _popupStack.Peek();
        if (type != Type.None && popup.PopupType != type) return;
        HidePopupInternal(_popupStack.Pop());
        await UniTask.NextFrame();
    }

    public UniTask HideAllPopups()
    {
        if (_popupStack.Count <= 0) return UniTask.CompletedTask;

        return HideAllPopupsInternal();
    }

    private async UniTask HideAllPopupsInternal()
    {
        while (_popupStack.Count > 0)
        {
            HidePopupInternal(_popupStack.Pop());
        }
        await UniTask.NextFrame();
    }

    private static void HidePopupInternal(PopupBase popup)
    {
        if (popup == null) return;

        popup.HideInternal();
        Destroy(popup.gameObject);
    }

    private void PushPopup(PopupBase popup)
    {
        if (popup == null) return;

        _popupStack.Push(popup);
        popup.transform.SetAsLastSibling();
    }

    private PopupBase CreatePopupInstance(Type popupType)
    {
        if (!_popupResourceKey.TryGetValue(popupType, out var popupResourceKey)) return null;

        var popupPrefab = ResourceManager.Instance.LoadPopupPrefab(popupResourceKey);
        if (popupPrefab == null) return null;

        var popup = Instantiate(popupPrefab, _parentRectTransform);
        popup.gameObject.SetActive(true);

        return popup;
    }
}