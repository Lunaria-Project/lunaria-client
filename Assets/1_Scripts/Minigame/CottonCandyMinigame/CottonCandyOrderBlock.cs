using Lunaria;
using UnityEngine;

public class CottonCandyOrderBlock : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Text[] _quantityTexts;
    [SerializeField] private Image[] _layerImages;
    [SerializeField] private Image[] _directionImages;

    private readonly Color _clockwiseColor = "#FFFB87".ToColor();
    private readonly Color _counterClockwiseColor = "#88FF87".ToColor();

    public void SetOrder(CottonCandyOrder order)
    {
        var uiCount = Mathf.Min(_layerImages.Length, _quantityTexts.Length, _directionImages.Length);
        if (order.Layers.Length != uiCount)
        {
            LogManager.LogErrorPack("[CottonCandyMinigame] OrderView: 레이어 개수 불일치", order.Layers.Length, uiCount);
            return;
        }
        _root.SetActive(true);
        for (var i = 0; i < uiCount; i++)
        {
            var layer = order.Layers[i];
            var isClockwise = layer.Direction == CottonCandyRotationDirection.Clockwise;
            _layerImages[i].SetSprite(ResourceManager.Instance.LoadCottonCandyMinigameSprite(i, layer.Color, layer.Shape));
            _quantityTexts[i].SetText(layer.RotationCount.ToString());
            _directionImages[i].transform.localScale = new Vector3(isClockwise ? 1 : -1, 1, 1);
            _directionImages[i].color = isClockwise ? _clockwiseColor : _counterClockwiseColor;
        }
    }

    public void Hide()
    {
        _root.SetActive(false);
    }
}
