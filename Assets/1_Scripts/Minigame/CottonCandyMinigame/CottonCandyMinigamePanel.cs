using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class CottonCandyMinigamePanel : Panel<CottonCandyMinigamePanel>
{
    private bool _isInitialized;
    private float _remainTime;
    private float _minigameTime;
    private int _score;
    private System.Random _random;

    protected void Update()
    {
        if (!_isInitialized) return;
        UpdateTime();
        if (!_isInitialized) return;
        _customerBlock.UpdateCustomer(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnDiscardButtonClick();
        }
    }

    protected override void OnShow(params object[] args)
    {
        GlobalManager.Instance.SetCursor(CursorType.CottonCandyStick);
        var artifactData = GameData.Instance.GetArtifactData(UserData.Instance.EquippedArtifactId);
        var requiredArtifactType = GameData.Instance.GetMinigameInfoData(MinigameType.CottonCandy).EquippedArtifactType;
        if (artifactData.ArtifactType != requiredArtifactType)
        {
            LogManager.LogErrorPack($"CottonCandyMinigamePanel: {requiredArtifactType.GetDisplayName()}이(가) 장착되지 않았습니다.", artifactData.ArtifactType);
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
        GlobalManager.Instance.SetDefaultCursor();
    }

    private void Init()
    {
        var minigameSeconds = GameData.Instance.GetMinigameInfoData(MinigameType.CottonCandy).MinigameSeconds;
        _isInitialized = false;
        _score = 0;
        _remainTime = minigameSeconds;
        _minigameTime = minigameSeconds;
        var seed = Environment.TickCount;
        _random = new System.Random(seed);
        LogManager.Log($"[CottonCandyMinigame] order seed={seed}");

        InitUI();

        PopupManager.Instance.ShowPopup(PopupManager.Type.CountDown, new CountDownPopupParameter { CountDownSeconds = 3 })
            .SetOnHideAction(() => { ShowReady().Forget(); });
    }

    private void UpdateTime()
    {
        _remainTime -= Time.deltaTime;
        _remainTimeImage.fillAmount = (_minigameTime - _remainTime) / _minigameTime;
        _remainTimeTexts.SetTexts(Mathf.RoundToInt(_remainTime).ToNDigits(2));

        if (!(_remainTime <= 0)) return;
        _isInitialized = false;
        OnShowResult();
    }

    private void OnShowResult()
    {
        var parameter = new CottonCandyMinigameResultPopupParameter
        {
            Score = _score,
            RetryAction = Init,
            HideAction = HidePanel,
        };
        PopupManager.Instance.ShowPopup(PopupManager.Type.CottonCandyMinigameResult, parameter).GetTask();
    }

    private CottonCandyOrder GenerateOrder()
    {
        var order = new CottonCandyOrder
        {
            Quantity = _random.Next(_config.OrderQuantityMin, _config.OrderQuantityMax + 1),
            ScoreReward = _random.Next(_config.ScoreRewardMin, _config.ScoreRewardMax + 1),
            Layers = new CottonCandyLayerOrder[CottonCandyMinigameConfig.LayerCount],
        };
        var topLayerIndex = order.Layers.Length - 1;
        for (var i = 0; i < order.Layers.Length; i++)
        {
            order.Layers[i] = new CottonCandyLayerOrder
            {
                Color = (CottonCandyColor)_random.Next(1, 6),
                Shape = i == topLayerIndex ? (CottonCandyShape)_random.Next(1, 4) : CottonCandyShape.Circle,
                Direction = (CottonCandyRotationDirection)_random.Next(0, 2),
                RotationCount = _random.Next(_config.RotationCountMin, _config.RotationCountMax + 1),
            };
        }
        LogManager.LogColor(FormatOrder(order), Color.pink);
        return order;
    }

    private static string FormatOrder(CottonCandyOrder order)
    {
        var layers = string.Empty;
        for (var i = 0; i < order.Layers.Length; i++)
        {
            var l = order.Layers[i];
            layers += $" L{i}={{{l.Color}, {l.Shape}, {l.Direction}, {l.RotationCount}}}";
        }
        return $"[CottonCandyMinigame] Order: qty={order.Quantity}, score={order.ScoreReward},{layers}";
    }

    public void OnSubmitButtonClick()
    {
        if (!_isInitialized) return;
        if (!_cottonCandyBlock.IsComplete) return;

        var isMatch = _cottonCandyBlock.MatchesOrder();
        if (!_customerBlock.TryServeFrontCustomer(out var scoreReward)) return;

        if (isMatch)
        {
            _score += scoreReward;
            SetScoreText();
        }
        LogManager.Log($"[CottonCandyMinigame] 솜사탕 제출: 일치={isMatch}, +{(isMatch ? scoreReward : 0)}, 총점={_score}");
    }

    public void OnDiscardButtonClick()
    {
        if (!_isInitialized) return;
        _cottonCandyBlock.ResetMaking();
        DeselectAllButtons();
    }
}