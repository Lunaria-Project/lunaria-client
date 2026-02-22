using System;
using System.Collections.Generic;
using Lunaria;
using UnityEngine;

public enum CursorType
{
    DefaultEmpty,
    Default,
    BubbleGun,
}

public partial class GlobalManager
{
    [Serializable] public class SerializedCursorData : SerializedDictionary<CursorType, CursorData> { }

    [Serializable] public struct CursorData
    {
        public Sprite[] Sprites;
        public Vector2 Offset;
        public float Scale;
    }

    [Header("Cursor")]
    [SerializeField] private RectTransform _cursorRectTransform;
    [SerializeField] private RectTransform _cursorImageRectTransform;
    [SerializeField] private Image _cursorImage;
    [SerializeField] private float _spriteDuration = 0.2f;
    [SerializeField] private SerializedCursorData _cursorPrefix = new();

    private readonly List<Sprite> _currentCursorSprites = new();
    private int _currentCursorIndex;
    private float _lastSpriteChangeTime;

    public void SetDefaultCursor()
    {
        //TODO(지선): 조건 체크해서 Default or DefaultEmpty
        SetCursor(CursorType.Default);
    }

    public void SetCursor(CursorType cursorType)
    {
        _currentCursorSprites.Clear();
        if (!_cursorPrefix.TryGetValue(cursorType, out var data))
        {
            LogManager.LogErrorPack("GlobalManager.Cursor: prefix를 찾을 수 없습니다.", cursorType);
            return;
        }
        _currentCursorSprites.AddRange(data.Sprites);
        if (_currentCursorSprites.IsNullOrEmpty())
        {
            _cursorRectTransform.SetActive(false);
            return;
        }
        _currentCursorIndex = 0;
        _lastSpriteChangeTime = 0;

        _cursorRectTransform.SetActive(true);
        _cursorImageRectTransform.anchoredPosition = data.Offset;
        _cursorImageRectTransform.SetLocalScale(data.Scale);
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (_currentCursorSprites.IsNullOrEmpty()) return;

        _cursorRectTransform.position = Input.mousePosition;

        if (Time.unscaledTime - _lastSpriteChangeTime < _spriteDuration) return;

        _lastSpriteChangeTime = Time.unscaledTime;

        _currentCursorIndex++;
        if (_currentCursorIndex >= _currentCursorSprites.Count)
        {
            _currentCursorIndex = 0;
        }
        _cursorImage.SetSprite(_currentCursorSprites.GetAt(_currentCursorIndex));
    }

    private void HideUserCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }
}