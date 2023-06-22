using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.Animators
{
    [RequireComponent(typeof(Image))]
    public class ScreenFadeAnimator : BaseSingletonMonoBehaviour<ScreenFadeAnimator>
    {
        [SerializeField]
        private AnimationCurve _fadeInAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField]
        private AnimationCurve _fadeOutAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField]
        private float _duration = 0.3f;
        public float Duration {
            get {
                return _duration;
            }
            set {
                _duration = value;
            }
        }

        [Serializable]
        public class FadeColor
        {
            [SerializeField]
            public Color From = default;
            [SerializeField]
            public Color To = default;
        }

        [SerializeField]
        private FadeColor _fadeInColor = new FadeColor { From = Color.black, To = Color.clear };
        [SerializeField]
        private FadeColor _fadeOutColor = new FadeColor { From = Color.clear, To = Color.black };

        [SerializeField]
        private Color _defaultFillColor = Color.clear;

        private Action _animationFinished;
        private Color _fromColor;
        private Color _toColor;
        private Color _fillColor;
        public Color FillColor {
            get {
                return _fillColor;
            }
            set {
                _fillColor = value;
            }
        }

        private Rect _fadeArea;
        private Texture2D _texture;
        private Texture2D OverlayTexture {
            get {
                if (_texture == null)
                {
                    _texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    _texture.SetPixel(0, 0, Color.white);
                    _texture.Apply();
                }
                return _texture;
            }
        }
        
        void Awake()
        {
            if (this != Instance)
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(this.gameObject);

            _fillColor = _defaultFillColor;
            // 画面回転時を考慮し、最大サイズで描画
            var maxSize = Math.Max(Screen.width, Screen.height);
            _fadeArea = new Rect(0, 0, maxSize, maxSize);
        }

        void OnGUI()
        {
            if (_fillColor != Color.clear)
            {
                GUI.color = _fillColor;
                GUI.DrawTexture(_fadeArea, OverlayTexture);
            }
        }

        void Start()
        {
        }

        void Update()
        {
        }

        public void FadeIn(Action callback)
        {
            StartCoroutine(StartFadeAnimation(true, callback));
        }

        public void FadeOut(Action callback)
        {
            StartCoroutine(StartFadeAnimation(false, callback));
        }

        private IEnumerator StartFadeAnimation(bool isFadeIn, Action callback)
        {
            AnimationCurve animCurve;
            float startTime = Time.time;

            if (isFadeIn)
            {
                _fromColor = _fadeInColor.From;
                _toColor = _fadeInColor.To;
                animCurve = _fadeInAnimCurve;
            }
            else {
                _fromColor = _fadeOutColor.From;
                _toColor = _fadeOutColor.To;
                animCurve = _fadeOutAnimCurve;
            }
            
            while ((Time.time - startTime) < Duration)
            {
                _fillColor = Color.Lerp(_fromColor, _toColor, animCurve.Evaluate((Time.time - startTime) / Duration));
                yield return 0;
            }
            _fillColor = _toColor;
            yield return 0;

            if (callback != null)
            {
                callback();
            }
            yield return 0;
        }
    }
}