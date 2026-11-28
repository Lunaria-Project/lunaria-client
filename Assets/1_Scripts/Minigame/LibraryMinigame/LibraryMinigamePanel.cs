using System.Collections.Generic;
using DG.Tweening;
using Lunaria;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class LibraryMinigamePanel : Panel<LibraryMinigamePanel>, IPointerDownHandler, IPointerUpHandler
{
    private const int BooksPerSelection = 3;
    private const int BooksPerShelf = 3;
    private const int ShelfRowCount = 3;
    private const int ShelfColumnCount = 5;
    private const int ShelfCellCount = ShelfRowCount * ShelfColumnCount;
    private const int DefaultMinigameSeconds = 60;
    private const float TimeAttackSeconds = 10f;
    private static readonly int TimeAttackState = Animator.StringToHash("Base Layer.item_minigame_timer_timeattack");
    private const float ShelfBookSpacing = 44f;

    private readonly List<Book> _selectedBooks = new(BooksPerSelection);
    private readonly Book[,] _shelfBooks = new Book[ShelfCellCount, BooksPerShelf];
    private readonly Vector3[] _worldCorners = new Vector3[4];
    private float _minigameTime;
    private float _remainingTime;
    private Sequence _pickupSequence;
    private Sequence _resolutionSequence;
    private float _pickupProgress;
    private bool _canInteract;
    private bool _isResolving;

    protected override void OnShow(params object[] args)
    {
        GlobalManager.Instance.IsMinigamePlaying = true;
        GameTimeManager.Instance.Pause(this);
        StartRound();
    }

    private void StartRound()
    {
        CreateBoard();
        CreateBooks();
        InitTimer();
#if UNITY_EDITOR
        CreateEditorRetryButton();
#endif

        var currentBoard = _boardRoot;
        PopupManager.Instance.ShowPopup(PopupManager.Type.CountDown, new CountDownPopupParameter
        {
            CountDownSeconds = 3,
        }).SetOnHideAction(() =>
        {
            if (currentBoard == null || _boardRoot != currentBoard) return;
            _canInteract = true;
#if UNITY_EDITOR
            _editorRetryButton.interactable = true;
#endif
        });
    }

    protected override void OnHide()
    {
        GlobalManager.Instance.IsMinigamePlaying = false;
        GameTimeManager.Instance.Resume(this);
        ClearRound();
    }

    private void ClearRound()
    {
        _canInteract = false;
        _isResolving = false;

        _pickupSequence?.Kill();
        _pickupSequence = null;
        _pickupProgress = 0f;
        _resolutionSequence?.Kill();
        _resolutionSequence = null;

        foreach (var book in _books)
        {
            if (book.RectTransform != null) book.RectTransform.DOKill();
        }

        if (_boardRoot != null)
        {
            _boardRoot.gameObject.SetActive(false);
            Destroy(_boardRoot.gameObject);
            _boardRoot = null;
        }

        _timer = null;
        _timeAttackEffect = null;
#if UNITY_EDITOR
        _editorRetryButton = null;
#endif
        _books.Clear();
        _selectedBooks.Clear();
        System.Array.Clear(_shelfBooks, 0, _shelfBooks.Length);
        System.Array.Clear(_shelfCells, 0, _shelfCells.Length);
    }

    private void InitTimer()
    {
        var configuredSeconds = GameData.Instance.GetMinigameInfoData(MinigameType.Library)?.MinigameSeconds ?? 0;
        _minigameTime = configuredSeconds > 0 ? configuredSeconds : DefaultMinigameSeconds;
        _remainingTime = _minigameTime;

        _timeAttackEffect = Instantiate(_timeAttackEffectPrefab, _boardRoot, false);
        _timeAttackEffect.gameObject.SetActive(false);
        _timeAttackEffect.gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
        Stretch(_timeAttackEffect);

        _timer = Instantiate(_timerPrefab, _boardRoot, false);
        var timerRect = (RectTransform)_timer.transform;
        var topCenter = new Vector2(0.5f, 1f);
        timerRect.anchorMin = timerRect.anchorMax = timerRect.pivot = topCenter;
        timerRect.anchoredPosition = new Vector2(0f, -24f);
        // 타이머 장식이 책을 집거나 놓는 입력을 가로채지 않게 합니다.
        _timer.gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
        _timer.SetRemainTime(_remainingTime, _minigameTime);
    }

    private void Update()
    {
        if (!_canInteract) return;

        _remainingTime = Mathf.Max(0f, _remainingTime - Time.deltaTime);
        _timer.SetRemainTime(_remainingTime, _minigameTime);
        if (_remainingTime <= 0f)
        {
            _canInteract = false;
            HidePanel();
            return;
        }

        if (_remainingTime <= TimeAttackSeconds && !_timeAttackEffect.gameObject.activeSelf)
        {
            // 공용 테두리의 장식 위치는 1920×1080 UI 기준입니다.
            _timeAttackEffect.localScale = new Vector3(_boardRoot.rect.width / 1920f, _boardRoot.rect.height / 1080f, 1f);
            _timeAttackEffect.gameObject.SetActive(true);
            _timer.GetComponent<Animator>().Play(TimeAttackState, 0, 0f);
        }
    }

    private void LateUpdate()
    {
        if (!_canInteract || _isResolving || _selectedBooks.Count != BooksPerSelection) return;
        SetSelectedBooksAroundPointer(Input.mousePosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        var hitObject = eventData.pointerCurrentRaycast.gameObject;
        var book = _books.Find(candidate => candidate.Image.gameObject == hitObject);
        if (book != null) BeginBookDrag(book);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ResolveSelectedBooksAtPointer(eventData.position);
    }

    private void BeginBookDrag(Book clickedBook)
    {
        if (!_canInteract || _isResolving || _selectedBooks.Count > 0 || clickedBook.IsPlaced || clickedBook.IsMoving) return;

        var randomCandidates = _books.FindAll(book => book != clickedBook && !book.IsPlaced && !book.IsMoving);
        if (randomCandidates.Count < BooksPerSelection - 1) return;

        var firstBookIndex = Random.Range(0, randomCandidates.Count);
        var firstRandomBook = randomCandidates[firstBookIndex];
        randomCandidates.RemoveAt(firstBookIndex);
        var thirdRandomBook = randomCandidates[Random.Range(0, randomCandidates.Count)];

        _selectedBooks.Add(firstRandomBook);
        _selectedBooks.Add(clickedBook);
        _selectedBooks.Add(thirdRandomBook);
        // 책이 모이는 중에도 이동 목표는 현재 커서 위치를 따라갑니다.
        const float pickupDuration = 0.3f;
        _pickupProgress = 0f;
        _pickupSequence = DOTween.Sequence();
        _pickupSequence.Join(DOTween.To(() => _pickupProgress, value => _pickupProgress = value, 1f, pickupDuration)
            .SetEase(Ease.OutBack));
        foreach (var book in _selectedBooks)
        {
            book.PickupStartPosition = book.RectTransform.anchoredPosition;
            book.Image.raycastTarget = false;
            _pickupSequence.Join(book.RectTransform.DORotate(Vector3.zero, pickupDuration).SetEase(Ease.OutQuad));
        }

        _pickupSequence.OnComplete(() => _pickupSequence = null);
    }

    private void ResolveSelectedBooksAtPointer(Vector2 screenPosition)
    {
        if (!_canInteract || _isResolving || _selectedBooks.Count != BooksPerSelection) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_boardRoot, screenPosition, _uiCamera, out _)) return;

        // 모이는 연출이 끝나기 전에 놓아도 마우스를 놓은 위치에서 판정합니다.
        _pickupSequence?.Kill();
        _pickupSequence = null;
        _pickupProgress = 1f;
        SetSelectedBooksAroundPointer(screenPosition);
        _isResolving = true;
        var sequence = _resolutionSequence = DOTween.Sequence();
        var rejected = new List<Book>(BooksPerSelection);

        for (var index = 0; index < _selectedBooks.Count; index++)
        {
            var book = _selectedBooks[index];
            book.RectTransform.localRotation = Quaternion.identity;
            var targetCellIndex = FindOverlappingShelfCell(book.RectTransform);
            var targetSlot = targetCellIndex >= 0 ? FindNextCellSlot(targetCellIndex) : -1;
            if (targetSlot >= 0)
            {
                _shelfBooks[targetCellIndex, targetSlot] = book;
                book.IsPlaced = true;
                AnimateShelfPlacement(sequence, book, targetCellIndex, targetSlot);
            }
            else
            {
                rejected.Add(book);
            }
        }

        const float returnToFloorDuration = 0.36f;
        foreach (var book in rejected)
        {
            book.IsMoving = true;
            var fallPosition = book.FloorPosition;
            var fall = book.RectTransform.DOAnchorPos(fallPosition, returnToFloorDuration).SetEase(Ease.InQuad);
            sequence.Join(fall);
            var tilt = Random.Range(-18f, 18f);
            sequence.Join(book.RectTransform.DORotate(new Vector3(0, 0, tilt), returnToFloorDuration));
        }

        sequence.OnComplete(() =>
        {
            foreach (var book in rejected)
            {
                book.IsMoving = false;
                book.Image.raycastTarget = true;
                book.RectTransform.localRotation = Quaternion.identity;
            }

            _selectedBooks.Clear();
            _resolutionSequence = null;
            _isResolving = false;
        });
    }

    private void AnimateShelfPlacement(Sequence sequence, Book book, int cellIndex, int slot)
    {
        var cellRect = _shelfCells[cellIndex].rectTransform;
        var worldPosition = cellRect.TransformPoint(new Vector3((slot - 1) * ShelfBookSpacing, 0f, 0f));
        var target = _boardRoot.InverseTransformPoint(worldPosition);
        book.RectTransform.sizeDelta = ShelvedBookSize;
        sequence.Join(book.RectTransform.DOAnchorPos(target, 0.42f).SetEase(Ease.OutBounce));
        sequence.Join(book.RectTransform.DORotate(Vector3.zero, 0.25f));
    }

    private bool IsShelfComplete(int cellIndex)
    {
        var left = _shelfBooks[cellIndex, 0];
        var middle = _shelfBooks[cellIndex, 1];
        var right = _shelfBooks[cellIndex, 2];
        if (left == null || middle == null || right == null) return false;

        var sameColor = left.ColorId == middle.ColorId && left.ColorId == right.ColorId;
        var samePattern = left.PatternId == middle.PatternId && left.PatternId == right.PatternId;
        if (left.Volume > 0 || middle.Volume > 0 || right.Volume > 0)
        {
            return sameColor && samePattern && left.Volume == 1 && middle.Volume == 2 && right.Volume == 3;
        }

        return sameColor || samePattern;
    }

    private void ReturnLastBooksFromShelf(int centerCellIndex)
    {
        if (!_canInteract || _isResolving || _selectedBooks.Count > 0) return;
        if (_shelfBooks[centerCellIndex, 0] == null) return;

        var returnedBooks = new List<Book>(BooksPerSelection);

        for (var cellIndex = centerCellIndex - 1; cellIndex <= centerCellIndex + 1; cellIndex++)
        {
            if (IsShelfComplete(cellIndex)) continue;

            // 책은 왼쪽부터 채워지므로 가장 오른쪽의 책이 마지막에 꽂힌 책입니다.
            for (var slot = _shelfBooks.GetLength(1) - 1; slot >= 0; slot--)
            {
                var book = _shelfBooks[cellIndex, slot];
                if (book == null) continue;

                _shelfBooks[cellIndex, slot] = null;
                book.IsPlaced = false;
                book.IsMoving = true;
                returnedBooks.Add(book);
                break;
            }
        }

        if (returnedBooks.Count == 0) return;

        _isResolving = true;
        var sequence = _resolutionSequence = DOTween.Sequence();
        const float returnDuration = 0.36f;
        foreach (var book in returnedBooks)
        {
            sequence.Join(book.RectTransform.DOAnchorPos(book.FloorPosition, returnDuration).SetEase(Ease.InQuad));
            sequence.Join(book.RectTransform.DOSizeDelta(FloorBookSize, returnDuration));
            sequence.Join(book.RectTransform.DOLocalRotate(new Vector3(0f, 0f, book.FloorRotation), returnDuration));
        }

        sequence.OnComplete(() =>
        {
            foreach (var book in returnedBooks)
            {
                book.IsMoving = false;
                book.Image.raycastTarget = true;
            }

            _resolutionSequence = null;
            _isResolving = false;
        });
    }

    private void SetSelectedBooksAroundPointer(Vector2 screenPosition)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_boardRoot, screenPosition, _uiCamera, out var pointerPosition)) return;

        for (var index = 0; index < _selectedBooks.Count; index++)
        {
            var book = _selectedBooks[index];
            var target = pointerPosition + new Vector2((index - 1) * _bookCursorSpacing, 0f);
            book.RectTransform.anchoredPosition = Vector2.LerpUnclamped(book.PickupStartPosition, target, _pickupProgress);
        }
    }

    private int FindNextCellSlot(int cellIndex)
    {
        for (var slot = 0; slot < _shelfBooks.GetLength(1); slot++)
        {
            if (_shelfBooks[cellIndex, slot] == null) return slot;
        }
        return -1;
    }

    private int FindOverlappingShelfCell(RectTransform bookRect)
    {
        var bookBounds = GetRectInRuntimeUi(bookRect);
        var bestCellIndex = -1;
        var bestOverlapArea = 0f;
        for (var cellIndex = 0; cellIndex < _shelfCells.Length; cellIndex++)
        {
            var cell = _shelfCells[cellIndex];
            if (cell == null) continue;

            var cellBounds = GetRectInRuntimeUi(cell.rectTransform);
            var overlapWidth = Mathf.Min(bookBounds.xMax, cellBounds.xMax) - Mathf.Max(bookBounds.xMin, cellBounds.xMin);
            var overlapHeight = Mathf.Min(bookBounds.yMax, cellBounds.yMax) - Mathf.Max(bookBounds.yMin, cellBounds.yMin);
            var overlapArea = overlapWidth > 0f && overlapHeight > 0f ? overlapWidth * overlapHeight : 0f;
            if (overlapArea <= bestOverlapArea) continue;

            bestOverlapArea = overlapArea;
            bestCellIndex = cellIndex;
        }
        return bestCellIndex;
    }

    private Rect GetRectInRuntimeUi(RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_worldCorners);
        var firstCorner = _boardRoot.InverseTransformPoint(_worldCorners[0]);
        var minX = firstCorner.x;
        var maxX = firstCorner.x;
        var minY = firstCorner.y;
        var maxY = firstCorner.y;

        for (var index = 1; index < _worldCorners.Length; index++)
        {
            var localCorner = _boardRoot.InverseTransformPoint(_worldCorners[index]);
            minX = Mathf.Min(minX, localCorner.x);
            maxX = Mathf.Max(maxX, localCorner.x);
            minY = Mathf.Min(minY, localCorner.y);
            maxY = Mathf.Max(maxY, localCorner.y);
        }

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }
}
