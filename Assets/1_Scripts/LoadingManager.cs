using Lunaria;
using UnityEngine;
using Random = UnityEngine.Random;

public class LoadingManager : SingletonMonoBehaviour<LoadingManager>
{
    [SerializeField] private Image _loadingImage;
    [SerializeField] private Text _loadingDescription;

    public const int DefaultLoadingAwaitMillis = 2000;

    protected override void Start()
    {
        _loadingImage.SetActive(false);
    }

    public void ShowLoading(LoadingType loadingType)
    {
        var loadingDataList = GameData.Instance.GetLoadingDataByLoadingType(loadingType);
        if (loadingDataList.IsNullOrEmpty())
        {
            LogManager.LogErrorPack("LoadingManager: 해당 타입의 로딩데이터가 없습니다.", loadingType);
            return;
        }
        
        var index = Random.Range(0, loadingDataList.Count);
        var randomLoadingData = loadingDataList.GetAt(index);
        _loadingImage.SetActive(true);
        _loadingImage.SetSprite(ResourceManager.Instance.LoadSprite(randomLoadingData.ResourceKey));
        _loadingDescription.SetText(randomLoadingData.Description);
    }

    public void HideLoading()
    {
        _loadingImage.SetActive(false);
    }
}