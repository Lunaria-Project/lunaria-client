using UnityEngine;

namespace Lunaria
{
    /// <summary>Fits a fixed design-space UI into its immediate RectTransform parent.</summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Lunaria/UI/Fullscreen UI Fitter")]
    public sealed class FullscreenUIFitter : MonoBehaviour
    {
        public enum ScaleMode { FitInside, FillParent }

        [Tooltip("이 UI를 제작한 기준 크기입니다. 자식 UI는 이 좌표계를 유지합니다.")]
        [SerializeField] private Vector2 _referenceResolution = new Vector2(1920f, 1080f);
        [Tooltip("FitInside: 전체 표시, 여백 허용. FillParent: 영역 채우기, 일부가 밖으로 나갈 수 있음.")]
        [SerializeField] private ScaleMode _scaleMode = ScaleMode.FitInside;
        [Tooltip("부모 영역 안에서의 정렬 위치입니다. (0.5, 0.5)는 중앙입니다.")]
        [SerializeField] private Vector2 _alignment = new Vector2(0.5f, 0.5f);

        [Header("Empty area")]
        [Tooltip("FitInside로 생기는 여백만 지정 색상으로 채웁니다.")]
        [SerializeField] private bool _fillEmptyArea;
        [SerializeField] private Color _emptyAreaColor = Color.black;

        private RectTransform _background;
        private UnityEngine.UI.Image[] _bars;
        private RectTransform _rect;
        private DrivenRectTransformTracker _tracker;
        private RectTransform _previousParent;
        private Vector2 _previousSize;
        private bool _dirty = true;

        private void OnEnable()
        {
            _rect = (RectTransform)transform;
            _dirty = true;
            Refresh();
        }

        private void OnDisable()
        {
            _tracker.Clear();
            RemoveBackground();
            _dirty = true;
        }

        private void OnValidate()
        {
            _referenceResolution.x = Mathf.Max(1f, _referenceResolution.x);
            _referenceResolution.y = Mathf.Max(1f, _referenceResolution.y);
            _alignment.x = Mathf.Clamp01(_alignment.x);
            _alignment.y = Mathf.Clamp01(_alignment.y);
            _dirty = true;
        }

        private void OnTransformParentChanged() => _dirty = true;
        private void OnRectTransformDimensionsChange() => _dirty = true;
        private void LateUpdate() => Refresh();

        /// <summary>Call after changing the parent layout when an immediate update is needed.</summary>
        public void Refresh()
        {
            if (!isActiveAndEnabled || !gameObject.scene.IsValid()) return;
            if (_rect == null) _rect = (RectTransform)transform;
            var parent = _rect.parent as RectTransform;
            if (parent == null)
            {
                _tracker.Clear();
                RemoveBackground();
                _previousParent = null;
                return;
            }

            Vector2 size = parent.rect.size;
            if (size.x <= 0f || size.y <= 0f) { RemoveBackground(); return; }
            if (!_dirty && parent == _previousParent && size == _previousSize) return;

            Vector2 designSize = new Vector2(
                Mathf.Max(1f, _referenceResolution.x), Mathf.Max(1f, _referenceResolution.y));
            Vector2 alignment = new Vector2(Mathf.Clamp01(_alignment.x), Mathf.Clamp01(_alignment.y));
            float scale = CalculateScale(size, designSize, _scaleMode);

            _tracker.Clear();
            _tracker.Add(this, _rect, DrivenTransformProperties.Anchors |
                DrivenTransformProperties.Pivot | DrivenTransformProperties.AnchoredPosition3D |
                DrivenTransformProperties.SizeDelta | DrivenTransformProperties.Scale);
            _rect.anchorMin = _rect.anchorMax = alignment;
            _rect.pivot = alignment;
            _rect.sizeDelta = designSize;
            _rect.anchoredPosition3D = Vector3.zero;
            _rect.localScale = new Vector3(scale, scale, 1f);

            UpdateBackground(parent, size, designSize * scale, alignment);

            _previousParent = parent;
            _previousSize = size;
            _dirty = false;
        }

        private void UpdateBackground(RectTransform parent, Vector2 available, Vector2 content, Vector2 alignment)
        {
            if (!_fillEmptyArea || _scaleMode != ScaleMode.FitInside)
            {
                RemoveBackground();
                return;
            }
            if (_background == null)
            {
                var root = new GameObject("UI Fit Empty Area", typeof(RectTransform), typeof(UnityEngine.UI.LayoutElement));
                root.hideFlags = HideFlags.HideAndDontSave;
                root.GetComponent<UnityEngine.UI.LayoutElement>().ignoreLayout = true;
                _background = root.GetComponent<RectTransform>();
                _bars = new UnityEngine.UI.Image[4];
                for (int i = 0; i < _bars.Length; i++)
                {
                    var bar = new GameObject("Bar_" + i, typeof(RectTransform), typeof(Lunaria.Image));
                    bar.hideFlags = HideFlags.HideAndDontSave;
                    bar.transform.SetParent(_background, false);
                    _bars[i] = bar.GetComponent<UnityEngine.UI.Image>();
                    _bars[i].raycastTarget = false;
                }
            }
            _background.SetParent(parent, false);
            _background.gameObject.layer = gameObject.layer;
            _background.anchorMin = Vector2.zero;
            _background.anchorMax = Vector2.one;
            _background.offsetMin = _background.offsetMax = Vector2.zero;
            _background.localScale = Vector3.one;
            _background.localRotation = Quaternion.identity;
            // Place directly behind this view without changing the order of other UI.
            _background.SetSiblingIndex(_rect.GetSiblingIndex());
            if (_background.GetSiblingIndex() > _rect.GetSiblingIndex())
                _background.SetSiblingIndex(_rect.GetSiblingIndex());

            Vector2 gap = new Vector2(Mathf.Max(0f, available.x - content.x), Mathf.Max(0f, available.y - content.y));
            float left = gap.x * alignment.x;
            float bottom = gap.y * alignment.y;
            SetBar(0, Vector2.zero, new Vector2(left, available.y));
            SetBar(1, new Vector2(left + content.x, 0f), new Vector2(gap.x - left, available.y));
            SetBar(2, new Vector2(left, 0f), new Vector2(content.x, bottom));
            SetBar(3, new Vector2(left, bottom + content.y), new Vector2(content.x, gap.y - bottom));
        }

        private void SetBar(int index, Vector2 position, Vector2 size)
        {
            var image = _bars[index];
            image.gameObject.layer = gameObject.layer;
            image.color = _emptyAreaColor;
            var rect = image.rectTransform;
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            image.gameObject.SetActive(size.x > 0.01f && size.y > 0.01f);
        }

        private void RemoveBackground()
        {
            if (_background == null) return;
            _background.gameObject.SetActive(false);
            if (Application.isPlaying) Destroy(_background.gameObject);
            else DestroyImmediate(_background.gameObject);
            _background = null;
            _bars = null;
        }

        public static float CalculateScale(Vector2 available, Vector2 reference, ScaleMode mode)
        {
            if (available.x <= 0f || available.y <= 0f) return 0f;
            float width = available.x / Mathf.Max(1f, reference.x);
            float height = available.y / Mathf.Max(1f, reference.y);
            return mode == ScaleMode.FillParent ? Mathf.Max(width, height) : Mathf.Min(width, height);
        }
    }
}
