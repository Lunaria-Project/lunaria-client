using Lunaria;
using Sirenix.OdinInspector;
using UnityEngine;

public class WalletUI : MonoBehaviour
{
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetItemDataIds()")]
#endif
    [SerializeField] int _itemDataId;
    [SerializeField] Text[] _walletTexts;
    [SerializeField] LayoutSwitcher _layoutSwitcher;

    private const string SingleLayoutKey = "Single";
    private const string MultipleLayoutKey = "Multiple";

    protected void OnEnable()
    {
        UserData.Instance.OnItemQuantityChanged -= OnItemQuantityChanged;
        UserData.Instance.OnItemQuantityChanged += OnItemQuantityChanged;
        _layoutSwitcher.SetLayout(SingleLayoutKey); // TODO(지선): 임시
    }

    protected void OnDisable()
    {
        UserData.Instance.OnItemQuantityChanged -= OnItemQuantityChanged;
    }

    public void Refresh()
    {
        _walletTexts.SetTexts(UserData.Instance.GetMainCoinCount().ToPrice());
    }

    private void OnItemQuantityChanged(int itemId)
    {
        if (itemId != _itemDataId) return;
        Refresh();
    }
}