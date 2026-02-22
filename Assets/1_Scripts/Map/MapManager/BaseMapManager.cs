using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BaseMapManager : MonoBehaviour
{
    [SerializeField] private MapObject[] _mapObjects;
    //[SerializeField] private PlayerObject _player;

    //protected PlayerObject Player => _player;
    private MapConfig _config;
    private readonly List<NpcInfo> _npcInfoList = new();

    protected virtual void Start()
    {
        _npcInfoList.Clear();
        foreach (var mapObject in _mapObjects)
        {
            if (mapObject is MovableNpcObject movableNpcObject)
            {
                _npcInfoList.Add(movableNpcObject.NpcInfo);
            }
            if (mapObject is NotMovableNpcObject notMovableNpcObject)
            {
                _npcInfoList.Add(notMovableNpcObject.NpcInfo);
            }
        }
        GlobalManager.Instance.OnChangeMap(_npcInfoList);
        SetMapObjectSortingLayer();
        _config = ResourceManager.Instance.LoadMapConfig();
    }

    protected virtual void Update()
    {
        if (GameTimeManager.Instance.IsPaused) return;

        //_player.SetSortingLayer();
        //UpdateNpcDistance();
        //GlobalManager.Instance.UpdateCameraPosition(GetPlayerPosition());
    }

    //private Vector3 GetPlayerPosition()
    //{
    //    return _player.transform.position;
    //}

    //private void UpdateNpcDistance()
    //{
    //    foreach (var npc in _npcInfoList)
    //    {
    //        var distance = _player.Collider.Distance(npc.Collider).distance;
    //        npc.SetDistanceToPlayer(distance, _config.NpcDistance);
    //    }
    //}

    [Button]
    private void SetMapObjectSortingLayer()
    {
        foreach (var mapObject in _mapObjects)
        {
            mapObject.SetSortingLayer();
        }
    }
}