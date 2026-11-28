using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UiImage = Lunaria.Image;
using UiText = Lunaria.Text;

public partial class LibraryMinigamePanel
{
    private const int BookCount = 45;
    private const int BookColorCount = 5;
    private const int BookPatternCount = 5;
    private const int PileLayerCount = 2;
    private const float FloorHeightRatio = -0.34f;
    private const float PileWidthRatio = 0.72f;
    private const float PileLayerStaggerRatio = 0.45f;
    private const float PileHorizontalJitterRatio = 0.12f;
    private const float PileLayerSpacing = 24f;
    private const float PileVerticalJitter = 10f;
    private const float LyingBookChance = 0.82f;

    private static readonly Vector2 FloorBookSize = new(48f, 92f);
    private static readonly Vector2 ShelvedBookSize = new(38f, 82f);

    [SerializeField] private UiImage _bookPrefab;

    private sealed class Book
    {
        public int ColorId;
        public int PatternId;
        public int Volume;
        public bool IsPlaced;
        public bool IsMoving;
        public Vector2 FloorPosition;
        public Vector2 PickupStartPosition;
        public UiImage Image;
        public RectTransform RectTransform;
    }

    private readonly List<Book> _books = new(BookCount);

    private static int[] CreateShuffledBookIndices()
    {
        var bookIndices = new int[BookCount];
        for (var index = 0; index < bookIndices.Length; index++)
        {
            bookIndices[index] = index;
        }

        for (var index = bookIndices.Length - 1; index > 0; index--)
        {
            var swapIndex = Random.Range(0, index + 1);
            (bookIndices[index], bookIndices[swapIndex]) = (bookIndices[swapIndex], bookIndices[index]);
        }

        return bookIndices;
    }

    private void CreateBooks()
    {
        var bookFont = ResourceManager.Instance.LoadScriptableObject<TMP_FontAsset>("npsfont_regular sdf");
        if (bookFont == null) bookFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        var patternSprites = new Sprite[BookPatternCount];
        for (var index = 0; index < patternSprites.Length; index++)
        {
            patternSprites[index] = ResourceManager.Instance.LoadLibraryMinigameSprite(isVolume: false, number: index + 1);
        }
        var volumeSprites = new Sprite[BooksPerShelf];
        for (var index = 0; index < volumeSprites.Length; index++)
        {
            volumeSprites[index] = ResourceManager.Instance.LoadLibraryMinigameSprite(isVolume: true, number: index + 1);
        }

        var floorY = _layoutHeight * FloorHeightRatio;
        var pileWidth = Mathf.Min(_layoutWidth * PileWidthRatio, BookCount * 40f);
        var booksPerLayer = Mathf.CeilToInt(BookCount / (float)PileLayerCount);
        var pileStep = pileWidth / Mathf.Max(booksPerLayer - 1, 1);
        var horizontalJitter = pileStep * PileHorizontalJitterRatio;
        var bookIndices = CreateShuffledBookIndices();

        for (var index = 0; index < BookCount; index++)
        {
            var pileLayer = index % PileLayerCount;
            var pileColumn = index / PileLayerCount;
            var itemsInLayer = Mathf.CeilToInt((BookCount - pileLayer) / (float)PileLayerCount);
            var centeredColumn = pileColumn - (itemsInLayer - 1) * 0.5f;
            var layerStagger = pileLayer == 0 ? 0f : pileStep * PileLayerStaggerRatio;
            var x = centeredColumn * pileStep + layerStagger + Random.Range(-horizontalJitter, horizontalJitter);
            var y = floorY + pileLayer * PileLayerSpacing + Random.Range(-PileVerticalJitter, PileVerticalJitter);
            var floorPosition = new Vector2(x, y);
            var bookIndex = bookIndices[index];
            // 색마다 I~III 한 세트를 만들고, 나머지 책에는 색과 문양을 조합합니다.
            var regularBookIndex = bookIndex - BookColorCount * BooksPerShelf;
            var isNumberedBook = regularBookIndex < 0;
            var colorId = isNumberedBook ? bookIndex / BooksPerShelf : regularBookIndex % BookColorCount;
            var patternId = (isNumberedBook ? colorId : colorId + regularBookIndex / BookColorCount) % BookPatternCount;
            var volume = isNumberedBook ? bookIndex % BooksPerShelf + 1 : 0;
            var floorRotation = Random.value < LyingBookChance
                ? Random.Range(55f, 125f) * (Random.value < 0.5f ? -1f : 1f)
                : Random.Range(-25f, 25f);

            var coverSprite = patternSprites[patternId];
            var image = Instantiate(_bookPrefab, _boardRoot, false);
            image.name = $"LibraryBook_{index + 1}";
            if (coverSprite != null) image.SetSprite(coverSprite);
            image.color = GetBookColor(colorId);
            image.raycastTarget = true;
            var rectTransform = image.rectTransform;
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = FloorBookSize;
            rectTransform.anchoredPosition = floorPosition + new Vector2(
                Random.Range(-32f, 32f), Random.Range(100f, 160f));
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f));

            if (coverSprite == null)
            {
                CreateBookMark(rectTransform, (patternId + 1).ToString("D2"), bookFont, isNumberedBook ? 0.67f : 0.5f);
            }
            if (isNumberedBook)
            {
                var volumeSprite = volumeSprites[volume - 1];
                if (volumeSprite == null)
                {
                    CreateBookMark(rectTransform, new string('I', volume), bookFont, 0.25f);
                }
                else
                {
                    var badge = new GameObject("Volume", typeof(RectTransform), typeof(UiImage)).GetComponent<UiImage>();
                    PositionBookMark(badge.rectTransform, rectTransform, 0.25f);
                    badge.SetSprite(volumeSprite);
                    badge.preserveAspect = true;
                    badge.raycastTarget = false;
                }
            }

            var book = new Book
            {
                ColorId = colorId,
                PatternId = patternId,
                Volume = volume,
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

    private static void CreateBookMark(RectTransform book, string mark, TMP_FontAsset font, float anchorY)
    {
        var label = new GameObject("BookMark", typeof(RectTransform), typeof(UiText)).GetComponent<UiText>();
        PositionBookMark(label.rectTransform, book, anchorY);
        label.font = font;
        label.text = mark;
        label.fontSize = 20f;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
    }

    private static void PositionBookMark(RectTransform mark, RectTransform book, float anchorY)
    {
        mark.SetParent(book, false);
        mark.anchorMin = mark.anchorMax = new Vector2(0.5f, anchorY);
        mark.anchoredPosition = Vector2.zero;
        mark.sizeDelta = new Vector2(ShelvedBookSize.x, ShelvedBookSize.y * 0.4f);
    }

    private static Color GetBookColor(int colorId)
    {
        return colorId switch
        {
            0 => new Color(0.88f, 0.28f, 0.31f),
            1 => new Color(0.95f, 0.66f, 0.20f),
            2 => new Color(0.26f, 0.65f, 0.82f),
            3 => new Color(0.61f, 0.42f, 0.80f),
            _ => new Color(0.38f, 0.68f, 0.38f),
        };
    }
}
