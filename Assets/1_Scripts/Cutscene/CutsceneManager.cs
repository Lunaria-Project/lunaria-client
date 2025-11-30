using Cysharp.Threading.Tasks;
using Generated;

public partial class CutsceneManager : SingletonMonoBehaviourDontDestroy<CutsceneManager>
{
    private void Start()
    {
        EndCutscene();
    }

    public async UniTask PlayCutscene(int cutsceneGroupId)
    {
        if (!GameData.Instance.ContainsCutsceneGroupData(cutsceneGroupId))
        {
            LogManager.LogErrorFormat("CutsceneManager: Invalid cutscene group id", cutsceneGroupId);
            return;
        }

        var cutsceneDataList = GameData.Instance.GetCutsceneGroupDataListByGroupId(cutsceneGroupId);
        if (cutsceneDataList.IsNullOrEmpty())
        {
            LogManager.LogErrorFormat("CutsceneManager: 컷신이 없음", cutsceneGroupId);
            return;
        }

        foreach (var cutsceneData in cutsceneDataList)
        {
            await PlayCutsceneImpl(cutsceneData);
        }

        EndCutscene();
    }

    private void EndCutscene()
    {
        ClearUI();
        ClearTask();
    }

    private async UniTask PlayCutsceneImpl(CutsceneData data)
    {
        switch (data.CutsceneCommand)
        {
            case CutsceneCommand.ShowDialog:
            {
                ShowDialog(data);
                await GetWaitClickTask();
                break;
            }
            case CutsceneCommand.ShowFullIllustration:
            {
                ShowFullIllustration(data);
                await GetWaitClickTask();
                break;
            }
            case CutsceneCommand.HideFullIllustration:
            {
                HideFullIllustration();
                break;
            }
            case CutsceneCommand.ShowSpotIllustration:
            {
                ShowSpotIllustration(data);
                await GetWaitClickTask();
                break;
            }
            case CutsceneCommand.HideSpotIllustration:
            {
                HideSpotIllustration();
                break;
            }
            default:
            {
                LogManager.LogErrorFormat("CutsceneManager: Invalid CutsceneCommand", data.CutsceneCommand);
                break;
            }
        }
    }
}