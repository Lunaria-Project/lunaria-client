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

    public void Refresh()
    {
        _walletText.SetText(UserData.Instance.GetItemQuantity(_itemDataId).ToPrice());
    }
}
