using Cysharp.Threading.Tasks;
using Generated;

public partial class CutsceneManager : SingletonMonoBehaviour<CutsceneManager>
{
    public bool IsPlaying { get; private set; }
    
    private void Start()
    {
        EndCutscene();
    }

    public async UniTask PlayCutscene(int cutsceneId)
    {
        if (!GameData.Instance.ContainsCutsceneInfoData(cutsceneId))
        {
            LogManager.LogErrorFormat("CutsceneManager: Invalid cutscene id", cutsceneId);
            return;
        }

        var cutsceneDataList = GameData.Instance.GetCutsceneDataListById(cutsceneId);
        if (cutsceneDataList.IsNullOrEmpty())
        {
            LogManager.LogErrorFormat("CutsceneManager: 컷신이 없음", cutsceneId);
            return;
        }

        IsPlaying = true;
        foreach (var cutsceneData in cutsceneDataList)
        {
            await PlayCutsceneImpl(cutsceneData);
        }

        EndCutscene();
    }

    private void EndCutscene()
    {
        IsPlaying = false;
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
            case CutsceneCommand.Selection:
            {
                ShowSelection(data.StringValues.GetAt(0), data.IntValues);
                var selectionIndex = await GetWaitSelectionClickTask();
                HideSelection();

                var selectionData = GameData.Instance.GetCutsceneSelectionData(data.IntValues.GetAt(selectionIndex));
                await PlayCutscene(selectionData.SelectionCutsceneId);
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