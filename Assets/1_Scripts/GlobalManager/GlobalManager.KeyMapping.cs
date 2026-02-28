using System;
using UnityEngine;

public partial class GlobalManager
{
    public event Action OnQKeyDown;
    public event Action OnEKeyDown;

    private void UpdateKey()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnQKeyDown?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            OnEKeyDown?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!CanPlayerMove()) return;
            if (MapManager.Instance.TryInteractNearestNpc()) return;
            if (MapManager.Instance.TryInteractNearestStaticNpc()) return;
            if (MapManager.Instance.TryInteractNearestShop()) return;
        }
    }
}
