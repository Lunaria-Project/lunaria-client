using UnityEngine;

public class NormalMap : MonoBehaviour
{
    [SerializeField] private Transform _playerInitPosition;

    public Transform PlayerInitPosition => _playerInitPosition;
}
