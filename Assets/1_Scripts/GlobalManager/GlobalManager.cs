using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lunaria;
using UnityEngine;

public partial class GlobalManager : SingletonMonoBehaviour<GlobalManager>
{
    [SerializeField] private Camera _globalCamara;
    [SerializeField] private Image _toastMessageBackground;
    [SerializeField] private Text _toastMessageText;

    public GameData.LocalType LocalType = GameData.LocalType.Ko;

    public event Action OnApplicationPaused;
    public event Action OnApplicationResume;
    public bool IsMinigamePlaying;

    private bool _isDayRunning;
    private ShopType _currentShopType;
    private readonly Color _transparentColor = new Color(1, 1, 1, 0);
    private const int DefaultCameraSize = 700;

    protected override void Awake()
    {
        base.Awake();
        GameTimeManager.Instance.OnIntervalChanged -= OnIntervalChanged;
        GameTimeManager.Instance.OnIntervalChanged += OnIntervalChanged;
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
        UpdateKey();
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

    #region Day

    public void StartDay()
    {
        if (_isDayRunning)
        {
            LogManager.LogError("이미 게임을 진행중입니다.");
            return;
        }
        _isDayRunning = true;
        GameTimeManager.Instance.StartDay();
    }

    private void OnEndDay()
    {
        if (!_isDayRunning) return;
        _isDayRunning = false;
        // TODO(지선): 여기서 영수증이 나오게 작업 필요
        LogManager.Log("하루 끝");
        UserData.Instance.AddDay();
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

    public bool CanPlayerMove()
    {
        if (CutsceneManager.Instance.IsPlaying) return false;
        if (GameTimeManager.Instance.IsPaused) return false;
        if (IsMinigamePlaying) return false;
        foreach (var popup in PopupManager.Instance.PopupList)
        {
            if (!CanPlayerMoveWithPopup(popup.PopupType)) return false;
        }
        return true;

        bool CanPlayerMoveWithPopup(PopupManager.Type type)
        {
            return type switch
            {
                PopupManager.Type.Shortcut => true,
                _                          => false,
            };
        }
    }

    public void OnChangeMap(MapType type)
    {
        MapManager.Instance.SetMap(type);
        SetDefaultCursor();
    }

    public void UpdateCameraPosition(Vector3 playerPosition)
    {
        var cameraTransform = _globalCamara.transform;
        cameraTransform.position = new Vector3(playerPosition.x, playerPosition.y, cameraTransform.position.z);
    }

    private void SetCameraSize(float size)
    {
        _globalCamara.orthographicSize = DefaultCameraSize * size;
    }

    private void ResetCamaraPosition()
    {
        SetCamaraPosition(Vector2.zero);
    }

    private void SetCamaraPosition(Vector2 position)
    {
        var cameraTransform = _globalCamara.transform;
        cameraTransform.position = new Vector3(position.x, position.y, cameraTransform.position.z);
    }

    #endregion

    #region Shortcut

    private void OnIntervalChanged()
    {
        if (_currentShopType == ShopType.None) return;

        var shopInfoData = GameData.Instance.GetShopInfoDataByShopType(_currentShopType);
        var currentTime = GameTimeManager.Instance.CurrentGameTime;
        var currentHHMM = currentTime.Hours * 100 + currentTime.MinutesForUI;
        if (currentHHMM < shopInfoData.EndTime) return;

        var shortcutType = _currentShopType switch
        {
            ShopType.PowderShop      => ShortcutType.ShoppingSquareByPowderShop,
            ShopType.BeddingShop     => ShortcutType.ShoppingSquareByBeddingShop,
            ShopType.CottonCandyShop => ShortcutType.ShoppingSquareByCottonCandyShop,
            _                        => ShortcutType.ShoppingSquare,
        };

        GameTimeManager.Instance.Pause(this);
        var parameter = new SystemOneButtonParameter()
        {
            Description = LocalizationKey.ShopEndPopup_Description,
            ConfirmButtonText = LocalizationKey.ConfirmButton,
            OnConfirm = () =>
            {
                ShortcutInvoke(shortcutType).Forget();
                GameTimeManager.Instance.Resume(this);
            },
        };
        PopupManager.Instance.ShowPopup(PopupManager.Type.SystemButton, parameter);
    }

    private void ClearPreviousShortcut()
    {
        _currentShopType = ShopType.None;
    }

    #endregion
}