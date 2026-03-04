using Cysharp.Threading.Tasks;

public partial class GlobalManager
{
    public void InvokeNpcFunction(NpcMenuFunctionType functionType, int functionValue)
    {
        switch (functionType)
        {
            case NpcMenuFunctionType.PlayCutscene:
            {
                CutsceneManager.Instance.PlayCutscene(functionValue).Forget();
                break;
            }
            case NpcMenuFunctionType.PlaySlimeMinigame:
            {
                PopupManager.Instance.ShowPopupWithEmptyParameter(PopupManager.Type.SlimeMinigameReady);
                break;
            }
            case NpcMenuFunctionType.PlayPowderPortalMinigame:
            {
                var artifactData = GameData.Instance.GetArtifactData(UserData.Instance.EquippedArtifactId);
                if (artifactData.ArtifactType != ArtifactType.Bubblegun)
                {
                    ShowToastMessage("버블건을 장착하자."); // TODO
                    return;
                }
                PanelManager.Instance.ShowPanel(PanelManager.Type.SlimeMinigame);
                break;
            }
            default:
            {
                LogManager.LogErrorPack("Undefined NPC menu function type", functionType);
                break;
            }
        }
    }
}