using UnityEngine;

public class Scroller : MonoBehaviour
{
    [SerializeField] private Lunaria.Image _image;
    [SerializeField] private float x, y;

    private Material _materialInstance;

    private void Awake()
    {
        _materialInstance = new Material(_image.material);
        _image.material = _materialInstance;
    }

    private void Update()
    {
        _materialInstance.mainTextureOffset += new Vector2(x, y) * Time.deltaTime;
    }

    private void OnDestroy()
    {
        if (_materialInstance != null)
        {
            Destroy(_materialInstance);
        }
    }
}