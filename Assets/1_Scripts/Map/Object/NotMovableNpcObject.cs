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


    protected override void Start()
    {
        base.Start();
    }
}