namespace Generated
{
    [SerializeEnum]
    public enum CutsceneCommand
    {
        None = 0,
        HideFullIllustration = 1,
        HideSpotIllustration = 2,
        Selection = 3,
        ShowDialog = 4,
        ShowFullIllustration = 5,
        ShowSpotIllustration = 6,
    }

    [SerializeEnum]
    public enum ItemType
    {
        None = 0,
        MainCoin = 1,
    }

    [SerializeEnum]
    public enum NpcMenuFunctionType
    {
        None = 0,
        PlayCutscene = 1,
    }

    [SerializeEnum]
    public enum RequirementType
    {
        None = 0,
        AlwaysFalse = 1,
        AlwaysTrue = 2,
    }

}
