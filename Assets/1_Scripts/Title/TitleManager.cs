public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    protected override void Start()
    {
        GameData.Instance.LoadGameData();
        GlobalManager.Instance.SetCursor(CursorType.DefaultEmpty);
        PanelManager.Instance.ShowPanel(PanelManager.Type.TitleMain);
        GlobalManager.Instance.InitUI();
    }
}