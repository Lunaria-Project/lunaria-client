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
    [SerializeField] private int _slime1Rate = 100;
    [SerializeField] private int _slime2Rate = 100;
    [SerializeField] private int _slime3Rate = 100;
    [SerializeField] private int _slime4Rate = 100;
    [SerializeField] private float _slime1Size = 0.3f;
    [SerializeField] private float _slime2Size = 0.6f;
    [SerializeField] private float _slime3Size = 1f;
    [SerializeField] private float _slime4Size = 1.2f;

    public int MinigameSeconds => _minigameSeconds;
    public int SlimeShowCount => _slimeShowCount;

    public float GetShowDelayRandomSeconds(int order)
    {
        return Random.Range(_showDelayMinSeconds, _showDelayMaxSeconds) * order * _showDelayMultiplier;
    }

    public float GetHideDelayRandomSeconds()
    {
        return Random.Range(_hideDelayMinSeconds, _hideDelayMaxSeconds);
    }

    public int GetRandomSlimeOrder()
    {
        var totalRate = _slime1Rate + _slime2Rate + _slime3Rate + _slime4Rate;
        var random = Random.Range(0, totalRate);
        if (random < _slime1Rate) return 1;
        random -= _slime1Rate;
        if (random < _slime2Rate) return 2;
        random -= _slime2Rate;
        if (random < _slime3Rate) return 3;
        random -= _slime3Rate;
        if (random < _slime4Rate) return 4;
        LogManager.LogError("Unreachable");
        return 1;
    }

    public float GetSlimeScale(int order)
    {
        return order switch
        {
            1 => _slime1Size,
            2 => _slime2Size,
            3 => _slime3Size,
            4 => _slime4Size,
            _ => 1f,
        };
    }
}