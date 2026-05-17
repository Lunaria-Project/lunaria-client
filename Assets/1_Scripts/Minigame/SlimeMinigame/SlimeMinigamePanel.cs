using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lunaria;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SlimeType
{
    Level1,
    Level2,
    Level3,
    ToxicLevel1,
    ToxicLevel2,
    ToxicLevel3,
}

public class SlimeMinigamePanel : Panel<SlimeMinigamePanel>
{
    [SerializeField] private Image _remainTimeImage;
    [SerializeField] private Text[] _remainTimeTexts;
    [SerializeField] private Text _countText;
    [SerializeField] private SlimeBlock[] _slimeBlocks;
    [SerializeField] private SlimeMinigameConfig _config;

    private bool _isInitialized;
    private float _remainTime;
    private float _minigameTime;
    private readonly List<Coroutine> _coroutines = new();
    private int _slimeCount;
    private bool _isPaused;

    private void Awake()
    {
        foreach (var slimeBlock in _slimeBlocks)
        {
            slimeBlock.SetOnTouchSlime(OnTouchSlime);
        }
    }

    private void Update()
    {
        if (!_isInitialized) return;
        _remainTime -= Time.deltaTime;
        _remainTimeImage.fillAmount = (_minigameTime - _remainTime) / _minigameTime;
        _remainTimeTexts.SetTexts(Mathf.RoundToInt(_remainTime).ToNDigits(2));
    }

    protected override void OnShow(params object[] args)
    {
        GlobalManager.Instance.SetCursor(CursorType.BubbleGun);
        var artifactData = GameData.Instance.GetArtifactData(UserData.Instance.EquippedArtifactId);
        var requiredArtifactType = GameData.Instance.GetMinigameInfoData(MinigameType.Slime).EquippedArtifactType;
        if (artifactData.ArtifactType != requiredArtifactType)
        {
            LogManager.LogErrorPack($"SlimeMinigamePanel: {requiredArtifactType.GetDisplayName()}이(가) 장착되지 않았습니다.", artifactData.ArtifactType);
            HidePanel();
            return;
        }
        if (UserData.Instance.SlimeGauge < 100)
        {
            LogManager.LogErrorPack("SlimeMinigamePanel: 슬라임 게이지가 부족합니다.", UserData.Instance.SlimeGauge);
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
        foreach (var slime in _slimeBlocks)
        {
            slime.Hide();
        }
        GlobalManager.Instance.SetDefaultCursor();
    }

    private void Init()
    {
        var minigameSeconds = GameData.Instance.GetMinigameInfoData(MinigameType.Slime).MinigameSeconds;
        _isInitialized = false;
        _remainTime = minigameSeconds;
        _minigameTime = minigameSeconds;
        _slimeCount = 0;
        _remainTimeImage.fillAmount = 0;
        _remainTimeTexts.SetTexts(Mathf.RoundToInt(_remainTime).ToNDigits(2));

        foreach (var slime in _slimeBlocks)
        {
            slime.Init();
        }

        PopupManager.Instance.ShowPopup(PopupManager.Type.CountDown, new CountDownPopupParameter { CountDownSeconds = 3 })
            .SetOnHideAction(() =>
            {
                UserData.Instance.AddSlimeGauge(-100);
                _isInitialized = true;
                StartSlimeCoroutine();
                Invoke(nameof(StopSlimeCoroutine), minigameSeconds);
            });
    }

    private void StartSlimeCoroutine()
    {
        _coroutines.Clear();
        for (var i = 0; i < _config.SlimeShowCount; i++)
        {
            _coroutines.Add(StartCoroutine(CoShowSlime()));
        }
    }

    private void StopSlimeCoroutine()
    {
        foreach (var coroutine in _coroutines)
        {
            StopCoroutine(coroutine);
        }
        _coroutines.Clear();
        OnShowResult();
    }

    private IEnumerator CoShowSlime()
    {
        var waitFirstSeconds = _config.GetShowDelayRandomSeconds(SlimeType.Level1);
        yield return UniTask.Delay(TimeUtil.SecondsToMillis(waitFirstSeconds));
        while (true)
        {
            var randomDialogIndex = Random.Range(0, _slimeBlocks.Length);
            var slimeBlock = _slimeBlocks.GetAt(randomDialogIndex);
            if (slimeBlock.IsShowing)
            {
                yield return null;
                continue;
            }

            var slimeType = _config.GetRandomSlime();
            var scale = _config.GetSlimeScale(slimeType);
            var delaySeconds = _config.GetShowDelayRandomSeconds(slimeType);
            var touchCount = _config.GetTouchCount(slimeType);
            var score = _config.GetScore(slimeType);
            yield return slimeBlock.Show(slimeType, touchCount, score, scale, delaySeconds).ToCoroutine();
            yield return UniTask.WaitForSeconds(_config.GetHideDelayRandomSeconds());
        }
    }

    private void OnShowResult()
    {
        _isInitialized = false;
        var parameter = new SlimeMinigameResultPopupParameter
        {
            Score = _slimeCount,
            RetryAction = Init,
            HideAction = HidePanel,
        };
        PopupManager.Instance.ShowPopup(PopupManager.Type.SlimeMinigameResult, parameter).GetTask();
    }

    private void OnTouchSlime(SlimeType type)
    {
        _slimeCount += _config.GetScore(type);
        _countText.SetText(LocalizationKey.Minigame_Slime_Score.Format(_slimeCount.ToPrice()));
    }
}