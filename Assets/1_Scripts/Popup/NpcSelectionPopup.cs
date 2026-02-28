using Cysharp.Threading.Tasks;
using Lunaria;
using UnityEngine;

public struct NpcSelectionPopupParameter : IPopupParameter
{
    public int NpcDataId { get; init; }
}

public class NpcSelectionPopup : Popup<NpcSelectionPopupParameter>
{
    [SerializeField] private GameObject[] _selections;
    [SerializeField] private Text[] _selectionTexts;

    private int _npcDataId;

    protected override void OnShow(NpcSelectionPopupParameter parameter)
    {
        GameTimeManager.Instance.Pause();

        _npcDataId = parameter.NpcDataId;

        var npcDataList = GameData.Instance.GetActivatedMapNpcMenuDataListByNpcId(_npcDataId);
        var count = Mathf.Min(_selections.Length, npcDataList.Count, _selectionTexts.Length);
        _selections.SetActiveAll(false);
        for (var i = 0; i < count; i++)
        {
            _selections[i].SetActive(true);
            _selectionTexts[i].SetText(npcDataList.GetAt(i).MenuName);
        }
        _selections.GetAt(count).SetActive(true);
        _selectionTexts.GetAt(count).SetText("대화 마치기"); //TODO
    }

    protected override void OnHide()
    {
        GameTimeManager.Instance.Resume();
    }

    public void OnSelectionButtonClick(int index)
    {
        var npcDataList = GameData.Instance.GetActivatedMapNpcMenuDataListByNpcId(_npcDataId);
        if (index == npcDataList.Count)
        {
            OnHideButtonClick();
            return;
        }
        var npcData = npcDataList.GetAt(index);
        SelectNpcMenu(npcData.FunctionType, npcData.FunctionValue);
        OnHideButtonClick();
    }

    public static void SelectNpcMenu(NpcMenuFunctionType type, int value)
    {
        switch (type)
        {
            case NpcMenuFunctionType.PlayCutscene:
            {
                CutsceneManager.Instance.PlayCutscene(value).Forget();
                break;
            }
            case NpcMenuFunctionType.PlaySlimeMinigame:
            {
                var artifactData = GameData.Instance.GetArtifactData(UserData.Instance.EquippedArtifactId);
                if (artifactData.ArtifactType != ArtifactType.Bubblegun)
                {
                    GlobalManager.Instance.ShowToastMessage("버블건을 장착하자."); // TODO
                    return;
                }
                PanelManager.Instance.ShowPanel(PanelManager.Type.SlimeMinigame);
                break;
            }
            default:
            {
                LogManager.LogErrorPack("Undefined NPC menu function type", type);
                break;
            }
        }
    }
}