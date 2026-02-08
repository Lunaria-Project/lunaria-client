using Sirenix.OdinInspector;
using UnityEngine;

public class NpcObject : MovableObject
{
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetNpcDataIds()")]
#endif
    [SerializeField] private int _npcDataId;
    [SerializeField] private Transform _compassUITransform;

    public int NpcDataId => _npcDataId;
    public Transform CompassUITransform => _compassUITransform;
    public bool IsNearByPlayer { get; private set; }
    
    protected override int GetCharacterDataId()
    {
        var npcData = GameData.Instance.GetMapNpcInfoData(_npcDataId);
        return npcData.CharacterId;
    }

    public void SetIsNearByPlayer(bool isNearBy)
    {
        IsNearByPlayer = isNearBy;
    }
}