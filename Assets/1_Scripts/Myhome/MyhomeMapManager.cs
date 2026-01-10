public class MyhomeMapManager : BaseMapManager
{
    protected override void Start()
    {
        base.Start();
        PanelManager.Instance.ShowPanel(PanelManager.Type.MyhomeMain);
    }
}