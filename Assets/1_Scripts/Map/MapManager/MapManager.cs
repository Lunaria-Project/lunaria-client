using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    [SerializeField] private Transform _mapParent;

    private MapConfig _config;
    private BaseMap _currentMap;
    private PlayerObject _playerObject;
    private readonly List<NpcObject> _npcObjects = new();
    private readonly HashSet<int> _npcDataIdHashSet = new();
    private bool _followPlayer;

    protected override void Update()
    {
        base.Update();

        GlobalManager.Instance.UpdateCameraPosition(_playerObject.transform.position);

        if (_playerObject == null) return;
        if (_config == null) return;

        foreach (var npcObject in _npcObjects)
        {
            var distance = _playerObject.Collider.Distance(npcObject.Collider).distance;
            npcObject.SetIsNearBy(distance <= _config.NpcDistance, distance);
        }
    }

    public void SetMap(MapType type)
    {
        _config = ResourceManager.Instance.LoadMapConfig();
        LoadMap(type);
        TryLoadPlayer();
        TryLoadNpc(type);
        // 오브젝트가 스스로 위치 정하게도 해야함
        SetPanel(type);
        SetCamera(true);
    }

    public void TryInteractNearestNpc()
    {
        if (!GlobalManager.Instance.CanPlayerMove()) return;

        NpcCompassUI nearest = null;
        var minDistance = float.MaxValue;
        foreach (var npcObject in _npcObjects)
        {
            if (_config.NpcDistance < npcObject.DistanceToPlayer) continue;
            if (npcObject.DistanceToPlayer < minDistance)
            {
                minDistance = npcObject.DistanceToPlayer;
                nearest = npcObject.CompassUI;
            }
        }
        if (nearest == null) return;
        nearest.OnCompassUIClick();
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
        _playerObject.Init(_currentMap.PlayerInitPosition.position);
    }

    private void TryLoadNpc(MapType type)
    {
        if (_currentMap == null) return;

        var npcObjectIndex = 0;
        _npcDataIdHashSet.Clear();
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
                npcObject.Show(data);
                _npcObjects.Add(npcObject);
            }
            else
            {
                var npcObject = _npcObjects[npcObjectIndex];
                npcObject.Show(data);
            }
            npcObjectIndex++;
        }

        if (npcObjectIndex < _npcObjects.Count)
        {
            for (var i = npcObjectIndex; i < _npcObjects.Count; i++)
            {
                _npcObjects[i].Hide();
            }
        }
    }

    private void SetCamera(bool followPlayer)
    {
        _followPlayer = followPlayer;
        GlobalManager.Instance.UpdateCameraPosition(followPlayer ? _playerObject.transform.position : Vector3.zero);
    }

    private void SetPanel(MapType type)
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