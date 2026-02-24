using UnityEngine;

[CreateAssetMenu(menuName = "Lunaria/Minigame/SlimeConfig")]
public class SlimeMinigameConfig : ScriptableObject
{
    [SerializeField] private int _minigameSeconds = 60;
    [SerializeField] private int _slimeShowCount = 3;
    [SerializeField] private float _showDelayMinSeconds = 0.5f;
    [SerializeField] private float _showDelayMaxSeconds = 2.5f;
    [SerializeField] private float _hideDelayMinSeconds = 0.5f;
    [SerializeField] private float _hideDelayMaxSeconds = 2.5f;
    [SerializeField] private float _showDelayMultiplier = 0.2f;
    [SerializeField] private int _toxicSlimeRatePercent = 20;
    [SerializeField] private int _slime1Rate = 100;
    [SerializeField] private int _slime2Rate = 100;
    [SerializeField] private int _slime3Rate = 100;
    [SerializeField] private float _slime1Size = 0.3f;
    [SerializeField] private float _slime2Size = 0.6f;
    [SerializeField] private float _slime3Size = 1f;
    [SerializeField] private float _slime4Size = 1.2f;

    public int MinigameSeconds => _minigameSeconds;
    public int SlimeShowCount => _slimeShowCount;

    public float GetShowDelayRandomSeconds(SlimeType type)
    {
        var multiplier = type switch
        {
            SlimeType.Level1 or SlimeType.ToxicLevel1 => _showDelayMultiplier * 2,
            SlimeType.Level2 or SlimeType.ToxicLevel2 => _showDelayMultiplier * 3,
            SlimeType.Level3 or SlimeType.ToxicLevel3 => _showDelayMultiplier * 4,
            _                                         => 1f,
        };
        return Random.Range(_showDelayMinSeconds, _showDelayMaxSeconds) * multiplier;
    }

    public float GetHideDelayRandomSeconds()
    {
        return Random.Range(_hideDelayMinSeconds, _hideDelayMaxSeconds);
    }

    public SlimeType GetRandomSlime()
    {
        var isToxic = Random.Range(0, 100) < _toxicSlimeRatePercent;
        var totalRate = _slime1Rate + _slime2Rate + _slime3Rate;
        var random = Random.Range(0, totalRate);
        if (random < _slime1Rate) return isToxic ? SlimeType.ToxicLevel1 : SlimeType.Level1;
        random -= _slime1Rate;
        if (random < _slime2Rate) return isToxic ? SlimeType.ToxicLevel2 : SlimeType.Level2;
        random -= _slime2Rate;
        if (random < _slime3Rate) return isToxic ? SlimeType.ToxicLevel3 : SlimeType.Level3;
        LogManager.LogError("Unreachable");
        return SlimeType.Level1;
    }

    public float GetSlimeScale(SlimeType type)
    {
        return type switch
        {
            SlimeType.Level3 or SlimeType.ToxicLevel3 => _slime4Size,
            SlimeType.Level2 or SlimeType.ToxicLevel2 => _slime3Size,
            SlimeType.Level1 or SlimeType.ToxicLevel1 => _slime2Size,
            _                                         => 1f,
        };
    }

    public int GetTouchCount(SlimeType type)
    {
        return type switch
        {
            SlimeType.Level3 => 3,
            SlimeType.Level2 => 2,
            _                => 1,
        };
    }

    public int GetScore(SlimeType type)
    {
        return type switch
        {
            SlimeType.Level1      => 1,
            SlimeType.Level2      => 2,
            SlimeType.Level3      => 3,
            SlimeType.ToxicLevel1 => -1,
            SlimeType.ToxicLevel2 => -2,
            SlimeType.ToxicLevel3 => -3,
            _                     => 0,
        };
    }
}