using Lunaria;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class LocalizationImage : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private string _resourceKey;
    [SerializeField] private bool _setNativeSize;

    private const string DefaultPostfix = "_ko";

    protected void Start()
    {
        ApplyLocalization();
    }

    public void SetResourceKey(string resourceKey)
    {
        _resourceKey = resourceKey;
        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        if (string.IsNullOrEmpty(_resourceKey)) return;

        var localizedKey = $"{_resourceKey}{GetLocalPostfix()}";
        var sprite = ResourceManager.Instance.LoadSprite(localizedKey);
        if (sprite == null)
        {
            LogManager.LogWarning($"[LocalizationImage] ApplyLocalization: 현지화 이미지가 없어 기본 언어로 대체합니다. key={localizedKey}");
            sprite = ResourceManager.Instance.LoadSprite(localizedKey);
        }

        if (sprite == null)
        {
            LogManager.LogError($"[LocalizationImage] ApplyLocalization: 이미지를 찾을 수 없습니다. key={localizedKey}");
            return;
        }

        _image.SetSprite(sprite);
        if (_setNativeSize)
        {
            _image.SetNativeSize();
        }
    }

    private static string GetLocalPostfix()
    {
        return Application.systemLanguage switch
        {
            SystemLanguage.Korean   => "_ko",
            SystemLanguage.English  => "_en",
            SystemLanguage.Japanese => "_ja",
            _                       => DefaultPostfix,
        };
    }
}
