using System.Collections.Generic;

public partial class PopupManager
{
    public enum Type
    {
        None = 0,
        NpcSelection = 1,
        CountDown = 2,
        Shortcut = 3,
    }

    private readonly Dictionary<Type, string> _popupResourceKey = new()
    {
        { Type.NpcSelection, "npc_selection_popup" },
        { Type.CountDown, "count_down_popup" },
        { Type.Shortcut, "shortcut_popup" },
    };
}