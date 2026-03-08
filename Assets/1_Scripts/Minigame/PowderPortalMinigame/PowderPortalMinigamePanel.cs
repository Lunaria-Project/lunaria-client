using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lunaria;
using UnityEngine;

public class PowderPortalMinigamePanel : Panel<PowderPortalMinigamePanel>
{
    [SerializeField] private Image _remainTimeImage;
    [SerializeField] private Text[] _remainTimeTexts;
    [SerializeField] private Text _scoreText;
    [SerializeField] private float _objectDistance = 150;
    [SerializeField] private PowderPortalMinigameObject[] _objectSlots;
    [SerializeField] private PowderPortalMinigameConfig _config;
    [SerializeField] private RectTransform _upTargetPosition;
    [SerializeField] private RectTransform _downTargetPosition;
    [SerializeField] private RectTransform _disposeTargetPosition;
    [SerializeField] private Text[] _plusScoreTexts;
    [SerializeField] private RectTransform _scoreRectTransform;

    private bool _isInitialized;
    private float _remainTime;
    private float _minigameTime;
    private int _score;
    private bool _isObjectMoving;
    private readonly Queue<PowderPortalMinigameObject> _objectQueue = new();

    private readonly Color _minusColor = Color.red;
    private readonly Color _plusColor = Color.green;

    private void Update()
    {
        if (!_isInitialized) return;

        UpdateTimer();
        UpdateObjectPosition().Forget();
        UpdateInput();
    }

    private void UpdateTimer()
    {
        _remainTime -= Time.deltaTime;
        _remainTimeImage.fillAmount = (_minigameTime - _remainTime) / _minigameTime;
        _remainTimeTexts.SetTexts(Mathf.RoundToInt(_remainTime).ToNDigits(2));

        if (_remainTime <= 0)
        {
            _isInitialized = false;
            ShowResult();
        }
    }

    private async UniTask UpdateObjectPosition()
    {
        if (_isObjectMoving) return;
        var frontObject = _objectQueue.Peek();
        if (frontObject.AnchoredPosition.x == 0) return;

        foreach (var objectSlot in _objectSlots)
        {
            objectSlot.MoveToPosition(objectSlot.AnchoredPosition.x - _objectDistance);
        }
        _isObjectMoving = true;
        await UniTask.Delay(_config.AdvanceAnimDurationMillis);
        _isObjectMoving = false;
    }

    private void UpdateInput()
    {
        if (_isObjectMoving) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnKeyInput(PowderPortalDirection.Left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnKeyInput(PowderPortalDirection.Right);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnKeyInput(PowderPortalDirection.Space);
        }
    }

    protected override void OnShow(params object[] args)
    {
        var artifactData = GameData.Instance.GetArtifactData(UserData.Instance.EquippedArtifactId);
        if (artifactData.ArtifactType != ArtifactType.Powder)
        {
            LogManager.LogErrorPack("PowderPortalMinigamePanel: 파우더가 장착되지 않았습니다.", artifactData.ArtifactType);
            HidePanel();
            return;
        }
        GlobalManager.Instance.IsMinigamePlaying = true;
        Init();
    }

    protected override void OnHide()
    {
        GlobalManager.Instance.IsMinigamePlaying = false;
        _isInitialized = false;
        foreach (var slot in _objectSlots)
        {
            slot.Hide();
        }
    }

    private void Init()
    {
        _isInitialized = false;
        _isObjectMoving = false;
        _score = 0;
        _remainTime = _config.MinigameSeconds;
        _minigameTime = _config.MinigameSeconds;
        _remainTimeImage.fillAmount = 0;
        _remainTimeTexts.SetTexts(Mathf.RoundToInt(_remainTime).ToNDigits(2));
        //_scoreText.SetText(_score.ToString());
        _plusScoreTexts.SetActiveAll(false);

        var initPositionX = _objectDistance * 6;
        for (var i = 0; i < _objectSlots.Length; i++)
        {
            var direction = (PowderPortalDirection)Random.Range(0, 3);
            _objectSlots[i].Init(direction, initPositionX + _objectDistance * i);
            _objectQueue.Enqueue(_objectSlots[i]);
        }

        PopupManager.Instance.ShowPopup(PopupManager.Type.CountDown, new CountDownPopupParameter { CountDownSeconds = 3 })
            .SetOnHideAction(() => { _isInitialized = true; });
    }

    private void OnKeyInput(PowderPortalDirection direction)
    {
        var frontSlot = _objectQueue.Peek();
        if (!frontSlot.IsActive) return;
        if (frontSlot.AnchoredPosition.x != 0) return;

        var isCorrect = frontSlot.CorrectDirection == direction;
        _score += _config.GetScore(isCorrect);
        _score = Mathf.Max(_score, 0);
        //_scoreText.SetText(_score.ToString());

        var targetPosition = direction switch
        {
            PowderPortalDirection.Left  => _upTargetPosition.position,
            PowderPortalDirection.Right => _downTargetPosition.position,
            PowderPortalDirection.Space => _disposeTargetPosition.position,
            _                           => Vector3.zero
        };
        ShowScoreText(_config.GetScore(isCorrect));
        var duration = direction != PowderPortalDirection.Space ? _config.SendAnimDuration : _config.SendAnimDuration2;
        frontSlot.PlaySendAnimation(targetPosition, duration, OnSendComplete);
        _objectQueue.Dequeue();
        return;

        void OnSendComplete(PowderPortalMinigameObject spawnObject)
        {
            var maxX = 0f;
            foreach (var obj in _objectQueue)
            {
                if (obj.AnchoredPosition.x > maxX)
                {
                    maxX = obj.AnchoredPosition.x;
                }
            }
            var randomDirection = (PowderPortalDirection)Random.Range(0, 3);
            spawnObject.Init(randomDirection, maxX + _objectDistance);
            _objectQueue.Enqueue(spawnObject);
        }
    }

    private void ShowScoreText(int score)
    {
        Text availableText = null;
        foreach (var text in _plusScoreTexts)
        {
            if (text.gameObject.activeSelf) continue;
            availableText = text;
            break;
        }
        if (availableText == null) return;

        var rectTransform = (RectTransform)availableText.transform;
        rectTransform.position = _scoreRectTransform.position;
        availableText.SetText(score > 0 ? $"+{score}" : score.ToString());
        availableText.color = score > 0 ? _plusColor : _minusColor;
        availableText.SetActive(true);

        var startPos = rectTransform.anchoredPosition;
        DOTween.Kill(availableText);
        DOTween.Sequence()
            .Append(rectTransform.DOAnchorPos(startPos + Vector2.up * 80f, 0.5f).SetEase(Ease.OutQuad))
            .AppendInterval(0.2f)
            .Append(availableText.DOFade(0f, 0.3f))
            .OnComplete(() => { availableText.SetActive(false); });
    }

    private void ShowResult()
    {
        var parameter = new PowderPortalMinigameResultPopupParameter
        {
            Score = _score,
            RetryAction = Init,
            HideAction = HidePanel,
        };
        PopupManager.Instance.ShowPopup(PopupManager.Type.PowderPortalMinigameResult, parameter).GetTask();
    }
}