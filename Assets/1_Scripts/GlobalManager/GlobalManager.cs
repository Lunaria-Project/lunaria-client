using System;
using UnityEngine;

public class GlobalManager : SingletonMonoBehaviourDontDestroy<GlobalManager>
{
    public event Action OnQKeyDown;
    public event Action OnEKeyDown;
    
    private bool _isRunning = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnQKeyDown?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            OnEKeyDown?.Invoke();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        GameTimeManager.Instance.OnEndDay -= OnEndDay;
        GameTimeManager.Instance.OnEndDay += OnEndDay;
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
        _isRunning = false;
        // TODO(지선): 여기서 영수증이 나오게 작업 필요
        LogManager.Log("하루 끝");
        StartDay();
    }
}