using System.Collections.Generic;
using UnityEngine;

public abstract class MovableObject : MapObject
{
    [Header("[Move]")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private ContactFilter2D _contactFilter;

    public Vector2 MoveDirection { get; protected set; }

    private Vector2 _forceMoveDirection;

    private MapConfig _config;
    private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[8];
    private bool _isFacingFront;
    private float _spriteFrameTime;
    private int _spriteIndex;
    private readonly List<Sprite> _frontSprites = new();
    private readonly List<Sprite> _backSprites = new();
    private const string _frontSpriteFormat = "{0}_front_{1:D2}";
    private const string _backSpriteFormat = "{0}_back_{1:D2}";

    #region UnityEvent

    protected void Start()
    {
        _isFacingFront = true;
        _config = ResourceManager.Instance.LoadMapConfig();
        InitMove();
        InitSprite();
    }

    protected virtual void Update()
    {
        UpdateSprite(Time.deltaTime);
    }

    protected void FixedUpdate()
    {
        UpdateMove(Time.fixedDeltaTime);
    }

    #endregion

    public void SetForceMoveDirection(Vector2 direction)
    {
        _forceMoveDirection = direction;
    }

    protected abstract int GetCharacterDataId();

    private void InitMove()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        MoveDirection = Vector2.zero;
    }

    private void InitSprite()
    {
        _spriteIndex = 0;
        var characterData = GameData.Instance.GetCharacterInfoData(GetCharacterDataId());
        for (var i = 1; i < 10; i++)
        {
            var resourceKey = string.Format(_frontSpriteFormat, characterData.ResourceKey, i);
            var sprite = ResourceManager.Instance.LoadSprite(resourceKey);
            if (sprite == null) break;
            _frontSprites.Add(sprite);
        }
        for (var i = 1; i < 10; i++)
        {
            var resourceKey = string.Format(_backSpriteFormat, characterData.ResourceKey, i);
            var sprite = ResourceManager.Instance.LoadSprite(resourceKey);
            if (sprite == null) break;
            _backSprites.Add(sprite);
        }
        if (_frontSprites.Count > 0)
        {
            SpriteRenderer.sprite = _frontSprites[_spriteIndex];
        }
        _spriteFrameTime = 0;
    }

    private void UpdateSprite(float dt)
    {
        if (_frontSprites.Count == 0 || _backSprites.Count == 0) return;

        var moveDirection = _forceMoveDirection != Vector2.zero ? _forceMoveDirection : MoveDirection;
        if (moveDirection.y > 0)
        {
            _isFacingFront = false;
        }
        else if (moveDirection.y < 0)
        {
            _isFacingFront = true;
        }

        if (moveDirection.x > 0)
        {
            SpriteRenderer.flipX = false;
        }
        else if (moveDirection.x < 0)
        {
            SpriteRenderer.flipX = true;
        }

        if (moveDirection == Vector2.zero)
        {
            _spriteFrameTime = 0;
            _spriteIndex = 0;
        }
        else
        {
            _spriteFrameTime += dt;
            if (_spriteFrameTime > _config.FrameDuration)
            {
                _spriteFrameTime -= _config.FrameDuration;
                _spriteIndex += 1;
                _spriteIndex %= (_isFacingFront ? _frontSprites.Count : _backSprites.Count);
            }
        }

        SpriteRenderer.sprite = _isFacingFront ? _frontSprites[_spriteIndex] : _backSprites[_spriteIndex];
    }

    private void UpdateMove(float dt)
    {
        var moveDirection = _forceMoveDirection != Vector2.zero ? _forceMoveDirection : MoveDirection;
        if (moveDirection == Vector2.zero) return;

        var deltaPosition = moveDirection * (dt * _config.PlayerSpeed);
        for (var i = 0; i < _config.CollisionResolveCount; i++)
        {
            var deltaDistance = deltaPosition.magnitude;
            var collisionCount = _rigidbody2D.Cast(deltaPosition.normalized, _contactFilter, _hitBuffer, deltaDistance + _config.CollisionMargin);
            if (collisionCount == 0)
            {
                _rigidbody2D.position += deltaPosition;
                return;
            }

            var minCollisionRatio = 1f;
            var nearestHit = _hitBuffer[0];
            for (var j = 0; j < collisionCount; j++)
            {
                var hit = _hitBuffer[j];
                var collisionRatio = Mathf.Clamp01((hit.distance - _config.CollisionMargin) / deltaDistance);
                if (collisionRatio < minCollisionRatio)
                {
                    minCollisionRatio = collisionRatio;
                    nearestHit = hit;
                }
            }

            var newDeltaPosition = deltaPosition * minCollisionRatio;
            _rigidbody2D.position += newDeltaPosition;

            // 남은 이동에서 법선 성분 제거하여 슬라이드
            var normal = nearestHit.normal;
            deltaPosition *= (1 - minCollisionRatio);
            var slide = deltaPosition - Vector2.Dot(deltaPosition, normal) * normal;
            slide += normal * _config.SlidePush;

            if (slide.magnitude <= float.Epsilon) return;

            deltaPosition = slide;
        }
    }
}