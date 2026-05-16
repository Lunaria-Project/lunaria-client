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
    private bool _isNearBy;

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

    public void SetIsNearBy(bool isNearBy, float distance)
    {
        if (_menuData == null) return;
        _isNearBy = isNearBy;
        DistanceToPlayer = distance;
    }

    public void OnNpcTouch()
    {
        if (_menuData == null) return;
        if (!_isNearBy) return;
        if (_menuData.FunctionType == NpcMenuFunctionType.None) return;
        GlobalManager.Instance.InvokeNpcFunction(_menuData.FunctionType, _menuData.FunctionValue);
    }
}