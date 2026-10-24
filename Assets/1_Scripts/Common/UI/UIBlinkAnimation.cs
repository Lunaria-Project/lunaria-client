using System.Collections;
using UnityEngine;

namespace Lunaria
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Lunaria.Image))]
    [AddComponentMenu("Lunaria/UI/UI Blink Animation")]
    public sealed class UIBlinkAnimation : MonoBehaviour
    {
        [Tooltip("기본 눈 뜬 이미지입니다. 비워두면 현재 Image의 Sprite를 사용합니다.")]
        [SerializeField] private Sprite _openEyes;
        [Tooltip("나중에 반쯤 감은 눈 이미지를 넣으세요. 두 눈 감기 이미지가 모두 있어야 재생됩니다.")]
        [SerializeField] private Sprite _halfClosedEyes;
        [Tooltip("나중에 완전히 감은 눈 이미지를 넣으세요.")]
        [SerializeField] private Sprite _closedEyes;
        [SerializeField] private Vector2 _blinkInterval = new Vector2(2f, 5f);
        [SerializeField, Min(0.01f)] private float _halfClosedDuration = 0.06f;
        [SerializeField, Min(0.01f)] private float _closedDuration = 0.1f;
        [SerializeField] private bool _useUnscaledTime = true;

        private Lunaria.Image _image;
        private Coroutine _routine;

        private void OnEnable()
        {
            _image = GetComponent<Lunaria.Image>();
            if (_openEyes == null) _openEyes = _image.sprite;
            RestoreOpenEyes();
            if (_openEyes != null && _halfClosedEyes != null && _closedEyes != null)
                _routine = StartCoroutine(BlinkLoop());
        }

        private void OnDisable()
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = null;
            RestoreOpenEyes();
        }

        private void RestoreOpenEyes()
        {
            if (_image != null && _openEyes != null) _image.sprite = _openEyes;
        }

        private IEnumerator BlinkLoop()
        {
            while (true)
            {
                float minimum = Mathf.Max(0.1f, _blinkInterval.x);
                float maximum = Mathf.Max(minimum, _blinkInterval.y);
                yield return Wait(Random.Range(minimum, maximum));
                _image.sprite = _halfClosedEyes;
                yield return Wait(Mathf.Max(0.01f, _halfClosedDuration));
                _image.sprite = _closedEyes;
                yield return Wait(Mathf.Max(0.01f, _closedDuration));
                RestoreOpenEyes();
            }
        }

        private IEnumerator Wait(float seconds)
        {
            float elapsed = 0f;
            while (elapsed < seconds)
            {
                yield return null;
                elapsed += _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }
        }
    }
}
