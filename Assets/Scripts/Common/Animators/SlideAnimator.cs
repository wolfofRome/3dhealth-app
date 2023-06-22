using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

namespace Assets.Scripts.Common.Animators
{
    public class SlideAnimator : MonoBehaviour
    {
        [Serializable]
        public class PositionChangeEvent : UnityEvent<Vector3> {}
        [SerializeField]
        private PositionChangeEvent _onPositionChanged = new PositionChangeEvent();
        public PositionChangeEvent onPositionChanged {
            get {
                return _onPositionChanged;
            }
        }
        
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

        [SerializeField]
        private AnimationCurve _animCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve animCurve {
            get {
                return _animCurve;
            }
            set {
                _animCurve = value;
            }
        }


        [SerializeField]
        private Vector2 _inPosition = default;
        public Vector2 inPosition {
            get {
                return _inPosition;
            }
            set {
                _inPosition = value;
            }
        }

        [SerializeField]
        private Vector2 _outPosition = default;
        public Vector2 outPosition {
            get {
                return _outPosition;
            }
            set {
                _outPosition = value;
            }
        }

        [SerializeField]
        public float _duration = 0.2f;
        public float duration {
            get {
                return _duration;
            }
            set {
                _duration = value;
            }
        }

        public void SlideIn()
        {
            StartCoroutine(StartSlidePanel(true));
        }

        public void SlideOut()
        {
            StartCoroutine(StartSlidePanel(false));
        }

        private IEnumerator StartSlidePanel(bool isSlideIn)
        {
            RectTransform rt = GetComponent<RectTransform>();
            float startTime = Time.time;
            Vector2 startPos = rt.anchoredPosition;
            Vector2 moveDistance;

            if (isSlideIn)
            {
                moveDistance = (inPosition - startPos);
            }
            else {
                moveDistance = (outPosition - startPos);
            }

            onStartAnimation.Invoke(gameObject);

            while ((Time.time - startTime) < duration)
            {
                rt.anchoredPosition = startPos + moveDistance * animCurve.Evaluate((Time.time - startTime) / duration);
                onPositionChanged.Invoke(rt.anchoredPosition);
                yield return 0;
            }
            rt.anchoredPosition = startPos + moveDistance;
            onPositionChanged.Invoke(rt.anchoredPosition);

            onFinishAnimation.Invoke(gameObject);
        }
    }
}