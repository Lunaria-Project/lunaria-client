using System.Collections.Generic;

public partial class PopupManager
{
    public enum Type
    {
        None = 0,
        NpcSelection = 1,
    }

    private readonly Dictionary<Type, string> _popupResourceKey = new()
    {
        { Type.NpcSelection, "npc_selection_popup" },
    };
}