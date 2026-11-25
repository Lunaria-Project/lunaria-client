using Generated;
using Lunaria;
using UnityEngine;

public class LoadingObject : MonoBehaviour
{
    [SerializeField] private Image _loadingImage;
    [SerializeField] private Text _loadingDescription;

    public void SetData(LoadingData data)
    {
        _loadingImage.SetSprite(ResourceManager.Instance.LoadSprite(data.ResourceKey));
        _loadingDescription.SetText(data.Description);
    }
}