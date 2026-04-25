using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    [SerializeField] private Transform _mapParent;

    public NormalMap CurrentMap { get; private set; }
    public PlayerObject PlayerObject { get; private set; }
    public PathGrid PathGrid { get; private set; }
    private MapConfig _config;
    private readonly List<NpcObject> _npcObjects = new();
    private readonly HashSet<int> _npcDataIdHashSet = new();
    private bool _followPlayer;

    protected override void Update()
    {
        base.Update();

        if (_followPlayer)
        {
            GlobalManager.Instance.UpdateCameraPosition(PlayerObject.transform.position);
        }

        if (PlayerObject == null) return;
        if (_config == null) return;

        foreach (var npcObject in _npcObjects)
        {
            var distance = PlayerObject.Collider.Distance(npcObject.Collider).distance;
            var isNearBy = distance <= _config.NpcDistance;
            npcObject.SetIsNearBy(isNearBy, distance);
        }

        foreach (var npcObject in CurrentMap.StaticNpcObjects)
        {
            var distance = PlayerObject.Collider.Distance(npcObject.Collider).distance;
            npcObject.SetIsNearBy(distance);
        }
    }

    public void SetMap(MapType type)
    {
        _config = ResourceManager.Instance.LoadMapConfig();
        LoadMap(type);
        BuildPathGrid();
        TryLoadPlayer();
        TryLoadNpc(type);
        SetPanel(type);
        SetCamera(type);
    }

    public void MovePlayerAuto(int npcDataId, Action onArrived)
    {
        var npcObject = FindNpcObject(npcDataId);
        if (npcObject == null) return;

        PlayerObject.StartAutoMove(npcObject.transform.position, onArrived);
    }

    public void MovePlayerAuto(ShopType shopType, Action onArrived)
    {
        if (CurrentMap is not ShoppingSquareMap shoppingSquareMap) return;

        var position = shoppingSquareMap.GetPlayerPosition(shopType);
        PlayerObject.StartAutoMove(position, onArrived);
    }

    public bool TryInteractNearestNpc()
    {
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
        if (nearest == null) return false;
        nearest.OnCompassUIClick();
        return true;
    }

    public bool TryInteractNearestStaticNpc()
    {
        StaticNpcObject nearest = null;
        var minDistance = float.MaxValue;
        foreach (var npcObject in CurrentMap.StaticNpcObjects)
        {
            if (_config.NpcDistance < npcObject.DistanceToPlayer) continue;
            if (npcObject.DistanceToPlayer < minDistance)
            {
                minDistance = npcObject.DistanceToPlayer;
                nearest = npcObject;
            }
        }
        if (nearest == null) return false;
        nearest.OnNpcTouch();
        return true;
    }

    public bool TryInteractNearestShop()
    {
        if (CurrentMap is ShoppingSquareMap shoppingSquareMap)
        {
            foreach (var shopObject in shoppingSquareMap.ShopObjects)
            {
                if (!shopObject.IsNearBy) continue;
                shopObject.OnShopButtonClick();
                return true;
            }
        }
        return false;
    }

    private NpcObject FindNpcObject(int npcDataId)
    {
        foreach (var npcObject in _npcObjects)
        {
            if (npcObject.NpcDataId == npcDataId) return npcObject;
        }
        return null;
    }

    private void BuildPathGrid()
    {
        PathGrid = null;
        if (CurrentMap == null || !CurrentMap.HasBounds) return;
        PathGrid = new PathGrid(
            CurrentMap.MapBounds,
            _config.PathCellSize,
            _config.PathCheckRadius,
            _config.PathObstacleLayer
        );
    }

    private void LoadMap(MapType type)
    {
        if (CurrentMap != null)
        {
            CurrentMap.Hide();
            Destroy(CurrentMap.gameObject);
            CurrentMap = null;
        }

        var prefab = ResourceManager.Instance.LoadMap(type);
        if (prefab == null)
        {
            LogManager.LogError($"Map prefab not found: {type}");
            return;
        }
        CurrentMap = Instantiate(prefab, _mapParent);
        CurrentMap.Init();
    }

    private void TryLoadPlayer()
    {
        if (CurrentMap == null) return;
        if (PlayerObject == null)
        {
            var playerPrefab = ResourceManager.Instance.LoadPlayerObject();
            PlayerObject = Instantiate(playerPrefab, _mapParent);
        }
        PlayerObject.Init(CurrentMap.PlayerInitPosition.position);
    }

    private void TryLoadNpc(MapType type)
    {
        if (CurrentMap == null) return;

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

            if (GameData.Instance.ContainsMapNpcInfoData(data.NpcId))
            {
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
        }

        if (npcObjectIndex < _npcObjects.Count)
        {
            for (var i = npcObjectIndex; i < _npcObjects.Count; i++)
            {
                _npcObjects[i].Hide();
            }
        }
    }

    private void SetCamera(MapType type)
    {
        _followPlayer = type switch
        {
            MapType.ShoppingSquare => true,
            _                      => false,
        };
        GlobalManager.Instance.UpdateCameraPosition(_followPlayer ? PlayerObject.transform.position : Vector3.zero);
    }

    private void SetPanel(MapType type)
    {
        switch (type)
        {
            case MapType.Myhome:
            case MapType.PowderShop:
            case MapType.CottonCandyShop:
            {
                PanelManager.Instance.ShowPanel(PanelManager.Type.LunariaDefault, type);
                break;
            }
            case MapType.ShoppingSquare:
            {
                PanelManager.Instance.ShowPanel(PanelManager.Type.ShoppingSquareMain);
                break;
            }
        }
    }
}