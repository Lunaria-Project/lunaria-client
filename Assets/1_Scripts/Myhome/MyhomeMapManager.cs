public class MyhomeMapManager : BaseMapManager
{
    protected override void Start()
    {
        base.Start();
        PanelManager.Instance.ShowPanel(PanelManager.Type.MyhomeMain);
    }

    protected override bool CanShowNpcSelectionPopup()
    {
        if (!base.CanShowNpcSelectionPopup()) return false;
        if (PanelManager.Instance.CurrentPanelInfo.Type != PanelManager.Type.MyhomeMain) return false;
        return true;
    }
}