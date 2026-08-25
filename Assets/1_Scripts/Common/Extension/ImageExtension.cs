using UnityEngine;
using Image = Lunaria.Image;

public static class ImageExtension
{
    public static void SetSprite(this Image image, Sprite sprite)
    {
        image.sprite = sprite;
    }

    public static void SetSprites(this Image[] images, Sprite sprite)
    {
        foreach (var image in images)
        {
            image.SetSprite(sprite);
        }
    }
}