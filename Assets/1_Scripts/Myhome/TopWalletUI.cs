using Lunaria;
using Sirenix.OdinInspector;
using UnityEngine;

public class TopWalletUI : MonoBehaviour
{
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetItemDataIds()")]
#endif
    [SerializeField] int _itemDataId;
    [SerializeField] Text _walletText;

    protected void OnEnable()
    {
        UserData.Instance.OnItemQuantityChanged -= OnItemQuantityChanged;
        UserData.Instance.OnItemQuantityChanged += OnItemQuantityChanged;
    }

    protected void OnDisable()
    {
        UserData.Instance.OnItemQuantityChanged -= OnItemQuantityChanged;
    }

    public void Refresh()
    {
        _walletText.SetText(UserData.Instance.GetItemQuantity(_itemDataId).ToPrice());
    }

    private void OnItemQuantityChanged(int itemId)
    {
        if (itemId != _itemDataId) return;
        Refresh();
    }
}
