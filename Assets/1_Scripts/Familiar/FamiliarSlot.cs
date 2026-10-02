using Lunaria;
using UnityEngine;

public enum FamiliarSlotState
{
    Locked,
    Empty,
    Idle,
    Summoned,
    NoEnergy,
    Working,
    WorkDone,
}

public class FamiliarSlot : MonoBehaviour
{
    [SerializeField] private LayoutSwitcher _layoutSwitcher;
    [SerializeField] private Image _familiarImage;
    [SerializeField] private UnityEngine.UI.Slider _hpSlider;
    [SerializeField] private GameObject _hpGreenObject;
    [SerializeField] private GameObject _hpRedObject;
    [SerializeField] private Text _noEnergyRemainTimeText;
    [SerializeField] private Text _workRemainTimeText;

    private const string LockedLayoutKey = "Locked";
    private const string IdleLayoutKey = "Idle";
    private const string SummonedLayoutKey = "Summoned";
    private const string NoEnergyLayoutKey = "NoEnergy";
    private const string WorkingLayoutKey = "Working";
    private const string EmptyLayoutKey = "Empty";
    private const string WorkDoneLayoutKey = "WorkDone";

    private const int FamiliarImageFrameNumber = 1;
    private const float LowHpRatio = 0.2f;

    private FamiliarSlotState _state;
    private FamiliarInfo _familiar;

    public void SetData(FamiliarSlotState state, FamiliarInfo familiar)
    {
        _state = state;
        _familiar = familiar;
        _layoutSwitcher.SetLayout(GetLayoutKey(state));

        if (familiar == null) return;
        var familiarCallData = GameData.Instance.GetFamiliarCallData(familiar.FamiliarCallItemId);
        _familiarImage.SetSprite(ResourceManager.Instance.LoadFamiliarSprite(familiarCallData.ResourceKey, true, FamiliarImageFrameNumber));
        RefreshHp(familiar.CurrentHp, familiarCallData.MaxHp);

        switch (state)
        {
            case FamiliarSlotState.NoEnergy:
            {
                var remainSeconds = UserData.Instance.GetNoEnergyRemainSeconds(familiar);
                _noEnergyRemainTimeText.SetText(TimeUtil.SecondsToHourMinuteString(TimeUtil.CeilToTenMinuteInterval(remainSeconds)));
                break;
            }
            case FamiliarSlotState.Working:
            {
                var remainSeconds = UserData.Instance.GetWorkRemainSeconds(familiar);
                _workRemainTimeText.SetText(TimeUtil.SecondsToHourMinuteString(TimeUtil.CeilToTenMinuteInterval(remainSeconds)));
                break;
            }
        }
    }

    private void RefreshHp(int currentHp, int maxHp)
    {
        var hpRatio = (float)currentHp / maxHp;
        _hpSlider.value = hpRatio;

        var isLowHp = hpRatio <= LowHpRatio;
        _hpRedObject.SetActive(isLowHp);
        _hpGreenObject.SetActive(!isLowHp);
    }

    private void ShowSummonPopup()
    {
        var familiar = _familiar;
        var familiarCallData = GameData.Instance.GetFamiliarCallData(familiar.FamiliarCallItemId);
        var parameter = new SystemTwoButtonParameter
        {
            Description = LocalizationKey.Familiar_SetSummonedState.Text(familiarCallData.Name),
            ConfirmButtonText = LocalizationKey.ConfirmButton,
            CancelButtonText = LocalizationKey.CancelButton,
            OnConfirm = () => { UserData.Instance.SummonFamiliar(familiar); },
        };
        PopupManager.Instance.ShowPopup(PopupManager.Type.SystemButton, parameter);
    }

    private void ShowUnsummonPopup()
    {
        var familiar = _familiar;
        var familiarCallData = GameData.Instance.GetFamiliarCallData(familiar.FamiliarCallItemId);
        var parameter = new SystemTwoButtonParameter
        {
            Description = LocalizationKey.Familiar_SetIdleState.Text(familiarCallData.Name),
            ConfirmButtonText = LocalizationKey.ConfirmButton,
            CancelButtonText = LocalizationKey.CancelButton,
            OnConfirm = () => { UserData.Instance.UnsummonFamiliar(familiar); },
        };
        PopupManager.Instance.ShowPopup(PopupManager.Type.SystemButton, parameter);
    }

    private void ShowFamiliarCallItemSelection()
    {
        var familiarCallItems = UserData.Instance.GetItemQuantities(ItemType.FamiliarCall);
        familiarCallItems.RemoveAll(item => !UserData.Instance.CanAddFamiliar(item.ItemId, false));
        if (familiarCallItems.Count == 0)
        {
            GlobalManager.Instance.ShowToastMessage(LocalizationKey.Familiar_EmptySlotNoFamiliarMessage.Text());
            return;
        }
        if (familiarCallItems.Count == 1)
        {
            ItemUseManager.Instance.Use(familiarCallItems[0].ItemId);
            return;
        }

        PopupManager.Instance.ShowPopupWithEmptyParameter(PopupManager.Type.Inventory);
        GlobalManager.Instance.ShowToastMessage(LocalizationKey.Familiar_EmptySlotMessage.Text());
    }

    public void OnButtonClick()
    {
        switch (_state)
        {
            case FamiliarSlotState.Empty:
            {
                ShowFamiliarCallItemSelection();
                break;
            }
            case FamiliarSlotState.Idle:
            {
                ShowSummonPopup();
                break;
            }
            case FamiliarSlotState.Summoned:
            {
                ShowUnsummonPopup();
                break;
            }
            case FamiliarSlotState.WorkDone:
            {
                // TODO(지선): 근무 완료 보상 팝업 띄우기
                break;
            }
            case FamiliarSlotState.Locked:
            {
                // TODO(지선): 슬롯 해제 아이템 타입 작업할 때 작업 필요
                GlobalManager.Instance.ShowToastMessage(LocalizationKey.Familiar_LockedSlotMessage.Text());
                break;
            }
            case FamiliarSlotState.NoEnergy: // DO NOTHING
            {
                break;
            }
        }
    }

    private static string GetLayoutKey(FamiliarSlotState state)
    {
        return state switch
        {
            FamiliarSlotState.Locked   => LockedLayoutKey,
            FamiliarSlotState.Empty    => EmptyLayoutKey,
            FamiliarSlotState.Idle     => IdleLayoutKey,
            FamiliarSlotState.Summoned => SummonedLayoutKey,
            FamiliarSlotState.NoEnergy => NoEnergyLayoutKey,
            FamiliarSlotState.Working  => WorkingLayoutKey,
            FamiliarSlotState.WorkDone => WorkDoneLayoutKey,
            _                          => EmptyLayoutKey,
        };
    }
}