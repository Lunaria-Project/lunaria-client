using UnityEngine;

public class OldMapObject : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Transform _spriteTransform;

    protected Transform Transform => _transform;
    protected SpriteRenderer SpriteRenderer => _sprite;

    protected virtual void Start() { }

    protected virtual void LateUpdate()
    {
        //var pos = _spriteTransform.localPosition;
        //if (Mathf.Approximately(pos.z, pos.y)) return;
        //_spriteTransform.localPosition = new Vector3(pos.x, pos.y, pos.y);
    }

    protected void InitSpritePosition(Vector2 position, float scale)
    {
        _spriteTransform.localPosition = new Vector3(position.x, position.y, position.y);
        _spriteTransform.localScale = new Vector3(scale, scale, 1);
    }
}