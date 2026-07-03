using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseButtonTrigger : MonoBehaviour
{
    private Action _onMouseDown;
    private Action _onMouseUp;

    public void SetOnMouseDown(Action onMouseDown)
    {
        _onMouseDown = onMouseDown;
    }

    public void SetOnMouseUp(Action onMouseUp)
    {
        _onMouseUp = onMouseUp;
    }

    protected void OnMouseDown()
    {
        if (IsPointerOverUI()) return;
        _onMouseDown?.Invoke();
    }

    protected void OnMouseUp()
    {
        if (IsPointerOverUI()) return;
        _onMouseUp?.Invoke();
    }

    private static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        return EventSystem.current.IsPointerOverGameObject();
    }
}