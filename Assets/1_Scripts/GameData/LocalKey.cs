public readonly struct LocalKey
{
    public readonly string Value;
    public LocalKey(string value) => Value = value;

    public string Format(params object[] args)
    {
        if (!GameData.Instance.TryGetLocalization(Value, out var localization)) return Value;
        var template = GlobalManager.Instance.LocalType switch
        {
            GameData.LocalType.Ko => localization.Ko,
            GameData.LocalType.En => localization.En,
            GameData.LocalType.Ja => localization.Ja,
            _ => localization.Ko,
        };
        return string.Format(template, args);
    }
}
