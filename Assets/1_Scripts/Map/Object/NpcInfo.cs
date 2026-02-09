using UnityEngine;

public class NpcInfo
{
    public int NpcDataId { get; private set; }
    public bool IsNearByPlayer { get; private set; }
    public Transform CompassUITransform { get; private set; }
    public Transform Transform { get; private set; }

    public void Init(int npcDataId, Transform compassUITransform, Transform transform)
    {
        NpcDataId = npcDataId;
        CompassUITransform = compassUITransform;
        Transform = transform;
    }

    public void SetIsNearByPlayer(bool isNearByPlayer)
    {
        IsNearByPlayer = isNearByPlayer;
    }
}