using UnityEngine;
using UnityEngine.UI;

public static class ImageExtension
{
    public static void SetSprite(this Image image, Sprite sprite)
    {
        image.sprite = sprite;
    }
}
