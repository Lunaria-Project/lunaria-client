using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public partial class PopupManager : SingletonMonoBehaviour<PopupManager>
{
    [SerializeField] private RectTransform _parentRectTransform;
    [SerializeField] private Image _backgroundImage;

    private List<PopupBase> PopupList { get; set; } = new();

    protected override void Awake()
    {
        base.Awake();
        UpdateBackground();
    }

    protected override void Update()
    {
        if (PopupList.Count <= 0) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var currentPopup = PopupList.GetLast();
            if (currentPopup == null) return;
            if (!currentPopup.HideWithEscapeKey()) return;

            currentPopup.OnHideButtonClick();
        }
    }

    public PopupBase ShowPopup(Type popupType, IPopupParameter parameter)
    {
        var popup = CreatePopupInstance(popupType);
        if (popup == null) return null;

        PushPopup(popup);
        popup.ShowInternal(popupType, parameter);
        UpdateBackground();

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
                UpdateBackground();
            }
            return;
        }

        HidePopupInternal(PopupList.GetLast());
        PopupList.RemoveLast();
        UpdateBackground();
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
        UpdateBackground();
        await UniTask.NextFrame();
    }

    private void UpdateBackground()
    {
        if (PopupList.Count <= 0)
        {
            _backgroundImage.gameObject.SetActive(false);
            return;
        }

        var topPopup = PopupList.GetLast();
        var background = topPopup.Background;

        switch (EnumSwitch.Exhaustive(background))
        {
            case PopupBase.PopupBackground.None:
            {
                _backgroundImage.gameObject.SetActive(false);
                return;
            }
            case PopupBase.PopupBackground.Dim0:
            {
                ApplyBackgroundAlpha(0f);
                return;
            }
            case PopupBase.PopupBackground.Dim25:
            {
                ApplyBackgroundAlpha(0.25f);
                return;
            }
            case PopupBase.PopupBackground.Dim50:
            {
                ApplyBackgroundAlpha(0.5f);
                return;
            }
            case PopupBase.PopupBackground.Dim75:
            {
                ApplyBackgroundAlpha(0.75f);
                return;
            }
            case PopupBase.PopupBackground.Dim90:
            {
                ApplyBackgroundAlpha(0.9f);
                return;
            }
        }
        return;

        void ApplyBackgroundAlpha(float alpha)
        {
            _backgroundImage.gameObject.SetActive(true);
            var color = _backgroundImage.color;
            color.a = alpha;
            _backgroundImage.color = color;
        }
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
        if (!_popupResourceKey.TryGetValue(popupType, out var popupResourceKey))
        {
            LogManager.LogError($"팝업 리소스키가 없습니다 {popupType}");
            return null;
        }

        var popupPrefab = ResourceManager.Instance.LoadPopupPrefab(popupResourceKey);
        if (popupPrefab == null)
        {
            LogManager.LogError($"팝업 리소스키 없습니다 {popupType}");
            return null;
        }

        var popup = Instantiate(popupPrefab, _parentRectTransform);
        popup.gameObject.SetActive(true);

        return popup;
    }
}