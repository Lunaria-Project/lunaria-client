using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class PopupManager : SingletonMonoBehaviour<PopupManager>
{
    [SerializeField] private RectTransform _parentRectTransform;

    private readonly List<PopupBase> _popupList = new();

    private void Update()
    {
        if (_popupList.Count <= 0) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var currentPopup = _popupList.GetLast();
            if (currentPopup == null) return;
            if (!currentPopup.HideWithEscapeKey()) return;

            HideCurrentPopup().Forget();
        }
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
        if (_popupList.Count <= 0) return null;
        return _popupList.GetLast();
    }

    public async UniTask HideCurrentPopup(Type type = Type.None)
    {
        if (_popupList.Count <= 0) return;

        if (type != Type.None)
        {
            foreach (var popupBase in _popupList)
            {
                if (popupBase.PopupType != type) continue;
                HidePopupInternal(popupBase);
                return;
            }
            return;
        }

        HidePopupInternal(_popupList.GetLast());
        _popupList.RemoveLast();
        await UniTask.NextFrame();
    }

    public UniTask HideAllPopups()
    {
        if (_popupList.Count <= 0) return UniTask.CompletedTask;

        return HideAllPopupsInternal();
    }

    public bool ContainsPopup(Type type)
    {
        foreach (var popup in _popupList)
        {
            if (popup.PopupType != type) continue;
            return true;
        }
        return false;
    }

    private async UniTask HideAllPopupsInternal()
    {
        foreach (var popup in _popupList)
        {
            HidePopupInternal(popup);
        }
        _popupList.Clear();
        await UniTask.NextFrame();
    }

    private void HidePopupInternal(PopupBase popup)
    {
        if (popup == null) return;

        _popupList.Remove(popup);
        popup.HideInternal();
        Destroy(popup.gameObject);
    }

    private void PushPopup(PopupBase popup)
    {
        if (popup == null) return;

        _popupList.Add(popup);
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