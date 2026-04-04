using System.Collections.Generic;

public partial class PanelManager
{
    private readonly Dictionary<Type, string> _panelResourceKey = new()
    {
        { Type.TitleMain, "title_main_panel" },
        { Type.LunariaDefault, "lunaria_default_panel" },
        { Type.ShoppingSquareMain, "shopping_square_panel" },
        { Type.SlimeMinigame, "slime_minigame_panel" },
        { Type.PowderPortalMinigame, "powder_portal_minigame_panel" },
        { Type.Inventory, "inventory_panel" },
    };
}