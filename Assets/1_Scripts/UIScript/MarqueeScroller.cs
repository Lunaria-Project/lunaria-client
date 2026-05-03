using UnityEngine;
using System.Collections;
using TMPro;

public class MarqueeScroller : MonoBehaviour
{
    [SerializeField] private RectTransform contentA;
    [SerializeField] private RectTransform contentB;
    [SerializeField] private TMP_Text textA;
    [SerializeField] private TMP_Text textB;
    [SerializeField] private float speed = 80f;

    private float _contentWidth;
    private Vector2 _posA;
    private Vector2 _posB;

    void Start() => StartCoroutine(Init());

    IEnumerator Init()
    {
        yield return null; // Layout 계산 완료 대기

        _contentWidth = contentA.rect.width;
        _posA = Vector2.zero;
        _posB = new Vector2(_contentWidth, 0f);

        contentA.anchoredPosition = _posA;
        contentB.anchoredPosition = _posB;
    }

    void Update()
    {
        if (_contentWidth == 0f) return; // Init 완료 전 방어

        float delta = speed * Time.deltaTime;

        _posA.x -= delta;
        _posB.x -= delta;

        if (_posA.x <= -_contentWidth) _posA.x = _posB.x + _contentWidth;
        if (_posB.x <= -_contentWidth) _posB.x = _posA.x + _contentWidth;

        contentA.anchoredPosition = _posA;
        contentB.anchoredPosition = _posB;
    }

    public void RefreshContent(string newText)
    {
        textA.text = newText;
        textB.text = newText;
        StartCoroutine(RecalculateWidth());
    }

    IEnumerator RecalculateWidth()
    {
        yield return null;
        _contentWidth = contentA.rect.width;
        _posB.x = _posA.x + _contentWidth;
        contentB.anchoredPosition = _posB;
    }
}