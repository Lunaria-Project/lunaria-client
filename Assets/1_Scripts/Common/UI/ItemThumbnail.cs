using UnityEngine;

namespace Lunaria
{
    public class ItemThumbnail : MonoBehaviour
    {
        [SerializeField] private Image[] _images;
        [SerializeField] private Text[] _quantityTexts;
        [SerializeField] private Text[] _nameTexts;

        public void SetData(int itemId)
        {
            var itemData = GameData.Instance.GetItemData(itemId);
            _images.SetSprites(ResourceManager.Instance.LoadSprite(itemData.IconResourceKey));
            _nameTexts.SetTexts(itemData.Name);
        }

        public void SetData(int itemId, long quantity)
        {
            SetData(itemId);
            _quantityTexts.SetTexts(quantity.ToPrice());
        }
    }
}
