using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class BaseMapManager : MonoBehaviour
{
    [SerializeField] private MapObject[] _mapObjects;
    [SerializeField] private NpcObject[] _npcObjects;
    [SerializeField] private PlayerObject _player;

    private int _currentNpcDataId;
    private MapConfig _config;

    protected virtual void Start()
    {
        _currentNpcDataId = 0;
        SetMapObjectSortingLayer();
        _config = ResourceManager.Instance.LoadMapConfig();
    }

    protected virtual void Update()
    {
        if (GameTimeManager.Instance.IsPaused) return;

        _player.SetSortingLayer();
        UpdateNpcMenu();
    }

    protected Vector3 GetPlayerPosition()
    {
        return _player.transform.position;
    }

    private void UpdateNpcMenu()
    {
        if (!CanShowNpcSelectionPopup()) return;

        var nearestNpc = GetNearestNpc(out var distance);

        if (_currentNpcDataId <= 0)
        {
            if (nearestNpc != null && distance <= _config.NpcMenuDistance && _currentNpcDataId != nearestNpc.NpcDataId)
            {
                _currentNpcDataId = nearestNpc.NpcDataId;
                PopupManager.Instance.ShowPopup(PopupManager.Type.NpcSelection, new NpcSelectionPopupParameter { NpcDataId = _currentNpcDataId }).SetOnHideAction(() => _currentNpcDataId = 0);
            }
        }
        else
        {
            if (nearestNpc == null || distance > _config.NpcMenuDistance)
            {
                _currentNpcDataId = 0;
                var currentPopup = PopupManager.Instance.GetCurrentPopup();
                if (currentPopup == null || currentPopup is not NpcSelectionPopup) return;
                PopupManager.Instance.HideCurrentPopup(PopupManager.Type.NpcSelection).Forget();
            }
        }
    }

    private NpcObject GetNearestNpc(out float distance)
    {
        NpcObject nearest = null;
        var minDist = float.MaxValue;

        foreach (var npc in _npcObjects)
        {
            if (npc == null) continue;

            var d = Vector2.Distance(_player.transform.position, npc.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = npc;
            }
        }

        distance = minDist;
        return nearest;
    }

    protected virtual bool CanShowNpcSelectionPopup()
    {
        if (PopupManager.Instance.ContainsPopup(PopupManager.Type.NpcSelection)) return false;
        if (_player == null) return false;
        if (_npcObjects == null || _npcObjects.Length == 0) return false;
        if (CutsceneManager.Instance.IsPlaying) return false;
        return true;
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