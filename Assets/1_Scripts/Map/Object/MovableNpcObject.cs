using UnityEngine;

public class MovableNpcObject : MovableObject
{
    [SerializeField] private Transform _compassUITransform;

    public NpcInfo NpcInfo { get; private set; } = new();
    private int _npcDataId;

    public void Init(Generated.MapNpcPositionData positionData)
    {
        _npcDataId = positionData.NpcId;
        InitPosition(new Vector2(positionData.Positions.GetAt(0), positionData.Positions.GetAt(1)));
        var infoData = GameData.Instance.GetMapNpcInfoData(_npcDataId);
        InitSpritePosition(new Vector2(infoData.SpritePositionAndScale.GetAt(0), infoData.SpritePositionAndScale.GetAt(1)), infoData.SpritePositionAndScale.GetAt(2));
        Transform.localScale = new Vector3(infoData.ColliderScale, infoData.ColliderScale, 1);
        NpcInfo.Init(_npcDataId, _compassUITransform, transform, Collider);
    }

    protected override int GetCharacterDataId()
    {
        var npcData = GameData.Instance.GetMapNpcInfoData(_npcDataId);
        return npcData.CharacterId;
    }
}