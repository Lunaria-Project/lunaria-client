using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static void SetActive<T>(this T component, bool isActive) where T : MonoBehaviour
    {
        if (component == null) return;

        component.gameObject.SetActive(isActive);
    }

    public static void SetActiveAll<T>(this IEnumerable<T> @this, bool active) where T : MonoBehaviour
    {
        foreach (var item in @this)
        {
            item.gameObject.SetActive(active);
        }
    }

    public static void SetActiveAll(this IEnumerable<GameObject> @this, bool active)
    {
        foreach (var item in @this)
        {
            item.gameObject.SetActive(active);
        }
    }
}