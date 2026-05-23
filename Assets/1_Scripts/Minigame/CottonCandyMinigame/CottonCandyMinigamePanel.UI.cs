using Lunaria;
using UnityEngine;

public partial class CottonCandyMinigamePanel
{
    [SerializeField] private Image _remainTimeImage;
    [SerializeField] private Text[] _remainTimeTexts;
    [SerializeField] private Text _scoreText;
    [SerializeField] private CottonCandyOrderBlock currentOrderBlock;
    [SerializeField] private CottonCandyMakeButton[] _colorButtons;
    [SerializeField] private CottonCandyMakeButton[] _shapeButtons;
    [SerializeField] private CottonCandyMinigameCustomerBlock _customerBlock;
    [SerializeField] private CottonCandyMinigameConfig _config;

    protected void Awake()
    {
        _customerBlock.SetOnCurrentCustomerChangedAction(OnCurrentCustomerChanged);
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
        _customerBlock.Init(_config);
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
    }

    private void DeselectAllButtons()
    {
        foreach (var button in _colorButtons)
        {
            button.SetSelected(false);
        }
        foreach (var button in _shapeButtons)
        {
            button.SetSelected(false);
        }
    }

    // TODO: 임시 - 색/모양 선택 로그 (제작 로직 연결 예정)
    private void OnColorButtonClick(CottonCandyMakeButton clicked)
    {
        foreach (var button in _colorButtons)
        {
            button.SetSelected(button == clicked);
        }
        LogManager.Log($"[CottonCandyMinigame] 색 선택: {(CottonCandyColor)clicked.Value}");
    }

    private void OnShapeButtonClick(CottonCandyMakeButton clicked)
    {
        foreach (var button in _shapeButtons)
        {
            button.SetSelected(button == clicked);
        }
        LogManager.Log($"[CottonCandyMinigame] 모양 선택: {(CottonCandyShape)clicked.Value}");
    }
}
