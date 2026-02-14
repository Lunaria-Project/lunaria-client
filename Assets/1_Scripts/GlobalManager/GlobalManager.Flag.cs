public partial class GlobalManager
{
    public bool CanCharacterMove()
    {
        if (CutsceneManager.Instance.IsPlaying) return false;
        var currentPopup = PopupManager.Instance.GetCurrentPopup();
        if (currentPopup is NpcSelectionPopup) return false;
        if (GameTimeManager.Instance.IsPaused) return false;
        return true;
    }
}
