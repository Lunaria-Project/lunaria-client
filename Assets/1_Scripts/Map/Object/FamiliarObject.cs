using System.Collections.Generic;
using UnityEngine;

public class FamiliarObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public int FamiliarCallItemId { get; private set; }

    private MapConfig _config;

    private bool _isFacingFront;
    private float _spriteFrameTime;
    private int _spriteIndex;
    private readonly List<Sprite> _frontSprites = new();
    private readonly List<Sprite> _backSprites = new();
    private const int _maxSpriteCount = 10;

    public void Show(int familiarCallItemId)
    {
        gameObject.SetActive(true);
        if (FamiliarCallItemId == familiarCallItemId) return;

        FamiliarCallItemId = familiarCallItemId;
        if (_config == null)
        {
            _config = ResourceManager.Instance.LoadMapConfig();
        }

        var familiarCallData = GameData.Instance.GetFamiliarCallData(familiarCallItemId);
        InitSprite(familiarCallData.ResourceKey);
    }

    public void Hide()
    {
        FamiliarCallItemId = 0;
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
        LoadSprites(resourceKey, true, _frontSprites);
        LoadSprites(resourceKey, false, _backSprites);

        if (_frontSprites.Count == 0 || _backSprites.Count == 0)
        {
            LogManager.LogError($"[Familiar] {resourceKey}: 패밀리어 이미지를 찾을 수 없습니다.");
            return;
        }
        _spriteRenderer.sprite = _frontSprites[_spriteIndex];
    }

    private static void LoadSprites(string resourceKey, bool isFront, List<Sprite> sprites)
    {
        sprites.Clear();
        for (var i = 1; i <= _maxSpriteCount; i++)
        {
            var sprite = ResourceManager.Instance.LoadFamiliarSprite(resourceKey, isFront, i);
            if (sprite == null) break;
            sprites.Add(sprite);
        }
    }

    #endregion
}