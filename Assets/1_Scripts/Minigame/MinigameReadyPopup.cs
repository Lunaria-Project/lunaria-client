using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class MinigameReadyPopup : EmptyParamPopup
{
    [SerializeField] private MinigameType _minigameType;
    [SerializeField, CanBeNull] private GameObject _familiarButton;

    protected override void OnShow()
    {
        if (_familiarButton != null)
        {
            // TODO(지선) : 패밀리어 시스템 작업할 때 하기
            _familiarButton.SetActive(false);
        }
    }

    protected override void OnHide() { }

    private int GetArtifactCutsceneId()
    {
        return _minigameType switch
        {
            MinigameType.Slime        => GameSetting.Instance.SlimeMinigameArtifactCutsceneId,
            MinigameType.PowderPortal => GameSetting.Instance.PowderMinigameArtifactCutsceneId,
            MinigameType.CottonCandy  => GameSetting.Instance.CottonCandyMinigameArtifactCutsceneId,
            _                         => 0,
        };
    }

    private PanelManager.Type GetMinigamePanelType()
    {
        return _minigameType switch
        {
            MinigameType.Slime        => PanelManager.Type.SlimeMinigame,
            MinigameType.PowderPortal => PanelManager.Type.PowderPortalMinigame,
            MinigameType.CottonCandy  => PanelManager.Type.CottonCandyMinigame,
            _                         => PanelManager.Type.None,
        };
    }

    private PopupManager.Type GetInfoPopupType()
    {
        return _minigameType switch
        {
            MinigameType.Slime        => PopupManager.Type.SlimeMinigameInfo,
            MinigameType.PowderPortal => PopupManager.Type.PowderPortalMinigameInfo,
            MinigameType.CottonCandy  => PopupManager.Type.CottonCandyMinigameInfo,
            _                         => PopupManager.Type.None,
        };
    }

    private PopupManager.Type GetReadyPopupType()
    {
        return _minigameType switch
        {
            MinigameType.Slime        => PopupManager.Type.SlimeMinigameReady,
            MinigameType.PowderPortal => PopupManager.Type.PowderPortalMinigameReady,
            MinigameType.CottonCandy  => PopupManager.Type.CottonCandyMinigameReady,
            _                         => PopupManager.Type.None,
        };
    }

    public void OnStartButtonClick()
    {
        var requiredArtifactType = GameData.Instance.GetMinigameInfoData(_minigameType).EquippedArtifactType;
        var artifactData = GameData.Instance.GetArtifactData(UserData.Instance.EquippedArtifactId);
        if (artifactData.ArtifactType != requiredArtifactType)
        {
            if (!UserData.Instance.TrySetEquippedArtifact(requiredArtifactType))
            {
                OnHideButtonClick();
                CutsceneManager.Instance.PlayCutscene(GetArtifactCutsceneId()).Forget();
                return;
            }
        }
        OnHideButtonClick();
        PanelManager.Instance.ShowPanel(GetMinigamePanelType());
    }

    public void OnInfoButtonClick()
    {
        OnHideButtonClick();
        PopupManager.Instance.ShowPopupWithEmptyParameter(GetInfoPopupType())
            .SetOnHideAction(() => { PopupManager.Instance.ShowPopupWithEmptyParameter(GetReadyPopupType()); });
    }

    public void OnFamiliarButtonClick()
    {
        // TODO(지선)
    }
}
