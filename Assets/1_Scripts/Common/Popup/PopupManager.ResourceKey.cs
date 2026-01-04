using System.Collections.Generic;

public partial class PopupManager
{
    public enum Type
    {
        None = 0,
        TitleMain = 1,
    }

    private readonly Dictionary<Type, string> _popupResourceKey = new()
    {
        { Type.TitleMain, "title_main_panel" },
    };
}