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
    [SerializeField] private GameObject[] _healthGroupObjects;
    [SerializeField] private GameObject[] _healthObjects;
    [SerializeField] private GameObject[] _healthEffectObjects;
    [SerializeField] private Vector2 _showPosition = Vector2.up;
    [SerializeField] private RectTransform _scoreRectTransform;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _destroyEffect;
    [SerializeField] private GameObject _toxicDestroyEffect;

    private readonly Color _minusColor = Color.red;
    private readonly Color _plusColor = Color.green;

    public bool IsShowing { get; private set; }

    private Action<SlimeType> _onTouchSlime;
    private SlimeType _slimeType;
    private int _remainTouchCount;
    private int _score;

    private bool IsToxicSlime => _slimeType is SlimeType.ToxicLevel1 or SlimeType.ToxicLevel2 or SlimeType.ToxicLevel3;

    public void SetOnTouchSlime(Action<SlimeType> onTouchSlime)
    {
        _onTouchSlime = onTouchSlime;
    }

    public void Init()
    {
        IsShowing = false;
        _healthGroupObjects.SetActiveAll(false);
        _healthObjects.SetActiveAll(false);
        _healthEffectObjects.SetActiveAll(false);
        _destroyEffect.SetActive(false);
        _toxicDestroyEffect.SetActive(false);
        _touchButton.SetActive(false);
        _scoreText.SetActive(false);
        _slimeImageRectTransform.SetActive(false);
        _canvas.overrideSorting = true;
        _canvas.sortingLayerName = NameContainer.SortingLayer.UI;
    }

    public async UniTask Show(SlimeType type, int touchCount, int score, float scale, float showTime)
    {
        _slimeType = type;
        _remainTouchCount = touchCount;
        _score = score;
        IsShowing = true;
        _rectTransform.anchoredPosition = Vector2.zero;
        _touchButton.SetActive(true);
        _scoreText.SetActive(false);
        _slimeImageRectTransform.SetActive(true);

        _slimeImage.SetSprite(ResourceManager.Instance.LoadSlimeMinigameSprite(type));
        _slimeImageRectTransform.SetLocalScale(scale);
        _healthGroupObjects.SetActiveAll(false);
        _healthEffectObjects.SetActiveAll(false);
        _destroyEffect.SetActive(false);
        _toxicDestroyEffect.SetActive(false);
        for (var i = 0; i < touchCount; i++)
        {
            _healthGroupObjects[i].SetActive(true);
        }
        RefreshHealthObjects();

        DOTween.Kill(this);
        await DOTween.Sequence().SetId(this)
            .Append(_rectTransform.DOAnchorPos(_showPosition, 0.3f).SetEase(Ease.OutQuad))
            .AppendInterval(showTime)
            .Append(_rectTransform.DOAnchorPos(Vector2.zero, 0.2f)).SetEase(Ease.OutQuad)
            .JoinCallback(() =>
            {
                _touchButton.SetActive(false);
                _healthObjects.SetActiveAll(false);
            })
            .AppendOnComplete(Hide)
            .GetTask();
    }

    public void Hide()
    {
        DOTween.Kill(this);
        IsShowing = false;
        _touchButton.SetActive(false);
        _healthGroupObjects.SetActiveAll(false);
        _healthObjects.SetActiveAll(false);
        _slimeImageRectTransform.SetActive(false);
    }

    private void RefreshHealthObjects()
    {
        _healthObjects.SetActiveAll(false);
        if (IsToxicSlime) return;
        for (var i = 0; i < _healthObjects.Length; i++)
        {
            _healthObjects[i].SetActive(i < _remainTouchCount);
        }
    }

    private void ShowHealthEffect(int index)
    {
        if (IsToxicSlime) return;

        var healthEffectObject = _healthEffectObjects.GetAt(index);
        if (healthEffectObject == null) return;

        healthEffectObject.SetActive(false);
        healthEffectObject.SetActive(true);
    }

    private void ShowDestroyEffect()
    {
        var destroyEffect = IsToxicSlime ? _toxicDestroyEffect : _destroyEffect;
        destroyEffect.SetActive(false);
        destroyEffect.SetActive(true);
    }

    public void OnTouchButtonClick()
    {
        _remainTouchCount--;
        RefreshHealthObjects();
        ShowHealthEffect(_remainTouchCount);
        if (_remainTouchCount > 0) return;

        _onTouchSlime.Invoke(_slimeType);
        Hide();
        ShowDestroyEffect();

        _scoreText.SetActive(true);
        _scoreText.SetText(_score.ToString());
        _scoreText.color = _score < 0 ? _minusColor : _plusColor;

        _scoreRectTransform.position = _rectTransform.position;
        DOTween.Kill(_scoreText);
        DOTween.Sequence().SetId(_scoreText)
            .Append(_scoreRectTransform.DOAnchorPos(_scoreRectTransform.anchoredPosition + Vector2.up * 50, 0.3f).SetEase(Ease.OutQuad))
            .OnComplete(() =>
            {
                IsShowing = false;
                _scoreText.SetActive(false);
            });
    }
}