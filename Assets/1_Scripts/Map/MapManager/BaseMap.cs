using UnityEngine;

public class BaseMap : MonoBehaviour
{
    [SerializeField] private Transform _playerInitPosition;

    public Transform PlayerInitPosition => _playerInitPosition;
}
