using UnityEngine;
using UnityEngine.UI;

public partial class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float x, y;

    private void Update()
    {
     
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(x, y) * Time.deltaTime, _img.uvRect.size);
    }
}