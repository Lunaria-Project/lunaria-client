using System.Collections.Generic;
using UnityEngine;

public class FamiliarObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public int FamiliarCallId { get; private set; }

    private MapConfig _config;

    private bool _isFacingFront;
    private float _spriteFrameTime;
    private int _spriteIndex;
    private readonly List<Sprite> _frontSprites = new();
    private readonly List<Sprite> _backSprites = new();
    private const string _frontSpriteFormat = "{0}_front{1:D2}";
    private const string _backSpriteFormat = "{0}_back{1:D2}";
    private const int _maxSpriteCount = 10;

    public void Show(int familiarCallId)
    {
        gameObject.SetActive(true);
        if (FamiliarCallId == familiarCallId) return;

        FamiliarCallId = familiarCallId;
        if (_config == null)
        {
            _config = ResourceManager.Instance.LoadMapConfig();
        }

        var familiarCallData = GameData.Instance.GetFamiliarCallData(familiarCallId);
        InitSprite(familiarCallData.ResourceKey);
    }

    public void Hide()
    {
        FamiliarCallId = 0;
        gameObject.SetActive(false);
    }

    #region Animation

    public void UpdateAnimation(Vector2 moveDirection, float dt)
    {
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
            _spriteRenderer.flipX = false;
        }
        else if (moveDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }

        _spriteFrameTime += dt;
        if (_spriteFrameTime > _config.FrameDuration)
        {
            _spriteFrameTime -= _config.FrameDuration;
            _spriteIndex += 1;
        }

        // front/back 프레임 수가 다를 수 있어 보여줄 목록 기준으로 인덱스를 맞춘다.
        var sprites = _isFacingFront ? _frontSprites : _backSprites;
        _spriteIndex %= sprites.Count;
        _spriteRenderer.sprite = sprites[_spriteIndex];
    }

    private void InitSprite(string resourceKey)
    {
        _isFacingFront = true;
        _spriteIndex = 0;
        _spriteFrameTime = 0;
        LoadSprites(_frontSpriteFormat, resourceKey, _frontSprites);
        LoadSprites(_backSpriteFormat, resourceKey, _backSprites);

        if (_frontSprites.Count == 0 || _backSprites.Count == 0)
        {
            LogManager.LogError($"[Familiar] {resourceKey}: 패밀리어 이미지를 찾을 수 없습니다.");
            return;
        }
        _spriteRenderer.sprite = _frontSprites[_spriteIndex];
    }

    private static void LoadSprites(string spriteFormat, string resourceKey, List<Sprite> sprites)
    {
        sprites.Clear();
        for (var i = 1; i <= _maxSpriteCount; i++)
        {
            var sprite = ResourceManager.Instance.LoadSprite(string.Format(spriteFormat, resourceKey, i));
            if (sprite == null) break;
            sprites.Add(sprite);
        }
    }

    #endregion
}