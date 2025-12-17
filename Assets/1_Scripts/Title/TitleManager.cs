public class TitleManager : SingletonMonoBehaviour<TitleManager>
{

    public void Start()
    {
        GameData.Instance.LoadGameData();
        PanelManager.Instance.ShowPanel(PanelManager.Type.TitleMain);
    }
}