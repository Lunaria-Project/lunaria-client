using UnityEngine;

public class NpcObject : MovableObject
{
    [SerializeField] private NpcCompassUI _compassUI;

    public NpcCompassUI CompassUI => _compassUI;
    public float DistanceToPlayer { get; private set; }
    private int _characterDataId;
    private bool _isShown;

    public void Show(Generated.MapNpcPositionData positionData)
    {
        gameObject.SetActive(true);
        _characterDataId = GameData.Instance.GetMapNpcInfoData(positionData.NpcId).CharacterId;
        var infoData = GameData.Instance.GetMapNpcInfoData(positionData.NpcId);
        var initPosition = new Vector2(positionData.Positions.GetAt(0), positionData.Positions.GetAt(1));
        var initSpritePosition = new Vector2(infoData.SpritePositionAndScale.GetAt(0), infoData.SpritePositionAndScale.GetAt(1));
        var initSpriteScale = infoData.SpritePositionAndScale.GetAt(2);
        InitPositionAndScale(initPosition, initSpritePosition, initSpriteScale, infoData.ColliderRadius);
        _compassUI.Init(new Vector2(infoData.CompassUIPosition.GetAt(0), infoData.CompassUIPosition.GetAt(1)), positionData.NpcId);
        _isShown = true;
    }

    public void SetIsNearBy(bool isNearBy, float distance)
    {
        if (!_isShown) return;
        _compassUI.SetIsNearBy(isNearBy);
        DistanceToPlayer = distance;
    }
    
    public void Hide()
    {
        _isShown = false;
        gameObject.SetActive(false);
        _compassUI.OnHide();
    }

    protected override int GetCharacterDataId()
    {
        return _characterDataId;
    }
}