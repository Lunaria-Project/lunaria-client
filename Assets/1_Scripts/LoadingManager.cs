using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class LoadingManager : SingletonMonoBehaviour<LoadingManager>
{
    [SerializeField] private RectTransform _loadingParent;

    public const int DefaultLoadingAwaitMillis = 2000;

    private readonly Dictionary<string, LoadingObject> _loadingObjectMap = new();
    [CanBeNull] private LoadingObject _currentLoadingObject;

    public void ShowNormalLoading()
    {
        var currentHours = TimeUtil.SecondsToHours(GameTimeManager.Instance.CurrentGameTime.TotalSeconds);
        ShowLoading(currentHours >= TimeUtil.NightStartHours ? LoadingType.NormalNight : LoadingType.NormalDay);
    }

    public void ShowLoading(LoadingType loadingType)
    {
        if (loadingType == LoadingType.None)
        {
            LogManager.LogErrorPack("LoadingManager: loadingType이 None입니다.");
            return;
        }
        var loadingDataList = GameData.Instance.GetLoadingDataByLoadingType(loadingType);
        if (loadingDataList.IsNullOrEmpty())
        {
            LogManager.LogErrorPack("LoadingManager: 해당 타입의 로딩데이터가 없습니다.", loadingType);
            return;
        }

        var index = Random.Range(0, loadingDataList.Count);
        var randomLoadingData = loadingDataList.GetAt(index);

        if (!_loadingObjectMap.TryGetValue(randomLoadingData.PrefabResourceKey, out var loadingObject))
        {
            var prefab = ResourceManager.Instance.LoadLoadingObject(randomLoadingData.PrefabResourceKey);
            if (prefab == null)
            {
                LogManager.LogErrorPack("LoadingManager: 로딩 프리팹을 찾을 수 없습니다.", randomLoadingData.PrefabResourceKey);
                return;
            }
            loadingObject = Instantiate(prefab, _loadingParent);
            _loadingObjectMap.Add(randomLoadingData.PrefabResourceKey, loadingObject);
        }

        HideLoading();
        _currentLoadingObject = loadingObject;
        loadingObject.SetActive(true);
        loadingObject.SetData(randomLoadingData);
    }

    public void HideLoading()
    {
        if (_currentLoadingObject == null) return;
        _currentLoadingObject.SetActive(false);
        _currentLoadingObject = null;
    }
}
