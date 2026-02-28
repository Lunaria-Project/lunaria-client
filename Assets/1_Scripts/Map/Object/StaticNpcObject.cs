using System.Collections.Generic;
using Generated;
using Sirenix.OdinInspector;
using UnityEngine;

public class StaticNpcObject : MapObject
{
    [SerializeField] private MouseButtonTrigger _mouseButtonTrigger;
    [SerializeField] private CircleCollider2D _collider2D;
    [SerializeField] private SpriteRenderer _npcSpriteRenderer;
    [SerializeField] private Transform _spriteTransform;
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

        switch (_menuData.FunctionType)
        {
            case NpcMenuFunctionType.PlayPowderPortalMinigame:
            {
                var artifactData = GameData.Instance.GetArtifactData(UserData.Instance.EquippedArtifactId);
                if (artifactData.ArtifactType != ArtifactType.Bubblegun)
                {
                    GlobalManager.Instance.ShowToastMessage("버블건을 장착하자."); // TODO
                    return;
                }
                PanelManager.Instance.ShowPanel(PanelManager.Type.SlimeMinigame);
                break;
            }
            default:
            {
                LogManager.LogErrorPack("Undefined NPC menu function type", _menuData.FunctionType);
                break;
            }
        }
    }
}