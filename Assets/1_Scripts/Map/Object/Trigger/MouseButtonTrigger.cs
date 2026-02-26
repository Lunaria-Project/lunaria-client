using System;
using UnityEngine;

public class MouseButtonTrigger : MonoBehaviour
{
    private Action<bool> _onMouseUpDown; 
    
    public void SetOnMouseUpDown(Action<bool> onMouseUpDown)
    {
        _onMouseUpDown = onMouseUpDown;
    }

    protected virtual void OnMouseDown()
    {
        _onMouseUpDown?.Invoke(true);
    }

    protected virtual void OnMouseUp()
    {
        _onMouseUpDown?.Invoke(false);
    }
}
