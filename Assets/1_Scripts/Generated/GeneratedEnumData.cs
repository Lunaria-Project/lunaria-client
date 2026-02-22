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
    public enum LoadingType
    {
        None = 0,
        BeddingShop = 1,
        CottonCandyShop = 2,
        Normal = 3,
        PowderShop = 4,
    }

    [SerializeEnum]
    public enum MapType
    {
        None = 0,
        Myhome = 1,
        PowderShop = 2,
        ShoppingSquare = 3,
    }

    [SerializeEnum]
    public enum NpcMenuFunctionType
    {
        None = 0,
        PlayCutscene = 1,
        PlayFixedCutscene = 2,
        PlayPowderPortalMinigame = 3,
        PlaySlimeMinigame = 4,
    }

    [SerializeEnum]
    public enum RequirementType
    {
        None = 0,
        ActivateDuration = 1,
        AlwaysFalse = 2,
        AlwaysTrue = 3,
        MyhomeSlimeAppeared = 4,
    }

    [SerializeEnum]
    public enum ShopType
    {
        None = 0,
        BeddingShop = 1,
        CottonCandyShop = 2,
        PowderShop = 3,
    }

}
