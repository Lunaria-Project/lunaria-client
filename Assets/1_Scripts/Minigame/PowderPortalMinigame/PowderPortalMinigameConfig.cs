using UnityEngine;

[CreateAssetMenu(menuName = "Lunaria/Minigame/PowderPortalConfig")]
public class PowderPortalMinigameConfig : ScriptableObject
{
    [SerializeField] private float _spawnIntervalSeconds = 1.5f;
    [SerializeField] private int _visibleSlotCount = 5;
    [SerializeField] private int _correctScore = 10;
    [SerializeField] private int _wrongScore = -5;
    [SerializeField] private int _advanceAnimDurationMillis = 200;
    [SerializeField] private float _sendAnimDuration = 0.3f;
    [SerializeField] private float _sendAnimDuration2 = 0.5f;

    [Header("Sprites")]
    [SerializeField] private Sprite _leftSprite;
    [SerializeField] private Sprite _rightSprite;
    [SerializeField] private Sprite _spaceSprite;

    [Header("Direction Rates")]
    [SerializeField] private int _leftRate = 33;
    [SerializeField] private int _rightRate = 33;
    [SerializeField] private int _spaceRate = 34;

    public int VisibleSlotCount => _visibleSlotCount;
    public int CorrectScore => _correctScore;
    public int WrongScore => _wrongScore;
    public int AdvanceAnimDurationMillis => _advanceAnimDurationMillis;
    public float SendAnimDuration => _sendAnimDuration;
    public float SendAnimDuration2 => _sendAnimDuration2;

    public PowderPortalDirection GetRandomDirection()
    {
        var total = _leftRate + _rightRate + _spaceRate;
        var random = Random.Range(0, total);
        if (random < _leftRate) return PowderPortalDirection.Left;
        random -= _leftRate;
        if (random < _rightRate) return PowderPortalDirection.Right;
        return PowderPortalDirection.Space;
    }

    public int GetScore(bool isCorrect)
    {
        return isCorrect ? _correctScore : _wrongScore;
    }

    public Sprite GetSprite(PowderPortalDirection direction)
    {
        return direction switch
        {
            PowderPortalDirection.Left  => _leftSprite,
            PowderPortalDirection.Right => _rightSprite,
            _                           => _spaceSprite,
        };
    }
}
