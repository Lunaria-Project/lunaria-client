using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lunaria;
using UnityEngine;

public class SlimeBlock : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private GameObject _touchButton;
    [SerializeField] private RectTransform _slimeImageRectTransform;
    [SerializeField] private Image _slimeImage;
    [SerializeField] private Vector2 _showPosition = Vector2.up;

    public bool IsShowing { get; private set; }

    private Action<SlimeType> _onTouchSlime;
    private SlimeType _slimeType;
    private int _remainTouchCount;

    public void SetOnTouchSlime(Action<SlimeType> onTouchSlime)
    {
        _onTouchSlime = onTouchSlime;
    }

    public async UniTask Show(SlimeType type, int touchCount, float scale, float showTime)
    {
        _slimeType = type;
        _remainTouchCount = touchCount;
        IsShowing = true;
        _rectTransform.anchoredPosition = Vector2.zero;
        _rectTransform.gameObject.SetActive(true);
        _touchButton.SetActive(true);

        _slimeImage.SetSprite(ResourceManager.Instance.LoadSlimeMinigameSprite(type));
        var isToxic = type is SlimeType.ToxicLevel1 or SlimeType.ToxicLevel2 or SlimeType.ToxicLevel3;
        _slimeImage.color = isToxic ? Color.gray : Color.white; // TODO(지선):임시 코드
        _slimeImageRectTransform.SetScale(scale);

        DOTween.Kill(this);
        await DOTween.Sequence().SetId(this)
            .Append(_rectTransform.DOAnchorPos(_showPosition, 0.3f).SetEase(Ease.OutQuad))
            .AppendInterval(showTime)
            .Append(_rectTransform.DOAnchorPos(Vector2.zero, 0.2f)).SetEase(Ease.OutQuad)
            .AppendOnComplete(Hide)
            .GetTask();
    }

    public void Hide()
    {
        IsShowing = false;
        DOTween.Kill(this);
        _rectTransform.anchoredPosition = Vector2.zero;
        _rectTransform.gameObject.SetActive(false);
        _touchButton.SetActive(false);
    }

    public void OnTouchButtonClick()
    {
        _remainTouchCount--;
        if (_remainTouchCount > 0) return;

        _onTouchSlime.Invoke(_slimeType);
        Hide();
    }
}