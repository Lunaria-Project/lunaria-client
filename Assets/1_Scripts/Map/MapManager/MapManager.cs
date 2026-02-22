using Generated;
using UnityEngine;

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    [SerializeField] private Transform _mapParent;

    private BaseMap _currentMap;
    private PlayerObject _playerObject;

    public void SetMap(MapType type)
    {
        LoadMap(type);
        TryLoadPlayer();
        // npc도 동적로드하게 해야한다.
        // 오브젝트가 스스로 위치 정하게도 해야함
        ShowPanel(type);
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