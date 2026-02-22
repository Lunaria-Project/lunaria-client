using Sirenix.OdinInspector;
using UnityEngine;

public class NotMovableNpcObject : OldMapObject
{
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetNpcDataIds()")]
#endif
    [SerializeField] private int _npcDataId;
    [SerializeField] private CircleCollider2D _collider2D;
    [SerializeField] private Transform _compassUITransform;

    public NpcInfo NpcInfo { get; private set; } = new();

    protected override void Start()
    {
        base.Start();
        NpcInfo.Init(_npcDataId, _compassUITransform, transform, _collider2D);
    }
}