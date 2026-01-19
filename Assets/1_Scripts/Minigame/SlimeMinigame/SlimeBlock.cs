using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lunaria;
using UnityEngine;

public class SlimeBlock : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private GameObject _touchButton;
    [SerializeField] private Image _slimeImage;
    [SerializeField] private Vector2 _showPosition = Vector2.up;

    public bool IsShowing { get; private set; }

    private Action<int> _onTouchSlime;
    private int _slimeOrder;

    public void SetOnTouchSlime(Action<int> onTouchSlime)
    {
        _onTouchSlime = onTouchSlime;
    }

    public async UniTask Show(int slimeOrder, float showTime)
    {
        _slimeOrder = slimeOrder;
        IsShowing = true;
        _rectTransform.anchoredPosition = Vector2.zero;
        _rectTransform.gameObject.SetActive(true);
        _touchButton.SetActive(true);

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
        _onTouchSlime.Invoke(_slimeOrder);
        Hide();
    }
}