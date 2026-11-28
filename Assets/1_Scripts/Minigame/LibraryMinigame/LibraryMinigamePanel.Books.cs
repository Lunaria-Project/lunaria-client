using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UiImage = Lunaria.Image;

public partial class LibraryMinigamePanel
{
    
    private const int DeckSetCount = ShelfCellCount + 1; // 마지막 칸까지 완성하도록 예비 3권
    private const int PileLayerCount = 2;
    private const float FloorHeightRatio = -0.40f;
    private const float PileWidthRatio = 0.36f;
    private const float PileLayerStaggerRatio = 0.45f;
    private const float PileHorizontalJitterRatio = 0.12f;
    private const float PileLayerSpacing = 24f;
    private const float PileVerticalJitter = 10f;
    private const float LyingBookChance = 0.82f;

    [SerializeField] private LibraryMinigameConfig _config;
    [SerializeField] private UiImage _bookPrefab;

    private sealed class Book
    {
        public LibraryMinigameConfig.BookType Type;
        public bool IsPlaced;
        public bool IsMoving;
        public Vector2 FloorPosition;
        public Vector2 PickupStartPosition;
        public UiImage Image;
        public RectTransform RectTransform;
    }

    private readonly List<Book> _books = new(DeckSetCount * BooksPerShelf);

    private void CreateBooks()
    {
        var deck = _config.CreateDeck(DeckSetCount);
        var floorY = _layoutHeight * FloorHeightRatio;
        var pileWidth = Mathf.Min(_layoutWidth * PileWidthRatio, deck.Count * 40f);
        var booksPerLayer = Mathf.CeilToInt(deck.Count / (float)PileLayerCount);
        var pileStep = pileWidth / Mathf.Max(booksPerLayer - 1, 1);
        var horizontalJitter = pileStep * PileHorizontalJitterRatio;

        for (var index = 0; index < deck.Count; index++)
        {
            var pileLayer = index % PileLayerCount;
            var pileColumn = index / PileLayerCount;
            var itemsInLayer = Mathf.CeilToInt((deck.Count - pileLayer) / (float)PileLayerCount);
            var centeredColumn = pileColumn - (itemsInLayer - 1) * 0.5f;
            var layerStagger = pileLayer == 0 ? 0f : pileStep * PileLayerStaggerRatio;
            var x = centeredColumn * pileStep + layerStagger + Random.Range(-horizontalJitter, horizontalJitter);
            var y = floorY + pileLayer * PileLayerSpacing + Random.Range(-PileVerticalJitter, PileVerticalJitter);
            var floorPosition = new Vector2(x, y);
            var floorRotation = Random.value < LyingBookChance
                ? Random.Range(55f, 125f) * (Random.value < 0.5f ? -1f : 1f)
                : Random.Range(-25f, 25f);

            var type = deck[index];
            var image = Instantiate(_bookPrefab, _boardRoot, false);
            image.name = $"LibraryBook_{index + 1}";
            image.sprite = type.Cover;
            image.preserveAspect = true;
            image.raycastTarget = true;
            var rectTransform = image.rectTransform;
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = _bookSize;
            rectTransform.anchoredPosition = floorPosition + new Vector2(
                Random.Range(-32f, 32f), Random.Range(100f, 160f));
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f));

            var book = new Book
            {
                Type = type,
                FloorPosition = floorPosition,
                Image = image,
                RectTransform = rectTransform,
                IsMoving = true,
            };
            _books.Add(book);

            var fallDuration = Random.Range(0.35f, 0.55f);
            var fallSequence = DOTween.Sequence().SetTarget(rectTransform).SetDelay(index * 0.025f);
            fallSequence.Join(rectTransform.DOAnchorPos(floorPosition, fallDuration).SetEase(Ease.OutBounce));
            fallSequence.Join(rectTransform.DORotate(new Vector3(0f, 0f, floorRotation), fallDuration).SetEase(Ease.OutBounce));
            fallSequence.OnComplete(() => book.IsMoving = false);
        }
    }
}
