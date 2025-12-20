using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Lunaria/user_data_cheat_asset")]
public class UserDataCheatAsset : ScriptableObject
{
    [Serializable]
    public class SerializedUserData : SerializedDictionary<ItemDataId, long> { }
    
    [Serializable]
    public struct ItemDataId
    {
#if UNITY_EDITOR
        [ValueDropdown("@DataIdDropDownList.GetItemDataIds()")]
#endif
        public int DataId;
    }

    [SerializeField] private SerializedUserData _userInventory = new();

    public SerializedUserData UserInventory => _userInventory;
}