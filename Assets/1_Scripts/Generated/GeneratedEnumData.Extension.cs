public static class GeneratedEnumExtensions
{
    public static string GetDisplayName(this ArtifactType value)
    {
        var key = value switch
        {
            ArtifactType.Stick     => "EnumData.Enum.ArtifactType.1",
            ArtifactType.Powder    => "EnumData.Enum.ArtifactType.2",
            ArtifactType.Bubblegun => "EnumData.Enum.ArtifactType.3",
            ArtifactType.Grinder   => "EnumData.Enum.ArtifactType.4",
            ArtifactType.Pendant   => "EnumData.Enum.ArtifactType.5",
            _                      => value.ToString(),
        };
        return GameData.Instance.GetLocalString(key);
    }

    public static string GetDisplayName(this ItemType value)
    {
        var key = value switch
        {
            ItemType.MainCoin              => "EnumData.Enum.ItemType.1",
            ItemType.Artifact              => "EnumData.Enum.ItemType.2",
            ItemType.PlayerHpRecovery      => "EnumData.Enum.ItemType.3",
            ItemType.FamiliarHpRecovery    => "EnumData.Enum.ItemType.4",
            ItemType.FamiliarTiredRecovery => "EnumData.Enum.ItemType.5",
            ItemType.RefinedMaterial       => "EnumData.Enum.ItemType.6",
            ItemType.Material              => "EnumData.Enum.ItemType.7",
            ItemType.Quest                 => "EnumData.Enum.ItemType.8",
            ItemType.Ride                  => "EnumData.Enum.ItemType.9",
            ItemType.FamiliarCall          => "EnumData.Enum.ItemType.10",
            _                              => value.ToString(),
        };
        return GameData.Instance.GetLocalString(key);
    }

}
