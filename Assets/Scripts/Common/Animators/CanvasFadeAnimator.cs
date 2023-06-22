using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common.Animators
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasFadeAnimator : MonoBehaviour
    {
        [Serializable]
        public class AnimationStartEvent : UnityEvent<GameObject> { }
        [SerializeField]
        private AnimationStartEvent _onStartAnimation = new AnimationStartEvent();
        public AnimationStartEvent onStartAnimation {
            get {
                return _onStartAnimation;
            }
            set {
                _onStartAnimation = value;
            }
        }


        [Serializable]
        public class AnimationProcessEvent : UnityEvent<GameObject, float> { }
        [SerializeField]
        private AnimationProcessEvent _onProcessAnimation = new AnimationProcessEvent();
        public AnimationProcessEvent onProcessAnimation {
            get {
                return _onProcessAnimation;
            }
            set {
                _onProcessAnimation = value;
            }
        }

        [Serializable]
        public class AnimationFinishEvent : UnityEvent<GameObject> { }
        [SerializeField]
        private AnimationFinishEvent _onFinishAnimation = new AnimationFinishEvent();
        public AnimationFinishEvent onFinishAnimation {
            get {
                return _onFinishAnimation;
            }
            set {
                _onFinishAnimation = value;
            }
        }

        [SerializeField, Range(0f, 1f)]
        private float _minAlpha = 0f;
        public float minAlpha {
            get {
                return _minAlpha;
            }
            set {
                _minAlpha = value;
            }
        }

        [SerializeField, Range(0f, 1f)]
        private float _maxAlpha = 1f;
        public float maxAlpha {
            get {
                return _maxAlpha;
            }
            set {
                _maxAlpha = value;
            }
        }

        [SerializeField]
        private AnimationCurve _animCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        private float _duration = 0.2f;
        public float duration {
            get {
                return _duration;
            }
            set {
                _duration = value;
            }
        }

        public bool isShown
        {
            get
            {
                return 0 < targetCanvasGroup.alpha;
            }
        }

        private Coroutine _coroutine;

        private CanvasGroup _targetCanvasGroup;
        public CanvasGroup targetCanvasGroup {
            get {
                return _targetCanvasGroup = _targetCanvasGroup ?? GetComponent<CanvasGroup>();
            }
        }

        public bool isRunning
        {
            get
            {
                return _coroutine != null;
            }
        }

        void OnValidate()
        {
            maxAlpha = Mathf.Max(maxAlpha, minAlpha);
            minAlpha = Mathf.Min(minAlpha, maxAlpha);
        }

        void Start()
        {
        }

        void Update()
        {
        }

        public void FadeIn()
        {
            if (gameObject.activeInHierarchy)
            {
                _coroutine = StartCoroutine(StartAnimation(minAlpha, maxAlpha));
            }
        }

        public void FadeOut()
        {
            if (gameObject.activeInHierarchy)
            {
                _coroutine = StartCoroutine(StartAnimation(maxAlpha, minAlpha));
            }
        }

        private IEnumerator StartAnimation(float fromAlpha, float toAlpha)
        {
            _onStartAnimation.Invoke(gameObject);

            float startTime = Time.time;

            while ((Time.time - startTime) < duration)
            {
                var progress = _animCurve.Evaluate((Time.time - startTime) / duration);
                targetCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, progress);
                _onProcessAnimation.Invoke(gameObject, progress);
                yield return 0;
            }

            targetCanvasGroup.alpha = toAlpha;
            _onFinishAnimation.Invoke(gameObject);
            
            _coroutine = null;
            yield return 0;
        }
    }
}