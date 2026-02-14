using System;
using System.Collections.Generic;
using DG.Tweening;
using Lunaria;
using UnityEngine;

public partial class GlobalManager : SingletonMonoBehaviour<GlobalManager>
{
    [SerializeField] private Camera _globalCamara;
    [SerializeField] private Image _toastMessageBackground;
    [SerializeField] private Text _toastMessageText;

    public event Action OnApplicationPaused;
    public event Action OnApplicationResume;
    public event Action OnQKeyDown;
    public event Action OnEKeyDown;
    public Camera GlobalCamera => _globalCamara;
    public bool FollowPlayer { get; private set; }

    private bool _isRunning = false;
    private List<NpcInfo> _npcInfos = new();
    private readonly Color _transparentColor = new Color(1, 1, 1, 0);

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
        base.OnDestroy();
        DOTween.Kill(_toastMessageBackground);
        OnApplicationResume -= HideUserCursor;
    }

    protected override void Update()
    {
        base.Update();
        UpdateCursor();
        UpdateCompassUIs();
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
        else
        {
            OnApplicationPaused?.Invoke();
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            OnApplicationPaused?.Invoke();
        }
        else
        {
            OnApplicationResume?.Invoke();
        }
    }

    public void InitUI()
    {
        _compassUIs.SetActiveAll(false);
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

    #region MapManager

    public void OnChangeMap(IList<NpcInfo> npcInfo)
    {
        SetDefaultCursor();
        InitCompassUIs(npcInfo);
    }

    public void UpdateCameraPosition(Vector3 playerPosition)
    {
        if (!FollowPlayer) return;
        var cameraTransform = GlobalCamera.transform;
        cameraTransform.position = new Vector3(playerPosition.x, playerPosition.y, cameraTransform.position.z);
    }

    private void SetCameraSize(float size)
    {
        GlobalCamera.orthographicSize = size;
    }

    private void ResetCamaraPosition()
    {
        SetCamaraPosition(0, 0);
    }

    private void SetCamaraPosition(float x, float y)
    {
        var cameraTransform = GlobalCamera.transform;
        cameraTransform.position = new Vector3(x, y, cameraTransform.position.z);
    }

    #endregion
}