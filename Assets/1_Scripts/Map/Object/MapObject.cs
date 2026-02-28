using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField, CanBeNull] private Collider2DTrigger _behindTrigger;
    [SerializeField, ShowIf(nameof(_hasBehindTrigger))] private SpriteRenderer _spriteRenderer;

    private bool _hasBehindTrigger => _behindTrigger != null;

    protected Transform Transform => _transform;
    private bool _isPlayerBehind;
    private static readonly Color TransparentColor = new Color(1, 1, 1, 0.5f);
    private const float FadeDuration = 0.3f;

    protected virtual void Start()
    {
        var pos = _transform.localPosition;
        if (Mathf.Approximately(pos.z, pos.y)) return;
        _transform.localPosition = new Vector3(pos.x, pos.y, pos.y);
    }

    private void OnDestroy()
    {
        _spriteRenderer.DOKill();
    }

    protected virtual void Update()
    {
        if (_behindTrigger != null && _isPlayerBehind != _behindTrigger.IsTriggerIn)
        {
            _isPlayerBehind = _behindTrigger.IsTriggerIn;
            _spriteRenderer.DOKill();
            _spriteRenderer.DOColor(_isPlayerBehind ? TransparentColor : Color.white, FadeDuration);
        }
    }

    protected virtual void InitPositionAndScale(Vector2 initPosition, Vector2 spritePosition, float spriteScale, float colliderScale)
    {
    }
}