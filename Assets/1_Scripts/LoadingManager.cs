using System.Linq;
using Generated;
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
        _loadingImage.SetActive(true);

        var loadingDataList = GameData.Instance.GetLoadingDataByLoadingType(loadingType);
        var index = Random.Range(0, loadingDataList.Count);
        var randomLoadingData = loadingDataList.GetAt(index);
        _loadingImage.SetSprite(ResourceManager.Instance.LoadSprite(randomLoadingData.ResourceKey));
        _loadingDescription.SetText(randomLoadingData.Description);
    }

    public void HideLoading()
    {
        _loadingImage.SetActive(false);
    }
}