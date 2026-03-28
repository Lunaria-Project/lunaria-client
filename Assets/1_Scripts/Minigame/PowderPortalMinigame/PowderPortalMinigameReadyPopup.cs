using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowderPortalMinigameReadyPopup : EmptyParamPopup
{
    [SerializeField] private GameObject _familiarButton;

    protected override void OnShow()
    {
        // TODO(지선) : 패밀리어 시스템 작업할 때 하기
        _familiarButton.SetActive(false);
    }

    protected override void OnHide() { }

    public void OnStartButtonClick()
    {
        var artifactData = GameData.Instance.GetArtifactData(UserData.Instance.EquippedArtifactId);
        if (artifactData.ArtifactType != ArtifactType.Powder)
        {
            if (!UserData.Instance.TrySetEquippedArtifact(ArtifactType.Powder))
            {
                OnHideButtonClick();
                CutsceneManager.Instance.PlayCutscene(GameSetting.Instance.PowderMinigameArtifactCutsceneId).Forget();
                return;
            }
        }
        OnHideButtonClick();
        PanelManager.Instance.ShowPanel(PanelManager.Type.PowderPortalMinigame);
    }

    public void OnInfoButtonClick()
    {
        OnHideButtonClick();
        PopupManager.Instance.ShowPopupWithEmptyParameter(PopupManager.Type.PowderPortalMinigameInfo)
            .SetOnHideAction(() => { PopupManager.Instance.ShowPopupWithEmptyParameter(PopupManager.Type.PowderPortalMinigameReady); });
    }

    public void OnFamiliarButtonClick()
    {
        // TODO(지선)
    }
}