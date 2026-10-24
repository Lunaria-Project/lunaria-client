using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lunaria
{
    [DisallowMultipleComponent]
    public sealed class ArtifactCraftPhaseAnimator : MonoBehaviour
    {
        [Serializable]
        public sealed class Step
        {
            public string Title;
            [TextArea] public string Description;
        }

        [Header("Reusable phase prefab (optional)")]
        [SerializeField] private RectTransform _itemPrefab;
        [SerializeField] private RectTransform[] _itemRows;
        [SerializeField] private Vector2 _prefabOrigin = new Vector2(240f, -230f);

        [Header("Legacy visual templates")]
        [SerializeField] private RectTransform _currentTemplate;
        [SerializeField] private RectTransform _normalTemplate;
        [SerializeField] private GameObject[] _staticRows;
        [SerializeField] private Step[] _steps;
        [SerializeField] private int _initialStep = 1;
        [SerializeField, Min(0f)] private float _duration = 0.45f;
        [SerializeField, Min(1f)] private float _spacing = 86f;
        [SerializeField, Range(0f, 1f)] private float _nearAlpha = 0.62f;
        [SerializeField, Range(0f, 1f)] private float _farAlpha = 0.18f;
        [SerializeField] private UnityEvent<int> _onStepChanged = new UnityEvent<int>();

        private sealed class Card
        {
            public RectTransform Rect;
            public CanvasGroup Group;
            public CanvasGroup Current;
            public CanvasGroup Normal;
            public LayoutSwitcher Switcher;
            public string LayoutKey;
        }

        private readonly List<Card> _cards = new List<Card>();
        private RectTransform _cardsRoot;
        private Coroutine _transition;
        private Vector2 _origin;
        private float _displayedStep;
        private int _step;
        private bool _hasStep;

        public int CurrentStep => _hasStep ? _step : _initialStep;
        public int StepCount => _steps == null ? 0 : _steps.Length;
        public bool IsAnimating => _transition != null;
        public UnityEvent<int> OnStepChanged => _onStepChanged;

        private void OnEnable()
        {
            if (!_hasStep) { _step = ClampStep(_initialStep); _hasStep = true; }
            if (!BuildCards()) return;
            _displayedStep = _step;
            Render();
        }

        private void OnDisable()
        {
            if (_transition != null) StopCoroutine(_transition);
            _transition = null;
        }

        public void SetStep(int index) => ChangeStep(index, true);
        public void SetStepImmediate(int index) => ChangeStep(index, false);
        public void NextStep() => SetStep(CurrentStep + 1);
        public void PreviousStep() => SetStep(CurrentStep - 1);
        public void ResetSteps() => SetStepImmediate(0);

        // The crafting lever reports 0..1. Moving it back does not undo a completed stage.
        public void OnLeverValueChanged(float value)
        {
            if (CurrentStep == 1 && value >= 0.98f) SetStep(2);
        }

        private int ClampStep(int index) => Mathf.Clamp(index, 0, Mathf.Max(0, StepCount - 1));

        private void ChangeStep(int index, bool animate)
        {
            if (StepCount == 0) return;
            index = ClampStep(index);
            bool changed = !_hasStep || index != _step;
            if (!changed && animate) return;
            if (_transition != null) StopCoroutine(_transition);
            _transition = null;
            _hasStep = true;
            _step = index;
            if (isActiveAndEnabled && BuildCards())
            {
                if (animate && Application.isPlaying && _duration > 0f &&
                    !Mathf.Approximately(_displayedStep, _step))
                    _transition = StartCoroutine(Animate(_displayedStep, _step));
                else { _displayedStep = _step; Render(); }
            }
            else _displayedStep = _step;
            if (changed) _onStepChanged.Invoke(_step);
        }

        private IEnumerator Animate(float from, float to)
        {
            float elapsed = 0f;
            while (elapsed < _duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / _duration);
                _displayedStep = Mathf.Lerp(from, to, t * t * (3f - 2f * t));
                Render();
                yield return null;
            }
            _displayedStep = to;
            Render();
            _transition = null;
        }

        private bool BuildCards()
        {
            if (_cardsRoot != null) return true;
            if (_itemPrefab != null) return BuildPrefabCards();
            if (_currentTemplate == null || _normalTemplate == null || StepCount == 0) return false;
            _origin = _currentTemplate.anchoredPosition;
            _cardsRoot = new GameObject("Animated_Phases", typeof(RectTransform)).GetComponent<RectTransform>();
            _cardsRoot.SetParent(transform, false);
            _cardsRoot.anchorMin = Vector2.zero;
            _cardsRoot.anchorMax = Vector2.one;
            _cardsRoot.sizeDelta = Vector2.zero;
            _cardsRoot.gameObject.layer = gameObject.layer;
            for (int i = 0; i < StepCount; i++)
            {
                var rect = new GameObject("Phase_" + i, typeof(RectTransform), typeof(CanvasGroup)).GetComponent<RectTransform>();
                rect.SetParent(_cardsRoot, false);
                rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
                rect.sizeDelta = _currentTemplate.sizeDelta;
                rect.gameObject.layer = gameObject.layer;
                var current = CloneVisual(_currentTemplate, rect, "Current_Visual", _steps[i]);
                var normal = CloneVisual(_normalTemplate, rect, "Normal_Visual", _steps[i]);
                var card = new Card { Rect = rect, Group = rect.GetComponent<CanvasGroup>(), Current = current, Normal = normal };
                card.Group.blocksRaycasts = false;
                card.Group.interactable = false;
                _cards.Add(card);
            }
            if (_staticRows != null)
                foreach (var row in _staticRows) if (row != null) row.SetActive(false);
            return true;
        }

        private bool BuildPrefabCards()
        {
            if (StepCount == 0) return false;
            _origin = _prefabOrigin;
            _cardsRoot = new GameObject("Animated_Phases", typeof(RectTransform)).GetComponent<RectTransform>();
            _cardsRoot.SetParent(transform, false);
            _cardsRoot.anchorMin = Vector2.zero;
            _cardsRoot.anchorMax = Vector2.one;
            _cardsRoot.sizeDelta = Vector2.zero;
            _cardsRoot.gameObject.layer = gameObject.layer;
            var canvas = GetComponentInParent<Canvas>();
            float referencePixelsPerUnit = canvas != null ? canvas.referencePixelsPerUnit : 100f;
            for (int i = 0; i < StepCount; i++)
            {
                RectTransform row = _itemRows != null && i < _itemRows.Length ? _itemRows[i] : null;
                if (row == null) row = Instantiate(_itemPrefab, _cardsRoot, false);
                else row.SetParent(_cardsRoot, false);
                row.name = "Phase_" + i;
                row.anchorMin = row.anchorMax = new Vector2(0f, 1f);
                row.localScale = Vector3.one;
                foreach (var text in row.GetComponentsInChildren<Text>(true))
                {
                    if (text.name == "txt_phase_title") text.text = _steps[i].Title;
                    if (text.name == "txt_phase_description") text.text = _steps[i].Description;
                }
                foreach (var image in row.GetComponentsInChildren<Image>(true))
                    if (image.type == Image.Type.Sliced && image.sprite != null)
                        image.pixelsPerUnitMultiplier = referencePixelsPerUnit / Mathf.Max(0.01f, image.sprite.pixelsPerUnit);
                var group = row.GetComponent<CanvasGroup>();
                if (group == null) group = row.gameObject.AddComponent<CanvasGroup>();
                group.blocksRaycasts = false;
                group.interactable = false;
                _cards.Add(new Card { Rect = row, Group = group, Switcher = row.GetComponent<LayoutSwitcher>() });
                row.gameObject.SetActive(true);
            }
            if (_itemRows != null)
                for (int i = StepCount; i < _itemRows.Length; i++)
                    if (_itemRows[i] != null) _itemRows[i].gameObject.SetActive(false);
            return true;
        }

        private static CanvasGroup CloneVisual(RectTransform template, Transform parent, string name, Step step)
        {
            var visual = Instantiate(template, parent, false);
            visual.name = name;
            visual.anchorMin = visual.anchorMax = new Vector2(0.5f, 0.5f);
            visual.anchoredPosition = Vector2.zero;
            visual.localScale = Vector3.one;
            foreach (var text in visual.GetComponentsInChildren<Text>(true))
            {
                if (text.name == "txt_phase_title") text.text = step.Title;
                if (text.name == "txt_phase_description") text.text = step.Description;
            }
            foreach (var graphic in visual.GetComponentsInChildren<Graphic>(true))
            {
                var color = graphic.color;
                color.a = 1f;
                graphic.color = color;
            }
            visual.gameObject.SetActive(true);
            var group = visual.GetComponent<CanvasGroup>();
            if (group == null) group = visual.gameObject.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            group.interactable = false;
            return group;
        }

        private void Render()
        {
            int front = Mathf.Clamp(_itemPrefab != null ? _step : Mathf.RoundToInt(_displayedStep), 0, _cards.Count - 1);
            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                float offset = i - _displayedStep;
                float distance = Mathf.Abs(offset);
                if (card.Switcher != null)
                {
                    // The prefab owns the appearance and size of each state.
                    // Select by logical progress, while movement continues smoothly from the displayed position.
                    int stepsAway = Mathf.Abs(i - _step);
                    string key = stepsAway == 0 ? "current" : stepsAway == 1 ? "pre" : "pre_pre";
                    if (card.LayoutKey != key)
                    {
                        card.Switcher.SetLayout(key);
                        card.LayoutKey = key;
                    }
                    card.Rect.anchoredPosition = _origin + new Vector2(0f, -offset * _spacing);
                    card.Group.alpha = distance <= 2f ? 1f : 1f - Mathf.Clamp01((distance - 2f) / 0.4f);
                    card.Rect.gameObject.SetActive(card.Group.alpha > 0.001f);
                    continue;
                }
                float current = Mathf.Clamp01(1f - distance);
                float alpha = distance <= 1f ? Mathf.Lerp(1f, _nearAlpha, distance) :
                    distance <= 2f ? Mathf.Lerp(_nearAlpha, _farAlpha, distance - 1f) :
                    Mathf.Lerp(_farAlpha, 0f, Mathf.Clamp01((distance - 2f) / 0.4f));
                card.Rect.anchoredPosition = _origin + new Vector2(30f * (1f - current), -offset * _spacing);
                card.Group.alpha = alpha;
                card.Current.alpha = current;
                card.Normal.alpha = 1f - current;
                card.Rect.gameObject.SetActive(alpha > 0.001f);
            }
            if (front >= 0 && front < _cards.Count) _cards[front].Rect.SetAsLastSibling();
        }
    }
}
