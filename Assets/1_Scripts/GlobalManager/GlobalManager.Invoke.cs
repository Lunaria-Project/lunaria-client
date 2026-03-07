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
                PopupManager.Instance.ShowPopupWithEmptyParameter(PopupManager.Type.PowderPortalMinigameReady);
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