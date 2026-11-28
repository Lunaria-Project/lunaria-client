using System;
using Lunaria;
using UnityEngine;

public class ArtifactCraftHandle : MonoBehaviour
{
    [SerializeField] private Text[] _coinTexts;
    [SerializeField] private LayoutSwitcher _layoutSwitcher;
    [SerializeField] private UnityEngine.UI.Slider _slider;

    private const string NoMoneyLayoutKey = "NoMoney";
    private const string NotReadyLayoutKey = "NotReady";
    private const string ReadyLayoutKey = "Ready";

    private Action _onPulledAction;

    public void SetPulledAction(Action onPulledAction)
    {
        _onPulledAction = onPulledAction;
    }

    protected void Awake()
    {
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void SetData(long requiredCoinQuantity, bool isAllMaterialFilled)
    {
        _coinTexts.SetTexts(requiredCoinQuantity.ToPrice());

        var hasEnoughCoin = UserData.Instance.GetMainCoinCount() >= requiredCoinQuantity;
        var isReady = hasEnoughCoin && isAllMaterialFilled;
        _slider.interactable = isReady;

        if (!hasEnoughCoin)
        {
            _layoutSwitcher.SetLayout(NoMoneyLayoutKey);
        }
        else if (!isAllMaterialFilled)
        {
            _layoutSwitcher.SetLayout(NotReadyLayoutKey);
        }
        else
        {
            _layoutSwitcher.SetLayout(ReadyLayoutKey);
        }
    }

    public void ResetSlider()
    {
        _slider.SetValueWithoutNotify(_slider.minValue);
    }

    private void OnSliderValueChanged(float value)
    {
        if (value < _slider.maxValue) return;
        _onPulledAction?.Invoke();
    }

    public void OnNoMoneyButtonClick()
    {
        GlobalManager.Instance.ShowToastMessage("보유 솔라가 부족해서 제작할 수 없다는 로컬키 필요"); // TODO(지선)
    }

    public void OnNotReadyButtonClick()
    {
        GlobalManager.Instance.ShowToastMessage("재료를 먼저 채워달라는 로컬키 필요"); // TODO(지선)
    }
}
