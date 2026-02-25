using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class PopupManager : SingletonMonoBehaviour<PopupManager>
{
    [SerializeField] private RectTransform _parentRectTransform;

    public List<PopupBase> PopupList { get; private set; }= new();

    protected override void Update()
    {
        if (PopupList.Count <= 0) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var currentPopup = PopupList.GetLast();
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

    public PopupBase ShowPopupWithEmptyParameter(Type popupType)
    {
        return ShowPopup(popupType, new PopupEmptyParameter());
    }

    public async UniTask HideCurrentPopup(Type type = Type.None)
    {
        if (PopupList.Count <= 0) return;

        if (type != Type.None)
        {
            PopupBase target = null;
            foreach (var popupBase in PopupList)
            {
                if (popupBase.PopupType != type) continue;
                target = popupBase;
                break;
            }
            if (target != null)
            {
                PopupList.Remove(target);
                HidePopupInternal(target);
            }
            return;
        }

        HidePopupInternal(PopupList.GetLast());
        PopupList.RemoveLast();
        await UniTask.NextFrame();
    }

    public UniTask HideAllPopups()
    {
        if (PopupList.Count <= 0) return UniTask.CompletedTask;

        return HideAllPopupsInternal();
    }

    public bool ContainsPopup(Type type)
    {
        foreach (var popup in PopupList)
        {
            if (popup.PopupType != type) continue;
            return true;
        }
        return false;
    }

    private async UniTask HideAllPopupsInternal()
    {
        for (var index = PopupList.Count - 1; index >= 0; index--)
        {
            var popup = PopupList[index];
            HidePopupInternal(popup);
        }

        PopupList.Clear();
        await UniTask.NextFrame();
    }

    private void HidePopupInternal(PopupBase popup)
    {
        if (popup == null) return;

        popup.HideInternal();
        Destroy(popup.gameObject);
    }

    private void PushPopup(PopupBase popup)
    {
        if (popup == null) return;

        PopupList.Add(popup);
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