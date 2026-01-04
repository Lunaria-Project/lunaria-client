using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class PopupBase : MonoBehaviour
{
    public static readonly Dictionary<string, List<MemberInfo>> ParameterMemberInfosDictionary = new();

    public bool HideOnEscapeKey = false;

    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransformRef == null)
            {
                _rectTransformRef = transform as RectTransform;
            }
            return _rectTransformRef;
        }
    }

    private RectTransform _rectTransformRef;
    private double _delayTimeForHideOnTouchDefaultBackground;
    private DateTime _openClientDateTime;

    protected abstract UniTask HidePopup();
    protected abstract void SetTaskResult();

    protected virtual void Awake()
    {
        _rectTransformRef = transform as RectTransform;
    }

    protected virtual bool OnEscapeKeyDown()
    {
        return false;
    }
}

public abstract class Popup<TPopupType> : PopupBase where TPopupType : Popup<TPopupType>
{
    private UniTaskCompletionSource _hideTaskCompletionSource = null;

    protected override void SetTaskResult()
    {
        TaskUtil.SetResult(ref _hideTaskCompletionSource);
    }

    public UniTask GetTask()
    {
        if (PopupManager.Instance == null) return UniTask.CompletedTask;

        _hideTaskCompletionSource ??= new UniTaskCompletionSource();
        return _hideTaskCompletionSource.Task;
    }
}

public abstract class Popup<TPopupType, TPopupParameter> : Popup<TPopupType> where TPopupType : Popup<TPopupType> where TPopupParameter : IPopupParameter<TPopupType>
{
    public abstract void OnShow(TPopupParameter parameter);
    public abstract void OnHide();
}

public interface IPopupParameter<TPopupType> where TPopupType : PopupBase { }