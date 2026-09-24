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

    [Serializable]
    public struct FamiliarCallDataId
    {
#if UNITY_EDITOR
        [ValueDropdown("@DataIdDropDownList.GetFamiliarCallDataIds()")]
#endif
        public int DataId;
    }

    [SerializeField] private SerializedUserData _userInventory = new();
    [SerializeField] private FamiliarCallDataId[] _summonedFamiliars = Array.Empty<FamiliarCallDataId>();
    [SerializeField] private float _initSlimeGauge;
    [SerializeField] private int _timeSpeedMultiplier;

    public SerializedUserData UserInventory => _userInventory;
    public FamiliarCallDataId[] SummonedFamiliars => _summonedFamiliars;
    public float InitSlimeGauge => _initSlimeGauge;
    public int TimeSpeedMultiplier => _timeSpeedMultiplier;
}