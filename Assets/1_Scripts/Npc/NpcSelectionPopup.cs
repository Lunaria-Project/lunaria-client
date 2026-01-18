using Cysharp.Threading.Tasks;
using Generated;
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
        _npcDataId = parameter.NpcDataId;

        var npcDataList = GameData.Instance.GetActivatedMapNpcMenuDataListByNpcId(_npcDataId);
        var count = Mathf.Min(_selections.Length, npcDataList.Count, _selectionTexts.Length);
        _selections.SetActiveAll(false);
        for (var i = 0; i < count; i++)
        {
            _selections[i].SetActive(true);
            _selectionTexts[i].SetText(npcDataList.GetAt(i).MenuName);
        }
    }

    protected override void OnHide() { }

    public void OnSelectionButtonClick(int index)
    {
        var npcDataList = GameData.Instance.GetActivatedMapNpcMenuDataListByNpcId(_npcDataId);
        var npcData = npcDataList.GetAt(index);
        switch (npcData.FunctionType)
        {
            case NpcMenuFunctionType.PlayCutscene:
            {
                CutsceneManager.Instance.PlayCutscene(npcData.FunctionValue).Forget();
                break;
            }
            case NpcMenuFunctionType.PlaySlimeMinigame:
            {
                PanelManager.Instance.ShowPanel(PanelManager.Type.SlimeMinigame);
                break;
            }
        }
        OnHideButtonClick();
    }
}