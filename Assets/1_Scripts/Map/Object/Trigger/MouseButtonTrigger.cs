using System;
using UnityEngine;

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
        _onMouseDown?.Invoke();
    }

    protected void OnMouseUp()
    {
        _onMouseUp?.Invoke();
    }
}