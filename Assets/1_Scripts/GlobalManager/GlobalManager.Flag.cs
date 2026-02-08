public partial class GlobalManager
{
    public bool CanPlayerMove()
    {
        if (CutsceneManager.Instance.IsPlaying) return false;
        var currentPopup = PopupManager.Instance.GetCurrentPopup();
        if (currentPopup is NpcSelectionPopup) return false;
        return true;
    }
}
