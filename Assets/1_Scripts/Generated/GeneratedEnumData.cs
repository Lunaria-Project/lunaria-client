namespace Generated
{
    [SerializeEnum]
    public enum ArtifactType
    {
        None = 0,
        Bubblegun = 1,
        Grinder = 2,
        Pendant = 3,
        Powder = 4,
        Stick = 5,
    }

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
        Artifact = 1,
        MainCoin = 2,
    }

    [SerializeEnum]
    public enum NpcMenuFunctionType
    {
        None = 0,
        PlayCutscene = 1,
        PlayFixedCutscene = 2,
        PlaySlimeMinigame = 3,
    }

    [SerializeEnum]
    public enum RequirementType
    {
        None = 0,
        ActivateDuration = 1,
        AlwaysFalse = 2,
        AlwaysTrue = 3,
    }

}
