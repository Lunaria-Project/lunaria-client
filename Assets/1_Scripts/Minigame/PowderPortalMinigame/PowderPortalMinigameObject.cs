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
        DOTween.Kill(_rectTransform);
        CorrectDirection = direction;
        IsActive = true;
        _objectImage.SetSprite(ResourceManager.Instance.LoadSprite($"powder_portal_minigame_object_{((int)direction + 1).ToNDigits(2)}"));
        _rectTransform.anchoredPosition = new Vector2(posX, _rectTransform.anchoredPosition.y);
        _rectTransform.localEulerAngles = Vector3.zero;
        gameObject.SetActive(true);
    }

    public void PlaySendAnimation(Vector2 targetPosition, float duration, Action<PowderPortalMinigameObject> onComplete)
    {
        DOTween.Kill(_rectTransform);
        DOTween.Sequence()
            .Append(_rectTransform.DOMove(targetPosition, duration).SetEase(Ease.OutExpo))
            .Join(_rectTransform.DORotate(new Vector3(0f, 0f, -360f), duration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
            .OnComplete(() => onComplete?.Invoke(this));
    }

    public void MoveToPosition(float posX)
    {
        _rectTransform.anchoredPosition = Vector2.right * posX;
    }

    public void Hide()
    {
        IsActive = false;
        DOTween.Kill(_rectTransform);
        gameObject.SetActive(false);
    }
}