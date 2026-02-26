using System.Collections.Generic;
using UnityEngine;

public abstract class MovableObject : MapObject
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _spriteTransform;

    [Header("[Move]")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private CircleCollider2D _collider2D;
    [SerializeField] private ContactFilter2D _contactFilter;

    public CircleCollider2D Collider => _collider2D;
    public Vector2 MoveDirection { get; protected set; }
    protected MapConfig Config;

    // move
    private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[8];
    private Vector2 _forceMoveDirection;

    // sprite animation
    private bool _isFacingFront;
    private float _spriteFrameTime;
    private int _spriteIndex;
    private readonly List<Sprite> _frontSprites = new();
    private readonly List<Sprite> _backSprites = new();
    private const string _frontSpriteFormat = "{0}_front_{1:D2}";
    private const string _backSpriteFormat = "{0}_back_{1:D2}";

    #region UnityEvent

    protected override void Update()
    {
        base.Update();
        if (!GlobalManager.Instance.CanPlayerMove()) return;
        UpdateSprite(Time.deltaTime);
        UpdateZPosition();
    }

    protected void FixedUpdate()
    {
        if (!GlobalManager.Instance.CanPlayerMove()) return;
        UpdateMove(Time.fixedDeltaTime);
    }

    #endregion

    public void SetForceMoveDirection(Vector2 direction)
    {
        _forceMoveDirection = direction;
    }

    protected void InitPositionAndScale(Vector2 initPosition, Vector2 spritePosition, float spriteScale, float colliderScale)
    {
        _isFacingFront = true;
        if (Config == null)
        {
            Config = ResourceManager.Instance.LoadMapConfig();
        }
        _spriteTransform.localPosition = spritePosition;
        _spriteTransform.SetLocalScale(spriteScale);
        Transform.position = initPosition;
        Transform.SetLocalScale(colliderScale);
        InitMove();
        InitSprite();
    }

    protected abstract int GetCharacterDataId();

    private void InitMove()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        MoveDirection = Vector2.zero;
    }

    private void UpdateMove(float dt)
    {
        var moveDirection = _forceMoveDirection != Vector2.zero ? _forceMoveDirection : MoveDirection;
        moveDirection = moveDirection.normalized;
        if (moveDirection == Vector2.zero) return;

        var deltaPosition = moveDirection * (dt * Config.MapCharacterSpeed);
        for (var i = 0; i < Config.CollisionResolveCount; i++)
        {
            var deltaDistance = deltaPosition.magnitude;
            var collisionCount = _rigidbody2D.Cast(deltaPosition.normalized, _contactFilter, _hitBuffer, deltaDistance + Config.CollisionMargin);
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
                var collisionRatio = Mathf.Clamp01((hit.distance - Config.CollisionMargin) / deltaDistance);
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
            slide += normal * Config.SlidePush;

            if (slide.magnitude <= float.Epsilon) return;

            deltaPosition = slide;
        }
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
            _spriteRenderer.sprite = _frontSprites[_spriteIndex];
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
            _spriteRenderer.flipX = false;
        }
        else if (moveDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }

        if (moveDirection == Vector2.zero)
        {
            _spriteFrameTime = 0;
            _spriteIndex = 0;
        }
        else
        {
            _spriteFrameTime += dt;
            if (_spriteFrameTime > Config.FrameDuration)
            {
                _spriteFrameTime -= Config.FrameDuration;
                _spriteIndex += 1;
                _spriteIndex %= (_isFacingFront ? _frontSprites.Count : _backSprites.Count);
            }
        }

        _spriteRenderer.sprite = _isFacingFront ? _frontSprites[_spriteIndex] : _backSprites[_spriteIndex];
    }

    private void UpdateZPosition()
    {
        var pos = Transform.localPosition;
        if (Mathf.Approximately(pos.z, pos.y)) return;
        Transform.localPosition = new Vector3(pos.x, pos.y, pos.y);
    }
}