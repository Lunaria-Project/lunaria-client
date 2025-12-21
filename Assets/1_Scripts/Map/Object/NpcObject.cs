using Sirenix.OdinInspector;
using UnityEngine;

public class NpcObject : MovableObject
{
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetNpcDataIds()")]
#endif
    [SerializeField] private int _npcDataId;

    public int NpcDataId => _npcDataId;
}