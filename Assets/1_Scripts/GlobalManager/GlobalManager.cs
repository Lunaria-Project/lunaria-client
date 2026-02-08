using System;
using UnityEngine;

public partial class GlobalManager : SingletonMonoBehaviour<GlobalManager>
{
    public event Action OnApplicationPaused;
    public event Action OnApplicationResume;
    public event Action OnQKeyDown;
    public event Action OnEKeyDown;

    private bool _isRunning = false;

    protected override void Awake()
    {
        base.Awake();
        GameTimeManager.Instance.OnEndDay -= OnEndDay;
        GameTimeManager.Instance.OnEndDay += OnEndDay;
        OnApplicationResume -= HideUserCursor;
        OnApplicationResume += HideUserCursor;
    }

    protected override void OnDestroy()
    {
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
}