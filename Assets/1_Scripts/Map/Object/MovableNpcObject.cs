using Sirenix.OdinInspector;
using UnityEngine;

public class MovableNpcObject : MovableObject
{
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetNpcDataIds()")]
#endif
    [SerializeField] private int _npcDataId;
    [SerializeField] private Transform _compassUITransform;

    public NpcInfo NpcInfo { get; private set; } = new();

    #region UnityEvent

    protected override void Start()
    {
        NpcInfo.Init(_npcDataId, _compassUITransform, transform);
        base.Start();
    }

    #endregion

    protected override int GetCharacterDataId()
    {
        var npcData = GameData.Instance.GetMapNpcInfoData(_npcDataId);
        return npcData.CharacterId;
    }
}