using System;
using UnityEngine;

public class GameTimeManager : SingletonMonoBehaviour<GameTimeManager>
{
    public event Action OnIntervalChanged;
    public event Action OnTimeSecondsChanged;
    public event Action OnEndDay;

    public GameTime CurrentGameTime => _currentGameTime;
    public bool IsInitialized => _isInitialized;

    private GameTime _currentGameTime;
    private bool _isPaused;
    private bool _isInitialized;
    private double _currentDaySeconds;
    private int _currentIntervalIndex = -1;
    private int _timeSpeedMultiplier;

    private void Update()
    {
        if (!_isInitialized || _isPaused) return;
        if (GameSetting.Instance.SecondsPerGameHour <= 0) return;

        // 현실 Δt → 게임 초 환산: (1시간=3600초) * (Δt / 현실_초당_게임1시간)
        _currentDaySeconds += Time.deltaTime * (TimeUtil.MinutesPerHour * TimeUtil.SecondsPerMinute) * _timeSpeedMultiplier / GameSetting.Instance.SecondsPerGameHour;

        var currentDaySecondsToInt = Mathf.FloorToInt((float)_currentDaySeconds);
        var tenMinuteIndex = TimeUtil.GetTenMinuteIntervalIndex(currentDaySecondsToInt);
        _currentGameTime.SetTime(currentDaySecondsToInt);

        OnTimeSecondsChanged?.Invoke();

        if (_currentIntervalIndex == tenMinuteIndex) return;

        _currentIntervalIndex = tenMinuteIndex;
        OnIntervalChanged?.Invoke();

        if (_currentDaySeconds <= GameSetting.Instance.EndGameTimeSeconds) return;
        Clear();
        OnEndDay?.Invoke();
    }

    public void Clear()
    {
        _isInitialized = false;
        _currentGameTime = GameTime.Invalid;
        _currentDaySeconds = 0;
        _currentIntervalIndex = -1;
    }

    public void StartDay()
    {
        var startGameTime = new GameTime();
        startGameTime.SetTime(GameSetting.Instance.StartGameTimeSeconds);

        _timeSpeedMultiplier = 1;
#if UNITY_EDITOR
        var userData = ResourceManager.Instance.LoadScriptableObject<UserDataCheatAsset>("user_data_cheat_asset");
        _timeSpeedMultiplier *= userData.TimeSpeedMultiplier <= 0 ? 1 : userData.TimeSpeedMultiplier;
#endif

        _currentGameTime = startGameTime;
        _currentDaySeconds = _currentGameTime.TotalSeconds;
        _currentIntervalIndex = TimeUtil.GetTenMinuteIntervalIndex(_currentGameTime.TotalSeconds);

        _isInitialized = true;
        _isPaused = false;
    }

    public void Pause()
    {
        if (!_isInitialized) return;
        _isPaused = true;
    }

    public void Resume()
    {
        if (!_isInitialized) return;
        _isPaused = false;
    }
}