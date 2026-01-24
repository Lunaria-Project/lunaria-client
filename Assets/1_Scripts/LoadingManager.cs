using System.Linq;
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

    public void ShowLoading()
    {
        _loadingImage.SetActive(true);
        
        var loadingDataDictionary = GameData.Instance.DTLoadingData;
        var index = Random.Range(0, loadingDataDictionary.Count);
        var randomLoadingData = loadingDataDictionary.ElementAt(index).Value;
        _loadingImage.SetSprite(ResourceManager.Instance.LoadSprite(randomLoadingData.ResourceKey));
        _loadingDescription.SetText(randomLoadingData.Description);
    }

    public void HideLoading()
    {
        _loadingImage.SetActive(false);
    }
}