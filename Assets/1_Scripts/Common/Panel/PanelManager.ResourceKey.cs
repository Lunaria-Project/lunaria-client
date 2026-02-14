using System.Collections.Generic;

public partial class PanelManager
{
    private readonly Dictionary<Type, string> _panelResourceKey = new()
    {
        { Type.TitleMain, "title_main_panel" },
        { Type.MyhomeMain, "myhome_main_panel" },
        { Type.ShoppingSquareMain, "shopping_square_panel" },
        { Type.SlimeMinigame, "slime_minigame_panel" },
        { Type.Shop, "shop_main_panel" },
    };
}