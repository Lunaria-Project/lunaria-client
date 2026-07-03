using Cysharp.Threading.Tasks;
using Lunaria;
using UnityEngine;

public partial class CottonCandyMinigamePanel
{
    [SerializeField] private Image _remainTimeImage;
    [SerializeField] private Text[] _remainTimeTexts;
    [SerializeField] private Text _scoreText;
    [SerializeField] private CottonCandyOrderBlock currentOrderBlock;
    [SerializeField] private GameObject _colorObject;
    [SerializeField] private CottonCandyMakeButton[] _colorButtons;
    [SerializeField] private GameObject _shapeBlock;
    [SerializeField] private CottonCandyMakeButton[] _shapeButtons;
    [SerializeField] private CottonCandyBlock _cottonCandyBlock;
    [SerializeField] private CottonCandyMinigameCustomerBlock _customerBlock;
    [SerializeField] private CottonCandyMinigameConfig _config;
    [SerializeField] private int _waitReadyMillis;
    [SerializeField] private GameObject _closedObject;
    [SerializeField] private GameObject _openedObject;
    [SerializeField] private GameObject _coverObject;
    [SerializeField] private Text _coverText;

    private CottonCandyColor _selectedColor;
    private CottonCandyShape _selectedShape;

    protected void Awake()
    {
        _customerBlock.SetOnCurrentCustomerChangedAction(OnCurrentCustomerChanged);
        _cottonCandyBlock.SetOnTierAdvancedAction(DeselectColorButtons);
        _cottonCandyBlock.SetOnStateChangedAction(OnBlockStateChanged);
        foreach (var button in _colorButtons)
        {
            button.SetOnClickAction(OnColorButtonClick);
        }
        foreach (var button in _shapeButtons)
        {
            button.SetOnClickAction(OnShapeButtonClick);
        }
    }

    private void InitUI()
    {
        _remainTimeImage.fillAmount = 0;
        _remainTimeTexts.SetTexts(Mathf.RoundToInt(_remainTime).ToNDigits(2));
        SetScoreText();
        currentOrderBlock.Hide();
        DeselectAllButtons();
        _cottonCandyBlock.Init(_config);
        _customerBlock.Init(_config);
        _openedObject.SetActive(false);
        _closedObject.SetActive(true);
    }

    private async UniTask ShowReady()
    {
        _closedObject.SetActive(false);
        _openedObject.SetActive(true);
        await UniTask.Delay(_waitReadyMillis);
        _openedObject.SetActive(false);
        _isInitialized = true;
    }

    private void SetScoreText()
    {
        _scoreText.SetText(_score.ToString());
    }

    private void OnCurrentCustomerChanged(CottonCandyCustomer customer)
    {
        if (customer == null)
        {
            currentOrderBlock.Hide();
            return;
        }
        var order = GenerateOrder();
        customer.SetOrder(order);
        currentOrderBlock.SetOrder(order);
        DeselectAllButtons();
        _cottonCandyBlock.SetOrder(order);
    }

    private void OnBlockStateChanged()
    {
        switch (_cottonCandyBlock.State)
        {
            case CottonCandyMakeState.BeforeOrder:
            {
                _colorObject.SetActive(false);
                _shapeBlock.SetActive(false);
                _coverObject.SetActive(true);
                _coverText.SetText(string.Empty);
                break;
            }
            case CottonCandyMakeState.SelectingColorShape:
            {
                var isLastTier = _cottonCandyBlock.IsLastTier;
                _colorObject.SetActive(true);
                _shapeBlock.SetActive(isLastTier);
                _coverObject.SetActive(true);
                _coverText.SetText(isLastTier ? LocalizationKey.CottonCandyMinigame_Step2 : LocalizationKey.CottonCandyMinigame_Step1);
                break;
            }
            case CottonCandyMakeState.Making:
            {
                _coverObject.SetActive(false);
                break;
            }
            case CottonCandyMakeState.Complete:
            {
                _coverObject.SetActive(true);
                _coverText.SetText(string.Empty);
                break;
            }
        }
    }

    private void DeselectAllButtons()
    {
        DeselectColorButtons();
        foreach (var button in _shapeButtons)
        {
            button.SetSelected(false);
        }
        _selectedShape = CottonCandyShape.None;
    }

    private void DeselectColorButtons()
    {
        foreach (var button in _colorButtons)
        {
            button.SetSelected(false);
        }
        _selectedColor = CottonCandyColor.None;
    }

    private void TryStartMaking()
    {
        if (_selectedColor == CottonCandyColor.None) return;

        var isLastTier = _cottonCandyBlock.IsLastTier;
        if (isLastTier && _selectedShape == CottonCandyShape.None) return;
        _cottonCandyBlock.StartMaking(_selectedColor, isLastTier ? _selectedShape : CottonCandyShape.Circle);
    }

    private void OnColorButtonClick(CottonCandyMakeButton clicked)
    {
        foreach (var button in _colorButtons)
        {
            button.SetSelected(button == clicked);
        }
        _selectedColor = (CottonCandyColor)clicked.Value;
        LogManager.Log($"[CottonCandyMinigame] 색 선택: {_selectedColor}");
        TryStartMaking();
    }

    private void OnShapeButtonClick(CottonCandyMakeButton clicked)
    {
        foreach (var button in _shapeButtons)
        {
            button.SetSelected(button == clicked);
        }
        _selectedShape = (CottonCandyShape)clicked.Value;
        LogManager.Log($"[CottonCandyMinigame] 모양 선택: {_selectedShape}");
        TryStartMaking();
    }
}
