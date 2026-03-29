using UnityEngine;

public class Scroller : MonoBehaviour
{
    [SerializeField] private Lunaria.Image _image;
    [SerializeField] private float x, y;

    private void Update()
    {
        _image.material.mainTextureOffset += new Vector2(x, y) * Time.deltaTime;
    }
}