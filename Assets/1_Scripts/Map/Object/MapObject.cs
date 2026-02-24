using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    
    protected Transform Transform => _transform;

    protected virtual void Start()
    {
        var pos = _transform.localPosition;
        if (Mathf.Approximately(pos.z, pos.y)) return;
        _transform.localPosition = new Vector3(pos.x, pos.y, pos.y);
    }
}