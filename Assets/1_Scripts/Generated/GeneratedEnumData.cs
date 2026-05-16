[SerializeEnum]
public enum ArtifactType
{
    None = 0,
    Stick = 1,
    Powder = 2,
    Bubblegun = 3,
    Grinder = 4,
    Pendant = 5,
}

[SerializeEnum]
public enum CutsceneCommand
{
    None = 0,
    ShowDialog = 1,
    ShowFullIllustration = 2,
    HideFullIllustration = 3,
    ShowSpotIllustration = 4,
    HideSpotIllustration = 5,
    Selection = 6,
    ShowShopPopup = 7,
}

[SerializeEnum]
public enum InventoryTabType
{
    None = 0,
    Equipment = 1,
    Material = 2,
    Quest = 3,
}

[SerializeEnum]
public enum ItemType
{
    None = 0,
    MainCoin = 1,
    Artifact = 2,
    PlayerHpRecovery = 3,
    FamiliarHpRecovery = 4,
    FamiliarTiredRecovery = 5,
    RefinedMaterial = 6,
    Material = 7,
    Quest = 8,
    Ride = 9,
    FamiliarCall = 10,
}

[SerializeEnum]
public enum LoadingType
{
    None = 0,
    Normal = 1,
    CottonCandyShop = 2,
    PowderShop = 3,
    BeddingShop = 4,
}

[SerializeEnum]
public enum MapType
{
    None = 0,
    Myhome = 1,
    ShoppingSquare = 2,
    PowderShop = 3,
    CottonCandyShop = 4,
}

[SerializeEnum]
public enum MinigameType
{
    None = 0,
    Slime = 1,
    PowderPortal = 2,
    CottonCandy = 3,
}

[SerializeEnum]
public enum NpcMenuFunctionType
{
    None = 0,
    PlayCutscene = 1,
    PlayFixedCutscene = 2,
    PlaySlimeMinigame = 3,
    PlayPowderPortalMinigame = 4,
    PlayCottonCandyMinigame = 5,
}

[SerializeEnum]
public enum RequirementType
{
    None = 0,
    AlwaysTrue = 1,
    AlwaysFalse = 2,
    ActivateDuration = 3,
    MyhomeSlimeAppeared = 4,
}

[SerializeEnum]
public enum ShopType
{
    None = 0,
    CottonCandyShop = 1,
    PowderShop = 2,
    BeddingShop = 3,
}

