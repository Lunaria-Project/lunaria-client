using System.Collections.Generic;
using Lunaria;
using UnityEngine;

public class PowderPortalMinigamePanel : Panel<PowderPortalMinigamePanel>
{
    [SerializeField] private MinigameRemainTime _minigameRemainTime;
    [SerializeField] private float _objectDistance = 150;
    [SerializeField] private PowderPortalMinigameObject[] _objectSlots;
    [SerializeField] private PowderPortalMinigameConfig _config;
    [SerializeField] private Animator _disposeAnimator;
    [SerializeField] private GameObject _upEffectObject;
    [SerializeField] private GameObject _downEffectObject;
    [SerializeField] private GameObject _disposeEffectObject;
    [SerializeField] private GameObject _plusEffectObject;
    [SerializeField] private GameObject _feverEffectObject;
    [SerializeField] private PowderPortalScoreText[] _scoreTexts;
    [SerializeField] private Image _railImage;

    private const float FadeOutDuration = 0.2f;
    private const int FeverComboCount = 10;
    private const float FeverBlockRemainSeconds = 1f;
    private static readonly int StartTriggerHash = Animator.StringToHash("start");

    private bool _isInitialized;
    private float _remainTime;
    private float _minigameTime;
    private int _score;
    private int _combo;
    private bool _isFever;
    private int _scoreTextIndex;
    private Material _railMaterial;
    private readonly Queue<PowderPortalMinigameObject> _objectQueue = new();

    protected void Awake()
    {
        _railMaterial = new Material(_railImage.material);
        _railImage.material = _railMaterial;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        UpdateTimer();
        UpdateObjectPosition();
        UpdateInput();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            _remainTime = 0f;
        }
#endif
    }

    private void UpdateTimer()
    {
        _remainTime -= Time.deltaTime;
        _minigameRemainTime.SetRemainTime(_remainTime, _minigameTime);

        if (_remainTime <= 0)
        {
            _isInitialized = false;
            ShowResult();
        }
    }

    private void UpdateObjectPosition()
    {
        var frontObject = _objectQueue.Peek();
        if (frontObject.AnchoredPosition.x <= 0) return;

        var moveX = Mathf.Min(_config.RailSpeed * Time.deltaTime, frontObject.AnchoredPosition.x);
        foreach (var objectSlot in _objectQueue)
        {
            objectSlot.MoveBy(-moveX);
        }
        var railRectTransform = _railImage.rectTransform;
        _railMaterial.mainTextureOffset += Vector2.right * (moveX / (railRectTransform.rect.width * railRectTransform.localScale.x));
    }

    private void UpdateInput()
    {
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
        var requiredArtifactType = GameData.Instance.GetMinigameInfoData(MinigameType.PowderPortal).EquippedArtifactType;
        if (artifactData.ArtifactType != requiredArtifactType)
        {
            LogManager.LogErrorPack($"PowderPortalMinigamePanel: {requiredArtifactType.GetDisplayName()}이(가) 장착되지 않았습니다.", artifactData.ArtifactType);
            HidePanel();
            return;
        }
        GlobalManager.Instance.IsMinigamePlaying = true;
        GameTimeManager.Instance.Pause(this);
        Init();
    }

    protected override void OnHide()
    {
        GlobalManager.Instance.IsMinigamePlaying = false;
        GameTimeManager.Instance.Resume(this);
        _isInitialized = false;
        foreach (var slot in _objectSlots)
        {
            slot.Hide();
        }
    }

    private void Init()
    {
        _isInitialized = false;
        _score = 0;
        _combo = 0;
        _isFever = false;
        _scoreTextIndex = 0;
        var minigameSeconds = GameData.Instance.GetMinigameInfoData(MinigameType.PowderPortal).MinigameSeconds;
        _remainTime = minigameSeconds;
        _minigameTime = minigameSeconds;
        _minigameRemainTime.SetRemainTime(_remainTime, _minigameTime);
        //_scoreText.SetText(_score.ToString());
        _scoreTexts.SetActiveAll(false);
        _upEffectObject.SetActive(false);
        _downEffectObject.SetActive(false);
        _disposeEffectObject.SetActive(false);
        _plusEffectObject.SetActive(false);
        _feverEffectObject.SetActive(false);

        _objectQueue.Clear();
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
        _combo = isCorrect ? _combo + 1 : 0;
        if (!isCorrect)
        {
            _isFever = false;
        }
        else if (!_isFever && _combo >= FeverComboCount && _remainTime >= FeverBlockRemainSeconds)
        {
            _isFever = true;
        }
        //_scoreText.SetText(_score.ToString());

        var effectObject = direction switch
        {
            PowderPortalDirection.Left  => _upEffectObject,
            PowderPortalDirection.Right => _downEffectObject,
            _                           => _disposeEffectObject
        };
        effectObject.SetActive(false);
        effectObject.SetActive(true);
        if (direction is PowderPortalDirection.Space)
        {
            _disposeAnimator.SetTrigger(StartTriggerHash);
        }
        ShowScoreText(_config.GetScore(isCorrect));
        frontSlot.PlayFadeOutAnimation(FadeOutDuration, OnSendComplete);
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
        _scoreTexts.GetAt(_scoreTextIndex).Play(score, _isFever);
        _scoreTextIndex = (_scoreTextIndex + 1) % _scoreTexts.Length;

        if (score > 0)
        {
            var effectObject = _isFever ? _feverEffectObject : _plusEffectObject;
            effectObject.SetActive(false);
            effectObject.SetActive(true);
        }
    }

    protected void OnDestroy()
    {
        if (_railMaterial != null)
        {
            Destroy(_railMaterial);
        }
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