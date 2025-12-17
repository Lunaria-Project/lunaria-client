using Generated;
using Sirenix.OdinInspector;
using UnityEngine;

public class MyhomeMapManager : MonoBehaviour
{
    [SerializeField] private MapObject[] _mapObjects;
    [SerializeField] private NpcObject[] _npcObjects;
    [SerializeField] private PlayerObject _player;
    [SerializeField] private int _sortingOrderOffset = 10;
    [SerializeField] private MapConfig _config;

    private int _currentNpcDataId;

    private void Start()
    {
        _currentNpcDataId = 0;
        SetMapObjectSortingLayer();
        PanelManager.Instance.ShowPanel(PanelManager.Type.MyhomeMain);
    }

    private void Update()
    {
        _player.SetSortingLayer(_sortingOrderOffset);
        UpdateNpcMenu();
        if (_currentNpcDataId > 0)
        {
            //TODO(지선): FunctionType에 따른 액션 구현하기
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var npcDataList = GameData.Instance.GetActivatedMapNpcMenuDataListByNpcId(_currentNpcDataId);
                var npcData = npcDataList.GetAt(0);
                if (npcData == null) return;
                if (npcData.FunctionType == NpcMenuFunctionType.PlayCutscene)
                {
                    CutsceneManager.Instance.PlayCutscene(npcData.FunctionValue);
                    return;
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                var npcDataList = GameData.Instance.GetActivatedMapNpcMenuDataListByNpcId(_currentNpcDataId);
                var npcData = npcDataList.GetAt(1);
                if (npcData == null) return;
                if (npcData.FunctionType == NpcMenuFunctionType.PlayCutscene)
                {
                    CutsceneManager.Instance.PlayCutscene(npcData.FunctionValue);
                    return;
                }
            }
        }
    }

    private void UpdateNpcMenu()
    {
        if (_player == null) return;
        if (_npcObjects == null || _npcObjects.Length == 0) return;

        var nearestNpc = GetNearestNpc(out var distance);

        if (_currentNpcDataId <= 0)
        {
            if (nearestNpc != null && distance <= _config.NpcMenuDistance && _currentNpcDataId != nearestNpc.NpcDataId)
            {
                ShowNpcMenu(nearestNpc.NpcDataId);
            }
        }
        else
        {
            if (nearestNpc == null || distance > _config.NpcMenuDistance)
            {
                HideNpcMenu();
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

    private void ShowNpcMenu(int npcDataId)
    {
        _currentNpcDataId = npcDataId;
        LogManager.Log("메뉴 보여주기");
        // 메뉴 보여주기
    }

    private void HideNpcMenu()
    {
        _currentNpcDataId = 0;
        LogManager.Log("메뉴 없애기");
        // 메뉴 없애기
    }

    [Button]
    private void SetMapObjectSortingLayer()
    {
        foreach (var mapObject in _mapObjects)
        {
            mapObject.SetSortingLayer(_sortingOrderOffset);
        }
    }
}