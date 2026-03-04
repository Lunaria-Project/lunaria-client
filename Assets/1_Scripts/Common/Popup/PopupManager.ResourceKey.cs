using System.Collections.Generic;

public partial class PopupManager
{
    public enum Type
    {
        None = 0,
        NpcSelection = 1,
        CountDown = 2,
        Shortcut = 3,
        SlimeMinigameInfo = 4,
        SlimeMinigameReady = 5,
        SlimeMinigameResult = 6,
    }

    private readonly Dictionary<Type, string> _popupResourceKey = new()
    {
        { Type.NpcSelection, "npc_selection_popup" },
        { Type.CountDown, "count_down_popup" },
        { Type.Shortcut, "shortcut_popup" },
        { Type.SlimeMinigameInfo, "slime_minigame_info_popup" },
        { Type.SlimeMinigameReady, "slime_minigame_ready_popup" },
        { Type.SlimeMinigameResult, "slime_minigame_result_popup" },
    };
}