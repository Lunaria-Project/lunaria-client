using UnityEngine;

[CreateAssetMenu(menuName = "Lunaria/Minigame/CottonCandyConfig")]
public class CottonCandyMinigameConfig : ScriptableObject
{
    public const int LayerCount = 3;

    [SerializeField] private float _customerSpawnMinSeconds = 3f;
    [SerializeField] private float _customerSpawnMaxSeconds = 5f;
    [SerializeField] private float _customerMoveSpeed = 500f;
    [SerializeField] private int _orderQuantityMin = 1;
    [SerializeField] private int _orderQuantityMax = 3;
    [SerializeField] private int _scoreRewardMin = 1;
    [SerializeField] private int _scoreRewardMax = 10;
    [SerializeField] private int _rotationCountMin = 1;
    [SerializeField] private int _rotationCountMax = 10;
    [SerializeField] private Color _burntColor = new(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private float _acceptTurns = 0.5f;

    public float CustomerSpawnMinSeconds => _customerSpawnMinSeconds;
    public float CustomerSpawnMaxSeconds => _customerSpawnMaxSeconds;
    public float CustomerMoveSpeed => _customerMoveSpeed;
    public int OrderQuantityMin => _orderQuantityMin;
    public int OrderQuantityMax => _orderQuantityMax;
    public int ScoreRewardMin => _scoreRewardMin;
    public int ScoreRewardMax => _scoreRewardMax;
    public int RotationCountMin => _rotationCountMin;
    public int RotationCountMax => _rotationCountMax;
    public Color BurntColor => _burntColor;
    public float AcceptTurns => _acceptTurns;
}
