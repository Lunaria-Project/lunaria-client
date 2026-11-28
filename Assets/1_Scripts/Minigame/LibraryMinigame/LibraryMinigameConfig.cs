using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lunaria/Minigame/LibraryConfig")]
public sealed class LibraryMinigameConfig : ScriptableObject
{
    public const int BooksPerShelf = 3;

    public enum CoverColor { Blue, BlueViolet, Green, JadeGreen, Periwinkle, Pink, Purple }
    private const int MaxPatternNumber = 2;

    [System.Serializable]
    public sealed class BookType
    {
        public Sprite Cover;
        public CoverColor Color;
        [Tooltip("0은 무늬 없음, 1과 2는 이미지 파일명의 패턴 번호입니다.")]
        [Range(0, MaxPatternNumber)] public int Pattern;
        [UnityEngine.Serialization.FormerlySerializedAs("Volume")]
        [Range(0, 3)] public int SeriesNumber;
    }

    [SerializeField] private BookType[] _bookTypes = System.Array.Empty<BookType>();

    public List<BookType> CreateDeck(int shelfCount)
    {
        var deck = new List<BookType>(shelfCount * BooksPerShelf);
        var sets = new List<BookType[]>();
        foreach (CoverColor color in System.Enum.GetValues(typeof(CoverColor)))
        {
            var seriesBooks = new BookType[BooksPerShelf];
            foreach (var type in _bookTypes)
                if (type.Cover != null && type.Color == color && type.SeriesNumber > 0)
                    seriesBooks[type.SeriesNumber - 1] = type;
            if (System.Array.TrueForAll(seriesBooks, type => type != null)) sets.Add(seriesBooks);
        }

        for (var pattern = 0; pattern <= MaxPatternNumber; pattern++)
        {
            var covers = System.Array.FindAll(_bookTypes, type =>
                type.Cover != null && type.SeriesNumber == 0 && type.Pattern == pattern);
            if (pattern == 0)
            {
                foreach (var cover in covers)
                    sets.Add(new[] { cover, cover, cover });
                continue;
            }

            for (var index = 0; index < covers.Length; index += BooksPerShelf)
            {
                var set = new BookType[BooksPerShelf];
                for (var slot = 0; slot < set.Length; slot++)
                    set[slot] = covers[(index + slot) % covers.Length];
                sets.Add(set);
            }
        }

        if (sets.Count == 0) throw new System.InvalidOperationException("Library book catalog has no playable sets.");
        for (var cell = 0; cell < shelfCount; cell++)
            deck.AddRange(cell < sets.Count ? sets[cell] : sets[Random.Range(0, sets.Count)]);

        for (var index = deck.Count - 1; index > 0; index--)
        {
            var swap = Random.Range(0, index + 1);
            (deck[index], deck[swap]) = (deck[swap], deck[index]);
        }
        return deck;
    }
}
