using System;
using DG.Tweening;
using Lunaria;
using UnityEngine;

public enum PowderPortalDirection
{
    Left,
    Right,
    Space
}

public class PowderPortalMinigameObject : MonoBehaviour
{
    [SerializeField] private Image _objectImage;
    [SerializeField] private RectTransform _rectTransform;

    public PowderPortalDirection CorrectDirection { get; private set; }
    public bool IsActive { get; private set; }
    public Vector2 AnchoredPosition => _rectTransform.anchoredPosition;

    public void Init(PowderPortalDirection direction, float posX)
    {
        DOTween.Kill(_objectImage);
        CorrectDirection = direction;
        IsActive = true;
        _objectImage.SetSprite(ResourceManager.Instance.LoadSprite($"powder_portal_minigame_object_{((int)direction + 1).ToNDigits(2)}"));
        var color = _objectImage.color;
        color.a = 1f;
        _objectImage.color = color;
        _rectTransform.anchoredPosition = new Vector2(posX, _rectTransform.anchoredPosition.y);
        gameObject.SetActive(true);
    }

    public void PlayFadeOutAnimation(float duration, Action<PowderPortalMinigameObject> onComplete)
    {
        DOTween.Kill(_objectImage);
        _objectImage.DOFade(0f, duration)
            .OnComplete(() => onComplete?.Invoke(this));
    }

    public void MoveBy(float deltaX)
    {
        _rectTransform.anchoredPosition += Vector2.right * deltaX;
    }

    public void Hide()
    {
        IsActive = false;
        DOTween.Kill(_objectImage);
        gameObject.SetActive(false);
    }
}