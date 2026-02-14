using System;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private SpriteRenderer _sprite;

    protected SpriteRenderer SpriteRenderer => _sprite;

    protected virtual void Start() { }

    public void SetSortingLayer()
    {
        if (string.Equals(_sprite.sortingLayerName, NameContainer.SortingLayer.MapObject, StringComparison.Ordinal))
        {
            _sprite.sortingOrder = -Mathf.RoundToInt(_transform.localPosition.y / 10);
        }
    }
}