using UnityEngine;

[CreateAssetMenu(fileName = "map_config", menuName = "Config/MapConfig")]
public class MapConfig : ScriptableObject
{
    [SerializeField] private float _collisionMargin;
    [SerializeField] private float _playerSpeed = 500;
    [SerializeField] private float _frameDuration;
    [SerializeField] private int _collisionResolveCount = 3;
    [SerializeField] private float _slidePush = 0.001f;
    [SerializeField] private float _npcMenuDistance = 2f;

    public float CollisionMargin => _collisionMargin;
    public float MapCharacterSpeed => _playerSpeed;
    public float FrameDuration => _frameDuration;
    public int CollisionResolveCount => _collisionResolveCount;
    public float SlidePush => _slidePush;
    public float NpcMenuDistance => _npcMenuDistance;
}