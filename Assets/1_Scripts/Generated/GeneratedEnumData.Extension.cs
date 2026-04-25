public static class GeneratedEnumExtensions
{
    public static string GetDisplayName(this ItemType value)
    {
        var key = value switch
        {
            ItemType.Artifact              => "EnumData.Enum.ItemType.2",
            ItemType.FamiliarCall          => "EnumData.Enum.ItemType.10",
            ItemType.FamiliarHpRecovery    => "EnumData.Enum.ItemType.4",
            ItemType.FamiliarTiredRecovery => "EnumData.Enum.ItemType.5",
            ItemType.MainCoin              => "EnumData.Enum.ItemType.1",
            ItemType.Material              => "EnumData.Enum.ItemType.7",
            ItemType.PlayerHpRecovery      => "EnumData.Enum.ItemType.3",
            ItemType.Quest                 => "EnumData.Enum.ItemType.8",
            ItemType.RefinedMaterial       => "EnumData.Enum.ItemType.6",
            ItemType.Ride                  => "EnumData.Enum.ItemType.9",
            _                              => value.ToString(),
        };
        return GameData.Instance.GetLocalString(key);
    }

}
