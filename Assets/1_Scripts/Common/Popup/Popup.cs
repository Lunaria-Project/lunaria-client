using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IPopupParameter { }
public struct PopupEmptyParameter : IPopupParameter { }

public abstract class PopupBase : MonoBehaviour
{
    [SerializeField] private bool _hideOnEscapeKey = false;

    public PopupManager.Type PopupType { get; private set; }

    private UniTaskCompletionSource _hideTaskCompletionSource = null;
    private Action _onHideAction = null;
    
    public void SetOnHideAction(Action onHideAction)
    {
        _onHideAction = onHideAction;
    }

    public UniTask GetTask()
    {
        if (_hideTaskCompletionSource == null) return UniTask.CompletedTask;
        return _hideTaskCompletionSource.Task;
    }

    public void OnHideButtonClick()
    {
        if (PopupManager.Instance == null) return;
        PopupManager.Instance.HideCurrentPopup(PopupType).Forget();
    }
    
    public virtual bool HideWithEscapeKey()
    {
        return _hideOnEscapeKey;
    }

    protected abstract void OnShowInternal(object parameterObject);

    protected abstract void OnHideInternal();

    internal void ShowInternal(PopupManager.Type type, object parameterObject)
    {
        PopupType = type;
        _hideTaskCompletionSource = new UniTaskCompletionSource();
        OnShowInternal(parameterObject);
    }

    internal void HideInternal()
    {
        OnHideInternal();
        TaskUtil.SetResult(ref _hideTaskCompletionSource);
        _onHideAction?.Invoke();
        _onHideAction = null;
    }
}

public abstract class Popup<TPopupParameter> : PopupBase where TPopupParameter : IPopupParameter
{
    protected abstract void OnShow(TPopupParameter parameter);
    protected abstract void OnHide();

    protected override void OnShowInternal(object parameterObject)
    {
        if (parameterObject == null)
        {
            OnShow(default);
            return;
        }

        if (parameterObject is not TPopupParameter parameter)
        {
            Debug.LogError($"Popup parameter type mismatch. Expected: {typeof(TPopupParameter).Name}, Actual: {parameterObject.GetType().Name}");
            return;
        }

        OnShow(parameter);
    }

    protected override void OnHideInternal()
    {
        OnHide();
    }
}