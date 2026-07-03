using System;
using System.Collections.Generic;
using Lunaria;
using UnityEngine;

public enum CottonCandyMakeState
{
    BeforeOrder,
    SelectingColorShape,
    Making,
    TierComplete,
    Failed,
    Complete,
}

public class CottonCandyBlock : MonoBehaviour
{
    private const float TierAlpha = 0.85f;
    private const float FailedMinScale = 0.15f;

    [System.Serializable]
    public struct Rail
    {
        public GameObject OnObject;
        public GameObject OffObject;
    }

    [SerializeField] private RectTransform _discCenter;
    [SerializeField] private RectTransform _cottonCandy;
    [SerializeField] private PointerOverImageDetector _railDetector;
    [SerializeField] private Rail[] _rails;
    [SerializeField] private Image[] _tiers;
    [SerializeField] private GameObject _spacebarIndicator;

    private CottonCandyMinigameConfig _config;
    private CottonCandyOrder _order;
    private CottonCandyMakeState _state;
    private int _currentTier;
    private float _progress;
    private float _stepTurns;
    private float _currentStepTurns;
    private float _prevAngle;
    private bool _wasOnRail;
    private Action _onTierAdvanced;
    private Action _onStateChanged;
    private readonly List<(CottonCandyColor Color, CottonCandyShape Shape)> _madeLayers = new();

    private bool IsMaking => _state is CottonCandyMakeState.Making or CottonCandyMakeState.TierComplete;
    public bool IsComplete => _state == CottonCandyMakeState.Complete;
    public CottonCandyMakeState State => _state;
    public bool IsLastTier => _currentTier >= _tiers.Length - 1;

    public void SetOnTierAdvancedAction(Action action)
    {
        _onTierAdvanced = action;
    }

    public void SetOnStateChangedAction(Action action)
    {
        _onStateChanged = action;
    }

    protected void Update()
    {
        var mousePos = (Vector2)Input.mousePosition;
        _cottonCandy.position = mousePos;
        UpdateRotation(mousePos);
        UpdateNextRailInput();
    }

    public void Init(CottonCandyMinigameConfig config)
    {
        _config = config;
        ResetMaking();
    }

    public void StartMaking(CottonCandyColor color, CottonCandyShape shape)
    {
        if (_state != CottonCandyMakeState.SelectingColorShape) return;
        _madeLayers.Add((color, shape));
        _tiers.GetAt(_currentTier).SetSprite(ResourceManager.Instance.LoadCottonCandyMinigameSprite(_currentTier, color, shape));
        LogManager.LogColor("[CottonCandyMinigame] 선택 완료: " + color + " " + shape, Color.pink);
        SetState(CottonCandyMakeState.Making);
    }

    public void SetOrder(CottonCandyOrder order)
    {
        var tierCount = Mathf.Min(_tiers.Length, _rails.Length);
        if (order.Layers.Length != tierCount)
        {
            LogManager.LogErrorPack("[CottonCandyMinigame] CottonCandyBlock: 단 개수 불일치", order.Layers.Length, tierCount);
            return;
        }
        _order = order;
        ResetMaking();
    }

    public bool MatchesOrder()
    {
        if (_order == null) return false;
        if (_madeLayers.Count != _order.Layers.Length) return false;
        for (var i = 0; i < _order.Layers.Length; i++)
        {
            var layer = _order.Layers.GetAt(i);
            if (_madeLayers[i].Color != layer.Color) return false;
            if (_madeLayers[i].Shape != layer.Shape) return false;
        }
        return true;
    }

    public void ResetMaking()
    {
        _currentTier = 0;
        _progress = 0f;
        _stepTurns = 0f;
        _currentStepTurns = 0f;
        _wasOnRail = false;
        _madeLayers.Clear();
        foreach (var tier in _tiers)
        {
            tier.transform.localScale = Vector3.zero;
            tier.color = new Color(1f, 1f, 1f, TierAlpha);
        }
        SetState(_order == null ? CottonCandyMakeState.BeforeOrder : CottonCandyMakeState.SelectingColorShape);
    }

    private void SetState(CottonCandyMakeState state)
    {
        LogManager.LogColor($"[CottonCandyMinigame] 상태: {_state} → {state}", Color.yellow);
        _state = state;

        switch (state)
        {
            case CottonCandyMakeState.TierComplete:
            {
                _spacebarIndicator.SetActive(true);
                break;
            }
            case CottonCandyMakeState.Failed:
            {
                _spacebarIndicator.SetActive(false);
                var burnt = _config.BurntColor;
                burnt.a = TierAlpha;
                foreach (var tier in _tiers)
                {
                    tier.color = burnt;
                }
                var currentTierTransform = _tiers.GetAt(_currentTier).transform;
                var currentScale = currentTierTransform.localScale.x;
                if (currentScale <= Mathf.Epsilon)
                {
                    currentTierTransform.localScale = Vector3.one * FailedMinScale;
                }
                break;
            }
            default:
            {
                _spacebarIndicator.SetActive(false);
                break;
            }
        }

        UpdateRails();
        _onStateChanged?.Invoke();
        return;

        void UpdateRails()
        {
            for (var i = 0; i < _rails.Length; i++)
            {
                var rail = _rails.GetAt(i);
                var isOn = IsMaking && i == _currentTier;
                rail.OnObject.SetActive(isOn);
                rail.OffObject.SetActive(!isOn);
            }
        }
    }

    private void UpdateRotation(Vector2 mousePos)
    {
        if (_state != CottonCandyMakeState.Making || !_railDetector.IsPointerOver)
        {
            _wasOnRail = false;
            return;
        }

        var angle = GetAngle(mousePos);
        if (!_wasOnRail)
        {
            _wasOnRail = true;
            _prevAngle = angle;
            return;
        }

        var deltaTurns = Mathf.DeltaAngle(_prevAngle, angle) / 360f;
        _prevAngle = angle;

        var requiredSign = _order.Layers.GetAt(_currentTier).Direction == CottonCandyRotationDirection.CounterClockwise ? 1f : -1f;
        _stepTurns += deltaTurns * requiredSign;

        if (_stepTurns <= _currentStepTurns - _config.AcceptTurns)
        {
            SetState(CottonCandyMakeState.Failed);
            return;
        }

        _currentStepTurns = Mathf.Max(_currentStepTurns, _stepTurns);

        var rotationCount = Mathf.Max(1, _order.Layers.GetAt(_currentTier).RotationCount);
        _progress = Mathf.Clamp01(_currentStepTurns / rotationCount);
        _tiers.GetAt(_currentTier).transform.localScale = Vector3.one * _progress;

        if (_progress >= 1f)
        {
            OnTierComplete();
        }
    }

    private void OnTierComplete()
    {
        if (IsLastTier)
        {
            SetState(CottonCandyMakeState.Complete);
            return;
        }
        SetState(CottonCandyMakeState.TierComplete);
    }

    private void UpdateNextRailInput()
    {
        if (_state != CottonCandyMakeState.TierComplete) return;
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        _currentTier++;
        _progress = 0f;
        _stepTurns = 0f;
        _currentStepTurns = 0f;
        SetState(CottonCandyMakeState.SelectingColorShape);
        _onTierAdvanced?.Invoke();
    }


    private float GetAngle(Vector2 mousePos)
    {
        var dir = mousePos - (Vector2)_discCenter.position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}