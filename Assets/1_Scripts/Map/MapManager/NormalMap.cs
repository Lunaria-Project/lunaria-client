using UnityEngine;

public class NormalMap : MonoBehaviour
{
    [SerializeField] private Transform _playerInitPosition;
    [SerializeField] private StaticNpcObject[] _staticNpcObjects;
    [SerializeField] private Transform _boundsMin;
    [SerializeField] private Transform _boundsMax;
    
    public StaticNpcObject[] StaticNpcObjects => _staticNpcObjects;
    public Transform PlayerInitPosition => _playerInitPosition;

    public Bounds MapBounds => new Bounds(
        ((Vector2)_boundsMin.position + (Vector2)_boundsMax.position) / 2f,
        (Vector2)_boundsMax.position - (Vector2)_boundsMin.position
    );

    public bool HasBounds => _boundsMin != null && _boundsMax != null;

    public virtual void Init()
    {
        foreach (var staticNpcObject in _staticNpcObjects)
        {
            staticNpcObject.Init();
        }
    }

    public virtual void Hide()
    {
        foreach (var staticNpcObject in _staticNpcObjects)
        {
            staticNpcObject.Hide();
        }
    }
}
