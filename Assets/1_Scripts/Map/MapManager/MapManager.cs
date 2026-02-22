using System.Collections.Generic;
using Generated;
using UnityEngine;

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    [SerializeField] private Transform _mapParent;

    private BaseMap _currentMap;
    private PlayerObject _playerObject;
    private List<MovableNpcObject> _npcObjects = new();
    private readonly HashSet<int> _npcDataIdHashSet = new();

    public void SetMap(MapType type)
    {
        LoadMap(type);
        TryLoadPlayer();
        TryLoadNpc(type);
        // 오브젝트가 스스로 위치 정하게도 해야함
        ShowPanel(type);
    }

    private void LoadMap(MapType type)
    {
        if (_currentMap != null)
        {
            Destroy(_currentMap.gameObject);
            _currentMap = null;
        }

        var prefab = ResourceManager.Instance.LoadMap(type);
        if (prefab == null)
        {
            LogManager.LogError($"Map prefab not found: {type}");
            return;
        }
        _currentMap = Instantiate(prefab, _mapParent);
    }

    private void TryLoadPlayer()
    {
        if (_currentMap == null) return;
        if (_playerObject == null)
        {
            var playerPrefab = ResourceManager.Instance.LoadPlayerObject();
            _playerObject = Instantiate(playerPrefab, _mapParent);
        }
        _playerObject.transform.position = _currentMap.PlayerInitPosition.position;
    }

    private void TryLoadNpc(MapType type)
    {
        if (_currentMap == null) return;

        var npcObjectIndex = 0;
        _npcDataIdHashSet.Clear();
        _npcObjects.SetActiveAll(false);
        foreach (var data in GameData.Instance.DTMapNpcPositionData)
        {
            if (!RequirementManager.Instance.IsSatisfied(data.ShowRequirement, data.ShowRequirementValues)) continue;
            if (data.MapType != type) continue;
            if (_npcDataIdHashSet.Contains(data.NpcId))
            {
                LogManager.LogErrorPack("Duplicate NPC data", data.NpcId);
                continue;
            }
            _npcDataIdHashSet.Add(data.NpcId);
            if (_npcObjects.Count <= npcObjectIndex)
            {
                var npcPrefab = ResourceManager.Instance.LoadNpcObject();
                var npcObject = Instantiate(npcPrefab, _mapParent);
                npcObject.Init(data);
                _npcObjects.Add(npcObject);
            }
            else
            {
                var npcObject = _npcObjects[npcObjectIndex];
                npcObject.gameObject.SetActive(true);
                npcObject.Init(data);
            }
            npcObjectIndex++;
        }
    }

    private void ShowPanel(MapType type)
    {
        switch (type)
        {
            case MapType.Myhome:
            {
                PanelManager.Instance.ShowPanel(PanelManager.Type.MyhomeMain);
                break;
            }
        }
    }
}