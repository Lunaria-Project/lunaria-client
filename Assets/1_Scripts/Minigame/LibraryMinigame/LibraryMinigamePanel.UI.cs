using Lunaria;
using UnityEngine;
using UnityEngine.UI;

using UiButton = Lunaria.Button;
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
            var shelfCell = shelfRect.Find($"ShelfCell_{cellIndex}")?.GetComponent<UiImage>();
            if (shelfCell == null) continue;
            _shelfCells[cellIndex] = shelfCell;

            var button = shelfCell.GetComponent<UiButton>() ?? shelfCell.gameObject.AddComponent<UiButton>();
            button.targetGraphic = shelfCell;
            button.transition = Selectable.Transition.ColorTint;

            // 양옆 칸이 같은 줄에 있는 가운데 열만 책을 꺼낼 수 있습니다.
            var column = cellIndex % ShelfColumnCount;
            if (column > 0 && column < ShelfColumnCount - 1)
            {
                var clickedCellIndex = cellIndex;
                button.onClick.AddListener(() => ReturnLastBooksFromShelf(clickedCellIndex));
            }
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
}
