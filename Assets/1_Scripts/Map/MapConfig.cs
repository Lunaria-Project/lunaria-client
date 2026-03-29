using UnityEngine;

[CreateAssetMenu(fileName = "map_config", menuName = "Config/MapConfig")]
public class MapConfig : ScriptableObject
{
    [SerializeField] private float _collisionMargin;
    [SerializeField] private float _playerSpeed = 500;
    [SerializeField] private float _frameDuration;
    [SerializeField] private int _collisionResolveCount = 3;
    [SerializeField] private float _slidePush = 0.001f;
    [SerializeField] private float _npcDistance = 100f;

    [Header("[Pathfinding]")]
    [SerializeField] private float _pathCellSize = 2f;
    [SerializeField] private float _pathCheckRadius = 1f;
    [SerializeField] private LayerMask _pathObstacleLayer;

    public float CollisionMargin => _collisionMargin;
    public float MapCharacterSpeed => _playerSpeed;
    public float FrameDuration => _frameDuration;
    public int CollisionResolveCount => _collisionResolveCount;
    public float SlidePush => _slidePush;
    public float NpcDistance => _npcDistance;
    public float PathCellSize => _pathCellSize;
    public float PathCheckRadius => _pathCheckRadius;
    public LayerMask PathObstacleLayer => _pathObstacleLayer;
}