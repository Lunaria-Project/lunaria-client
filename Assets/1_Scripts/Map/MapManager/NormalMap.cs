using UnityEngine;

public class NormalMap : MonoBehaviour
{
    [SerializeField] private Transform _playerInitPosition;
    [SerializeField] private StaticNpcObject[] _staticNpcObjects;
    
    public StaticNpcObject[] StaticNpcObjects => _staticNpcObjects;
    public Transform PlayerInitPosition => _playerInitPosition;

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
