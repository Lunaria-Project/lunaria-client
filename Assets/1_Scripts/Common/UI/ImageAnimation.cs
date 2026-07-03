using Lunaria;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class ImageAnimation : MonoBehaviour
{
    [SerializeField] private string _resourceKeyPrefix;
    [SerializeField] private int _frameCount;
    [SerializeField] private float _frameInterval = 0.2f;
    [SerializeField] private bool _loop = true;
    [SerializeField] private bool _playOnEnable = true;

    private Image _image;
    private Sprite[] _sprites;
    private float _timer;
    private int _currentFrame;
    private bool _isPlaying;

    protected void Awake()
    {
        _image = GetComponent<Image>();
        LoadSprites();
    }

    protected void OnEnable()
    {
        if (_playOnEnable)
        {
            Play();
        }
    }

    protected void OnDisable()
    {
        _isPlaying = false;
    }

    protected void Update()
    {
        if (!_isPlaying) return;

        _timer += Time.deltaTime;
        if (_timer < _frameInterval) return;

        _timer -= _frameInterval;
        _currentFrame++;
        if (_currentFrame >= _sprites.Length)
        {
            if (!_loop)
            {
                _currentFrame = _sprites.Length - 1;
                _isPlaying = false;
                return;
            }

            _currentFrame = 0;
        }

        _image.SetSprite(_sprites[_currentFrame]);
    }

    private void LoadSprites()
    {
        _sprites = new Sprite[_frameCount];
        for (var i = 0; i < _frameCount; i++)
        {
            var sprite = ResourceManager.Instance.LoadSprite($"{_resourceKeyPrefix}{i}");
            if (sprite == null)
            {
                LogManager.LogError($"[UISpriteAnimation] LoadSprites: 스프라이트를 찾을 수 없습니다. key={_resourceKeyPrefix}{i}");
            }

            _sprites[i] = sprite;
        }
    }

    private void Play()
    {
        if (_sprites == null || _sprites.Length == 0)
        {
            LogManager.LogError($"[UISpriteAnimation] Play: 로드된 스프라이트가 없습니다. prefix={_resourceKeyPrefix}");
            return;
        }

        _timer = 0f;
        _currentFrame = 0;
        _isPlaying = true;
        _image.SetSprite(_sprites[0]);
    }
}
