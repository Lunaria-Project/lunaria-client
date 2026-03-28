namespace Lunaria
{
    public class Text : TMPro.TextMeshProUGUI
    {
        public void SetText(LocalKey key)
        {
            if (!GameData.Instance.TryGetLocalization(key.Value, out var localization)) return;

            text = GlobalManager.Instance.LocalType switch
            {
                GameData.LocalType.Ko => localization.Ko,
                GameData.LocalType.En => localization.En,
                GameData.LocalType.Ja => localization.Ja,
                _                     => localization.Ko,
            };
        }
    }
}