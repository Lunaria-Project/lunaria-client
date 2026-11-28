using Lunaria;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.UI;
using UiButton = Lunaria.Button;
#endif
using UiImage = Lunaria.Image;

public partial class LibraryMinigamePanel
{
    private const float ShelfWidthRatio = 0.80f;
    private const float ShelfHeightRatio = 0.46f;
    private const float ShelfCenterHeightRatio = 0.15f;

    [SerializeField] private UiImage _bookshelfPrefab;
    [SerializeField] private MinigameRemainTime _timerPrefab;
    [SerializeField] private RectTransform _timeAttackEffectPrefab;

    private readonly UiImage[] _shelfCells = new UiImage[ShelfCellCount];
    private RectTransform _boardRoot;
    private MinigameRemainTime _timer;
    private RectTransform _timeAttackEffect;
    private Camera _uiCamera;
    private float _layoutWidth;
    private float _layoutHeight;
    private float _bookCursorSpacing;

    private void CreateBoard()
    {
        var oldBoard = transform.Find("PanelBlock");
        if (oldBoard != null) oldBoard.gameObject.SetActive(false);

        var rootObject = new GameObject("LibraryMinigameBoard", typeof(RectTransform), typeof(UiImage));
        _boardRoot = rootObject.GetComponent<RectTransform>();
        _boardRoot.SetParent(transform, false);
        Stretch(_boardRoot);
        var boardRect = _boardRoot.rect;
        _layoutWidth = boardRect.width;
        _layoutHeight = boardRect.height;
        rootObject.GetComponent<UiImage>().color = Color.clear;

        var shelfWidth = Mathf.Min(_layoutWidth * ShelfWidthRatio, 920f);
        var shelfHeight = Mathf.Min(_layoutHeight * ShelfHeightRatio, 450f);
        var shelfCenterY = _layoutHeight * ShelfCenterHeightRatio;
        var shelf = Instantiate(_bookshelfPrefab, _boardRoot, false);
        shelf.name = "LibraryBookshelfTemporary";
        var shelfRect = shelf.rectTransform;
        shelfRect.anchorMin = shelfRect.anchorMax = new Vector2(0.5f, 0.5f);
        shelfRect.sizeDelta = new Vector2(shelfWidth, shelfHeight);
        shelfRect.anchoredPosition = new Vector2(0f, shelfCenterY);
        var canvas = GetComponentInParent<Canvas>();
        _uiCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        for (var cellIndex = 0; cellIndex < _shelfCells.Length; cellIndex++)
        {
            _shelfCells[cellIndex] = shelfRect.Find($"ShelfCell_{cellIndex}")?.GetComponent<UiImage>();
        }

        if (_shelfCells[0] != null && _shelfCells[1] != null)
        {
            var firstCellCenter = _boardRoot.InverseTransformPoint(_shelfCells[0].rectTransform.position);
            var secondCellCenter = _boardRoot.InverseTransformPoint(_shelfCells[1].rectTransform.position);
            _bookCursorSpacing = Vector2.Distance(firstCellCenter, secondCellCenter);
        }
        else
        {
            _bookCursorSpacing = shelfWidth / ShelfColumnCount;
        }
    }

    private static void Stretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }

#if UNITY_EDITOR
    private const string EditorRetryButtonPref = "Lunaria.LibraryMinigame.ShowRetryButton";
    private UiButton _editorRetryButton;

    internal static bool EditorRetryButtonVisible
    {
        get => UnityEditor.EditorPrefs.GetBool(EditorRetryButtonPref, true);
        set
        {
            UnityEditor.EditorPrefs.SetBool(EditorRetryButtonPref, value);
            var panel = FindFirstObjectByType<LibraryMinigamePanel>();
            if (panel != null && panel._editorRetryButton != null)
            {
                panel._editorRetryButton.gameObject.SetActive(value);
            }
        }
    }

    private void CreateEditorRetryButton()
    {
        var buttonObject = new GameObject("EditorRetryButton", typeof(RectTransform), typeof(UiImage), typeof(UiButton));
        var image = buttonObject.GetComponent<UiImage>();
        var rect = image.rectTransform;
        rect.SetParent(_boardRoot, false);
        var topRight = new Vector2(1f, 1f);
        rect.anchorMin = rect.anchorMax = rect.pivot = topRight;
        rect.anchoredPosition = new Vector2(-24f, -24f);
        rect.sizeDelta = new Vector2(192f, 60f);
        image.color = new Color(0.20f, 0.29f, 0.39f);

        var label = new GameObject("Label", typeof(RectTransform), typeof(Lunaria.Text)).GetComponent<Lunaria.Text>();
        label.rectTransform.SetParent(rect, false);
        Stretch(label.rectTransform);
        label.font = UnityEditor.AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/Fonts/NPSfont_regular SDF.asset");
        label.text = "다시하기";
        label.fontSize = 30f;
        label.color = Color.white;
        label.alignment = TMPro.TextAlignmentOptions.Center;
        label.raycastTarget = false;

        _editorRetryButton = buttonObject.GetComponent<UiButton>();
        _editorRetryButton.targetGraphic = image;
        _editorRetryButton.navigation = new Navigation { mode = Navigation.Mode.None };
        _editorRetryButton.interactable = false;
        _editorRetryButton.onClick.AddListener(RetryRoundInEditor);
        buttonObject.SetActive(EditorRetryButtonVisible);
    }

    private void RetryRoundInEditor()
    {
        if (!_canInteract) return;

        ClearRound();
        StartRound();
    }
#endif
}
