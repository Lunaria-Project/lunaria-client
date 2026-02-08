using System;
using DG.Tweening;
using Lunaria;
using UnityEngine;

public partial class GlobalManager : SingletonMonoBehaviour<GlobalManager>
{
    [SerializeField] private Image _toastMessageBackground;
    [SerializeField] private Text _toastMessageText;

    public event Action OnApplicationPaused;
    public event Action OnApplicationResume;
    public event Action OnQKeyDown;
    public event Action OnEKeyDown;

    private bool _isRunning = false;
    private Color _transparentColor = new Color(1, 1, 1, 0);

    protected override void Awake()
    {
        base.Awake();
        GameTimeManager.Instance.OnEndDay -= OnEndDay;
        GameTimeManager.Instance.OnEndDay += OnEndDay;
        OnApplicationResume -= HideUserCursor;
        OnApplicationResume += HideUserCursor;
    }

    protected override void Start()
    {
        base.Start();
        _toastMessageBackground.SetActive(false);
    }

    protected override void OnDestroy()
    {
        DOTween.Kill(_toastMessageBackground);
        OnApplicationResume -= HideUserCursor;
    }

    protected override void Update()
    {
        base.Update();
        UpdateCursor();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnQKeyDown?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            OnEKeyDown?.Invoke();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            OnApplicationResume?.Invoke();
        }
        OnApplicationPaused?.Invoke();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            OnApplicationPaused?.Invoke();
        }
        OnApplicationResume?.Invoke();
    }

    #region Day

    public void StartDay()
    {
        if (_isRunning)
        {
            LogManager.LogError("이미 게임을 진행중입니다.");
            return;
        }
        _isRunning = true;
        GameTimeManager.Instance.StartDay();
    }

    private void OnEndDay()
    {
        if (!_isRunning) return;
        _isRunning = false;
        // TODO(지선): 여기서 영수증이 나오게 작업 필요
        LogManager.Log("하루 끝");
        StartDay();
    }

    #endregion

    #region ToastMessage

    public void ShowToastMessage(string message, float showTimeSeconds = 0.8f)
    {
        _toastMessageBackground.color = new Color(_toastMessageBackground.color.r, _toastMessageBackground.color.g, _toastMessageBackground.color.b, 0);
        _toastMessageText.color = _transparentColor;

        _toastMessageBackground.SetActive(true);
        _toastMessageText.SetText(message);

        DOTween.Kill(_toastMessageBackground);
        var sequence = DOTween.Sequence().SetId(_toastMessageBackground);
        sequence.Append(_toastMessageBackground.DOFade(0.78f, 0.2f))
            .Join(_toastMessageText.DOFade(1f, 0.2f))
            .AppendInterval(showTimeSeconds)
            .Append(_toastMessageBackground.DOFade(0f, 0.2f))
            .Join(_toastMessageText.DOFade(0f, 0.2f))
            .OnComplete(() => { _toastMessageBackground.SetActive(false); });
    }

    #endregion
}