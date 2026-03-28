using Cysharp.Threading.Tasks;

public class SlimeMinigameReadyPopup : EmptyParamPopup
{
    protected override void OnShow() { }

    protected override void OnHide() { }

    public void OnStartButtonClick()
    {
        var artifactData = GameData.Instance.GetArtifactData(UserData.Instance.EquippedArtifactId);
        if (artifactData.ArtifactType != ArtifactType.Bubblegun)
        {
            if (!UserData.Instance.TrySetEquippedArtifact(ArtifactType.Bubblegun))
            {
                OnHideButtonClick();
                CutsceneManager.Instance.PlayCutscene(GameSetting.Instance.SlimeMinigameArtifactCutsceneId).Forget();
                return;
            }
        }
        OnHideButtonClick();
        PanelManager.Instance.ShowPanel(PanelManager.Type.SlimeMinigame);
    }

    public void OnInfoButtonClick()
    {
        OnHideButtonClick();
        PopupManager.Instance.ShowPopupWithEmptyParameter(PopupManager.Type.SlimeMinigameInfo)
            .SetOnHideAction(() => { PopupManager.Instance.ShowPopupWithEmptyParameter(PopupManager.Type.SlimeMinigameReady); });
    }
}