using UnityEngine;

[RequireComponent(typeof(Lunaria.Text))]
public class LocalizationText : MonoBehaviour
{
    [SerializeField] private string _key;

    private Lunaria.Text _text;

    private void Start()
    {
        _text = GetComponent<Lunaria.Text>();
        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        if (string.IsNullOrEmpty(_key)) return;
        if (!GameData.Instance.TryGetLocalization(_key, out var localization)) return;

        _text.text = GlobalManager.Instance.LocalType switch
        {
            GameData.LocalType.Ko => localization.Ko,
            GameData.LocalType.En => localization.En,
            GameData.LocalType.Ja => localization.Ja,
            _ => localization.Ko,
        };
    }
}
