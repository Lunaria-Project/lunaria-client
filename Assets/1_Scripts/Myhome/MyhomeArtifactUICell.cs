using DG.Tweening;
using Lunaria;
using UnityEngine;

public class MyhomeArtifactUICell : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _selectedBlock;
    [SerializeField] private GameObject _unselectedBlock;

    public int ItemDataId { get; private set; }
    public RectTransform ImageRectTransform => _imageRectTransform;

    public void SetData(int itemId)
    {
        ItemDataId = itemId;

        var itemData = GameData.Instance.GetItemData(itemId);
        _image.SetSprite(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
    }

    public void SetSelected(bool isSelected)
    {
        _selectedBlock.SetActive(isSelected);
        _unselectedBlock.SetActive(!isSelected);

        if (isSelected)
        {
            _rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f).SetEase(Ease.InOutFlash);
        }
        else
        {
            _rectTransform.SetScale(1f);
        }
    }
}