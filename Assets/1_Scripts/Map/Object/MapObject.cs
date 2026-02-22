using System;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Transform _spriteTransform;

    protected Transform Transform => _transform;
    protected SpriteRenderer SpriteRenderer => _sprite;

    protected virtual void Start() { }

    public void SetSortingLayer()
    {
        if (string.Equals(_sprite.sortingLayerName, NameContainer.SortingLayer.MapObject, StringComparison.Ordinal))
        {
            _sprite.sortingOrder = -Mathf.RoundToInt(_transform.localPosition.y / 10);
        }
    }

    public void InitSpritePosition(Vector2 position, float scale)
    {
        _spriteTransform.localPosition = new Vector3(position.x, position.y, position.y);
        _spriteTransform.localScale = new Vector3(scale, scale, 1);
    }
}