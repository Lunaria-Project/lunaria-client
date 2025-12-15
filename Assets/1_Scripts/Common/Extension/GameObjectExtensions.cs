using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetActive(this Component component, bool isActive)
    {
        if (component == null) return;

        component.gameObject.SetActive(isActive);
    }

    public static void SetActive(this GameObject gameObject, bool isActive)
    {
        if (gameObject == null) return;

        gameObject.SetActive(isActive);
    }
}