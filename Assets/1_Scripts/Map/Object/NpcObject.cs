
using Sirenix.OdinInspector;
using UnityEngine;

public class NpcObject : MovableObject
{
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetNpcDataIds()")]
#endif
    [SerializeField] private int _npcDataId;

    public int NpcDataId => _npcDataId;
    
    protected override int GetCharacterDataId()
    {
        var npcData = GameData.Instance.GetMapNpcInfoData(_npcDataId);
        return npcData.CharacterId;
    }
}