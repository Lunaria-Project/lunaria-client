using Lunaria;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.UI;
using UiButton = Lunaria.Button;
#endif
using UiImage = Lunaria.Image;

public partial class LibraryMinigamePanel
{
    [SerializeField] private RectTransform _shelfGrid;
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
    private Vector2 _bookSize;

    private void CreateBoard()
    {
        var rootObject = new GameObject("LibraryMinigameBoard", typeof(RectTransform), typeof(UiImage));
        _boardRoot = rootObject.GetComponent<RectTransform>();
        _boardRoot.SetParent(transform, false);
        Stretch(_boardRoot);
        var boardRect = _boardRoot.rect;
        _layoutWidth = boardRect.width;
        _layoutHeight = boardRect.height;
        rootObject.GetComponent<UiImage>().raycastTarget = false;
        rootObject.GetComponent<UiImage>().color = Color.clear;
        var background = (RectTransform)_shelfGrid.parent;
        var panelBlock = (RectTransform)background.parent;
        var dim = panelBlock.GetComponent<UiImage>() ?? panelBlock.gameObject.AddComponent<UiImage>();
        dim.color = new Color(0f, 0f, 0f, 0.82f);
        dim.raycastTarget = true;

        var scale = Mathf.Min(_layoutWidth * 0.47f / background.rect.width,
                              _layoutHeight * 0.58f / background.rect.height);
        background.localScale = Vector3.one * scale;
        background.anchoredPosition = Vector2.zero;
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_shelfGrid);
        var canvas = GetComponentInParent<Canvas>();
        _uiCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        for (var cellIndex = 0; cellIndex < _shelfCells.Length; cellIndex++)
        {
            var cell = _shelfGrid.GetChild(cellIndex).GetComponent<UiImage>();
            _shelfCells[cellIndex] = cell;
            cell.raycastTarget = true;
            // 프리팹의 책 자리 3개는 배치 기준으로 유지하고, 화면에는 플레이에 쓰는 책만 표시합니다.
            for (var slot = 0; slot < BooksPerShelf; slot++)
                cell.transform.GetChild(slot).GetComponent<UiImage>().enabled = false;
        }
        _bookSize = GetRectInRuntimeUi((RectTransform)_shelfCells[0].transform.GetChild(0)).size;
        var first = _boardRoot.InverseTransformPoint(_shelfCells[0].transform.position);
        var next = _boardRoot.InverseTransformPoint(_shelfCells[1].transform.position);
        _bookCursorSpacing = Vector2.Distance(first, next);
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
