using Sirenix.OdinInspector;
using UnityEngine;

public class BaseMapManager : MonoBehaviour
{
    [SerializeField] private MapObject[] _mapObjects;
    [SerializeField] private NpcObject[] _npcObjects;
    [SerializeField] private PlayerObject _player;

    private MapConfig _config;

    protected virtual void Start()
    {
        GlobalManager.Instance.OnChangeMap(_npcObjects);
        SetMapObjectSortingLayer();
        _config = ResourceManager.Instance.LoadMapConfig();
    }

    protected virtual void Update()
    {
        if (GameTimeManager.Instance.IsPaused) return;

        _player.SetSortingLayer();
        UpdateNpcDistance();
    }

    protected Vector3 GetPlayerPosition()
    {
        return _player.transform.position;
    }

    private void UpdateNpcDistance()
    {
        foreach (var npc in _npcObjects)
        {
            var distance = Vector2.Distance(_player.transform.position, npc.transform.position);
            npc.SetIsNearByPlayer(distance <= _config.NpcMenuDistance);
        }
    }

    [Button]
    private void SetMapObjectSortingLayer()
    {
        foreach (var mapObject in _mapObjects)
        {
            mapObject.SetSortingLayer();
        }
    }
}