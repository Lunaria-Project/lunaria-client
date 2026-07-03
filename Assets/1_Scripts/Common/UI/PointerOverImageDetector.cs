using Lunaria;
using UnityEngine;
using UnityEngine.EventSystems;

// 이미지의 빈 공간까지 정확히 판정하려면 스프라이트 텍스처가 Read/Write Enabled 여야 한다.
[RequireComponent(typeof(Image))]
public class PointerOverImageDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Range(0f, 1f)]
    [SerializeField] private float _alphaHitTestThreshold = 0.5f;

    private Image _image;

    public bool IsPointerOver { get; private set; }

    protected void Awake()
    {
        _image = GetComponent<Image>();
        _image.alphaHitTestMinimumThreshold = _alphaHitTestThreshold;
    }

    protected void OnDisable()
    {
        IsPointerOver = false;
    }

    protected void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            IsPointerOver = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsPointerOver = false;
    }
}
