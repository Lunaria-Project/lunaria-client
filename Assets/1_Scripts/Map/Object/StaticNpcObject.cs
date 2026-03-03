using Generated;
using Sirenix.OdinInspector;
using UnityEngine;

public class StaticNpcObject : MapObject
{
    [SerializeField] private MouseButtonTrigger _mouseButtonTrigger;
    [SerializeField] private CircleCollider2D _collider2D;
#if UNITY_EDITOR
    [ValueDropdown("@DataIdDropDownList.GetNpcDataIds()")]
#endif
    [SerializeField] private int _npcId;

    public CircleCollider2D Collider => _collider2D;
    public float DistanceToPlayer { get; private set; }
    private MapStaticNpcMenuData _menuData;

    private void Awake()
    {
        _mouseButtonTrigger.SetOnMouseDown(OnNpcTouch);
    }

    public void Init()
    {
        DistanceToPlayer = float.MaxValue;
        GameTimeManager.Instance.OnIntervalChanged -= Refresh;
        GameTimeManager.Instance.OnIntervalChanged += Refresh;
        Refresh();
    }

    public void Hide()
    {
        GameTimeManager.Instance.OnIntervalChanged -= Refresh;
    }

    private void Refresh()
    {
        _menuData = GameData.Instance.GetStaticNpcMenuData(_npcId);
    }

    public void SetIsNearBy(float distance)
    {
        if (_menuData == null) return;
        DistanceToPlayer = distance;
    }

    public void OnNpcTouch()
    {
        if (_menuData == null) return;
        if (_menuData.FunctionType == NpcMenuFunctionType.None) return;
        GlobalManager.Instance.InvokeNpcFunction(_menuData.FunctionType, _menuData.FunctionValue);
    }
}