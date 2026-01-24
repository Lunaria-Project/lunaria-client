using UnityEngine;

public static class TransformExtension
{
    public static void SetActive(this RectTransform component, bool isActive)
    {
        if (component == null) return;

        component.gameObject.SetActive(isActive);
    }
    
    public static void SetScale(this RectTransform rectTransform, float scale)
    {
        if (rectTransform == null) return;
        rectTransform.localScale = new Vector3(scale, scale, scale);
    }
}