using System.Collections.Generic;
using Generated;
using UnityEngine;
using Lunaria;

public partial class CutsceneManager
{
    [SerializeField] private Image _fullIllustration;
    [SerializeField] private Image _spotIllustration;
    [SerializeField] private RectTransform _spotIllustrationTransform;
    [SerializeField] private GameObject _dialogBlock;
    [SerializeField] private Image _dialogCharacter;
    [SerializeField] private RectTransform _dialogNpcTransform;
    [SerializeField] private Text _dialogText;
    [SerializeField] private GameObject _selectionBlock;
    [SerializeField] private GameObject[] _selectionButtons;
    [SerializeField] private Image[] _selectionImages;
    [SerializeField] private Text[] _selectionTexts;

    private const string IsFlipped = "Flipped";

    private void ShowFullIllustration(CutsceneData data)
    {
        var resourceKey = data.StringValues.GetAtWithError(0);
        _fullIllustration.SetActive(true);
        _fullIllustration.SetSprite(ResourceManager.Instance.LoadSprite(resourceKey));
    }

    private void HideFullIllustration()
    {
        _fullIllustration.SetActive(false);
    }

    private void ShowSpotIllustration(CutsceneData data)
    {
        var resourceKey = data.StringValues.GetAtWithError(0);
        _spotIllustration.SetActive(true);
        _spotIllustration.SetSprite(ResourceManager.Instance.LoadSprite(resourceKey));
        _spotIllustrationTransform.anchoredPosition = data.Position;
    }

    private void HideSpotIllustration()
    {
        _spotIllustration.SetActive(false);
    }

    private void ShowDialog(CutsceneData data)
    {
        var isFlipped = data.StringValues.IsNullOrEmpty() || string.Equals(data.StringValues.GetAtWithError(0), IsFlipped);
        _dialogBlock.SetActive(true);
        //TODO(지선) : 캐릭터 데이터를 만들어서, 캐릭터아이디를 통해 그 리소스키를 읽어오도록 수정
        //_dialogCharacter.SetSprite(ResourceManager.Instance.LoadSprite(characterResourceKey));
        _dialogNpcTransform.anchoredPosition = data.Position;
        _dialogNpcTransform.localScale = isFlipped ? new Vector3(-1, 1, 1) : Vector3.one;
        _dialogText.SetText(data.CutsceneMessage);
    }

    private void HideDialog()
    {
        _dialogBlock.SetActive(false);
    }

    private void ShowSelection(string resourceKey, List<int> selectionIds)
    {
        _selectionBlock.SetActive(true);
        _selectionButtons.SetActiveAll(false);
        var selectionImage = ResourceManager.Instance.LoadSprite(resourceKey);
        foreach (var image in _selectionImages)
        {
            image.SetSprite(selectionImage);
        }
        var index = 0;
        foreach (var selectionId in selectionIds)
        {
            if (!GameData.Instance.TryGetCutsceneSelectionData(selectionId, out var selectionData)) continue;
            if (!RequirementManager.Instance.IsSatisfied(selectionData.ShowRequirement, selectionData.ShowRequirementValues)) continue;
            if (RequirementManager.Instance.IsSatisfied(selectionData.HideRequirement, selectionData.HideRequirementValues)) continue;

            _selectionButtons.GetAt(index).SetActive(true);
            _selectionTexts.GetAt(index++).SetText(selectionData.SelectionTitle);
        }
    }

    private void HideSelection()
    {
        _selectionBlock.SetActive(false);
    }

    private void ClearUI()
    {
        _emptyButton.SetActive(false);
        _fullIllustration.SetActive(false);
        _spotIllustration.SetActive(false);
        _dialogBlock.SetActive(false);
        _selectionBlock.SetActive(false);
    }
}