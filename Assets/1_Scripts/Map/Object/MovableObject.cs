using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Spine.Unity;
using UnityEngine;

public abstract class MovableObject : MapObject
{
    [SerializeField] private SpriteRenderer _characterSpriteRenderer;
    [SerializeField, CanBeNull] private SkeletonAnimation _skeletonAnimation;
    [SerializeField] private Transform _spriteTransform;

    [Header("[Move]")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private CircleCollider2D _collider2D;
    [SerializeField] private ContactFilter2D _contactFilter;

    public CircleCollider2D Collider => _collider2D;
    public Vector2 MoveDirection { get; protected set; }
    private MapConfig Config;

    // move
    private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[8];
    private Vector2 _forceMoveDirection;

    // auto move
    protected bool IsAutoMoving { get; private set; }
    private Vector2 _autoMoveTargetPosition;
    private Action _onAutoMoveArrived;

    // sprite animation
    private bool _isFacingFront;
    private float _spriteFrameTime;
    private int _spriteIndex;
    private readonly List<Sprite> _frontSprites = new();
    private readonly List<Sprite> _backSprites = new();
    private const string _frontSpriteFormat = "{0}_front_{1:D2}";
    private const string _backSpriteFormat = "{0}_back_{1:D2}";

    // skeleton animation
    private bool _useSkeletonAnimation;
    private const string _walkAnimationName = "animation";
    private const int _walkTrackIndex = 0;

    #region UnityEvent

    protected override void Update()
    {
        base.Update();
        if (!GlobalManager.Instance.CanPlayerMove()) return;
        UpdateAutoMove();
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

    protected override void InitPositionAndScale(Vector2 initPosition, Vector2 spritePosition, float spriteScale, float colliderScale)
    {
        base.InitPositionAndScale(initPosition, spritePosition, spriteScale, colliderScale);

        _isFacingFront = true;
        if (Config == null)
        {
            Config = ResourceManager.Instance.LoadMapConfig();
        }
        _spriteTransform.localPosition = spritePosition;
        _spriteTransform.localScale = Vector3.one * spriteScale;
        Transform.position = initPosition;
        _collider2D.radius = colliderScale;
        InitMove();
        InitSprite();
    }

    protected abstract int GetCharacterDataId();

    #region Move

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

    #endregion

    #region AutoMove

    public void StartAutoMove(Vector2 targetPosition, Action onArrived)
    {
        StopAutoMove();
        IsAutoMoving = true;
        _autoMoveTargetPosition = targetPosition;
        _onAutoMoveArrived = onArrived;
    }

    protected void StopAutoMove()
    {
        if (!IsAutoMoving) return;
        IsAutoMoving = false;
        _autoMoveTargetPosition = Vector2.zero;
        _onAutoMoveArrived = null;
        MoveDirection = Vector2.zero;
    }

    private void UpdateAutoMove()
    {
        if (!IsAutoMoving) return;

        var direction = _autoMoveTargetPosition - (Vector2)Transform.position;
        if (direction.magnitude <= 10f)
        {
            var callback = _onAutoMoveArrived;
            StopAutoMove();
            callback?.Invoke();
            return;
        }
        MoveDirection = direction.normalized;
    }

    #endregion

    private void UpdateZPosition()
    {
        var pos = Transform.localPosition;
        if (Mathf.Approximately(pos.z, pos.y)) return;
        Transform.localPosition = new Vector3(pos.x, pos.y, pos.y);
    }

    #region Sprite

    private void InitSprite()
    {
        var characterData = GameData.Instance.GetCharacterInfoData(GetCharacterDataId());
        var skeletonDataAsset = _skeletonAnimation == null ? null : ResourceManager.Instance.LoadCharacterSkeletonData(characterData.ResourceKey);
        _useSkeletonAnimation = skeletonDataAsset != null;

        if (_skeletonAnimation != null)
        {
            _skeletonAnimation.gameObject.SetActive(_useSkeletonAnimation);
        }
        _characterSpriteRenderer.gameObject.SetActive(!_useSkeletonAnimation);

        if (_useSkeletonAnimation)
        {
            InitSkeletonAnimation(skeletonDataAsset);
            return;
        }

        _spriteIndex = 0;
        _frontSprites.Clear();
        _backSprites.Clear();
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
            _characterSpriteRenderer.sprite = _frontSprites[_spriteIndex];
        }
        _spriteFrameTime = 0;
    }

    private void UpdateSprite(float dt)
    {
        var moveDirection = _forceMoveDirection != Vector2.zero ? _forceMoveDirection : MoveDirection;
        if (_useSkeletonAnimation)
        {
            UpdateSkeletonAnimation(moveDirection);
            return;
        }
        if (_frontSprites.Count == 0 || _backSprites.Count == 0) return;

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
            _characterSpriteRenderer.flipX = false;
        }
        else if (moveDirection.x < 0)
        {
            _characterSpriteRenderer.flipX = true;
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

        _characterSpriteRenderer.sprite = _isFacingFront ? _frontSprites[_spriteIndex] : _backSprites[_spriteIndex];
    }

    #endregion

    #region SkeletonAnimation

    private void InitSkeletonAnimation(SkeletonDataAsset skeletonDataAsset)
    {
        if (_skeletonAnimation == null) return;
        if (_skeletonAnimation.SkeletonDataAsset != skeletonDataAsset)
        {
            _skeletonAnimation.SkeletonDataAsset = skeletonDataAsset;
            _skeletonAnimation.Initialize(true);
        }

        var walkAnimation = skeletonDataAsset.GetSkeletonData(false).FindAnimation(_walkAnimationName);
        if (walkAnimation == null)
        {
            LogManager.LogError($"[MovableObject] {skeletonDataAsset.name}: 스파인 애니메이션을 찾을 수 없습니다. Animation: {_walkAnimationName}");
            return;
        }

        _skeletonAnimation.AnimationState.SetAnimation(_walkTrackIndex, walkAnimation, true);
        _skeletonAnimation.timeScale = 0f;
        _skeletonAnimation.Skeleton.ScaleX = 1f;
    }

    private void UpdateSkeletonAnimation(Vector2 moveDirection)
    {
        if (moveDirection.x > 0)
        {
            _skeletonAnimation.Skeleton.ScaleX = 1f;
        }
        else if (moveDirection.x < 0)
        {
            _skeletonAnimation.Skeleton.ScaleX = -1f;
        }

        var isMoving = moveDirection != Vector2.zero;
        _skeletonAnimation.timeScale = isMoving ? 1f : 0f;
        if (isMoving) return;

        // 정지 시 첫 프레임 포즈로 고정
        var trackEntry = _skeletonAnimation.AnimationState.GetTrack(_walkTrackIndex);
        if (trackEntry == null) return;
        trackEntry.TrackTime = 0f;
    }

    #endregion
}