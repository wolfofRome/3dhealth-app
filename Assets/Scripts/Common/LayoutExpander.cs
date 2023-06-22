using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(RectTransform))]
    public class LayoutExpander : MonoBehaviour
    {
        [SerializeField]
        private ExpandParam _expandParam = new ExpandParam();
        [SerializeField]
        private float _fontScale = 1.0f;
        [SerializeField]
        private AnimationCurve _animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField]
        private float _animationDuration = 0.1f;

        private ExpandParam _defaultParam = new ExpandParam();

        [Serializable]
        public class ExpandParam
        {
            [SerializeField]
            private Vector2 _anchorMin = new Vector2(0.2f, 0.2f);
            public Vector2 anchorMin {
                get {
                    return _anchorMin;
                }
                set {
                    _anchorMin = value;
                }
            }

            [SerializeField]
            private Vector2 _anchorMax = new Vector2(0.8f, 0.8f);
            public Vector2 anchorMax {
                get {
                    return _anchorMax;
                }
                set {
                    _anchorMax = value;
                }
            }

            [SerializeField]
            private Vector2 _scale = new Vector2(1.2f, 1.2f);
            public Vector2 scale {
                get {
                    return _scale;
                }
                set {
                    _scale = value;
                }
            }

            [SerializeField]
            private Vector2 _pivot = new Vector2(0.5f, 0.5f);
            public Vector2 pivot {
                get {
                    return _pivot;
                }
                set {
                    _pivot = value;
                }
            }

            private Vector2 _anchoredPosition = new Vector2(0f, 0f);
            public Vector2 anchoredPosition {
                get {
                    return _anchoredPosition;
                }
                set {
                    _anchoredPosition = value;
                }
            }
        }

        private Dictionary<Text, int> _childTextFontSizeDic = new Dictionary<Text, int>();

        private RectTransform _rectTransform;
        private RectTransform rectTransform {
            get {
                return _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
            }
        }

        void Awake()
        {
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnChangeValue(bool isExpand)
        {
            if (isExpand)
            {
                SaveLayoutParam();
                StartCoroutine(StartMorphingAnimation(_defaultParam, _expandParam, _fontScale));
            }
            else
            {
                StartCoroutine(StartMorphingAnimation(_expandParam, _defaultParam, 1));
            }
        }

        private void Expand(ExpandParam param, float fontScale)
        {
            rectTransform.anchorMin = param.anchorMin;
            rectTransform.anchorMax = param.anchorMax;
            rectTransform.pivot = param.pivot;
            rectTransform.localScale = param.scale;
            rectTransform.anchoredPosition = param.anchoredPosition;
            rectTransform.SetAsLastSibling();

            foreach (var textPair in _childTextFontSizeDic)
            {
                textPair.Key.fontSize = (int)(textPair.Value * fontScale);
            }
        }

        private void SaveLayoutParam()
        {
            _defaultParam.anchorMin = rectTransform.anchorMin;
            _defaultParam.anchorMax = rectTransform.anchorMax;
            _defaultParam.scale = rectTransform.localScale;
            _defaultParam.anchoredPosition = rectTransform.anchoredPosition;
            _defaultParam.pivot = rectTransform.pivot;

            var textArray = GetComponentsInChildren<Text>();
            foreach (var text in textArray)
            {
                _childTextFontSizeDic[text] = text.fontSize;
            }
        }

        private IEnumerator StartMorphingAnimation(ExpandParam to, ExpandParam from, float fontScale)
        {
            float startTime = Time.time;

            rectTransform.SetAsLastSibling();
            while ((Time.time - startTime) < _animationDuration)
            {
                var progress = _animationCurve.Evaluate((Time.time - startTime) / _animationDuration);
                rectTransform.anchorMin = Vector2.Lerp(to.anchorMin, from.anchorMin, progress);
                rectTransform.anchorMax = Vector2.Lerp(to.anchorMax, from.anchorMax, progress);
                rectTransform.localScale = Vector2.Lerp(to.scale, from.scale, progress);
                rectTransform.anchoredPosition = Vector2.Lerp(to.anchoredPosition, from.anchoredPosition, progress);
                rectTransform.pivot = Vector2.Lerp(to.pivot, from.pivot, progress);
                yield return 0;
            }
            Expand(from, fontScale);
        }
    }
}