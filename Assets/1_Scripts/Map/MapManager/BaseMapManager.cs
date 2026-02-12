using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BaseMapManager : MonoBehaviour
{
    [SerializeField] private MapObject[] _mapObjects;
    [SerializeField] private PlayerObject _player;

    private MapConfig _config;
    private List<NpcInfo> _npcInfoList = new();

    protected virtual void Start()
    {
        _npcInfoList.Clear();
        foreach (var mapObject in _mapObjects)
        {
            if (mapObject is MovableNpcObject movableNpcObject)
            {
                _npcInfoList.Add(movableNpcObject.NpcInfo);
            }

        }
        GlobalManager.Instance.OnChangeMap(_npcInfoList);
        SetMapObjectSortingLayer();
        _config = ResourceManager.Instance.LoadMapConfig();
    }

    protected virtual void Update()
    {
        if (GameTimeManager.Instance.IsPaused) return;

        _player.SetSortingLayer();
        UpdateNpcDistance();
        GlobalManager.Instance.UpdateCameraPosition(GetPlayerPosition());
    }

    protected Vector3 GetPlayerPosition()
    {
        return _player.transform.position;
    }

    private void UpdateNpcDistance()
    {
        foreach (var npc in _npcInfoList)
        {
            var distance = Vector2.Distance(_player.transform.position, npc.Transform.position);
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