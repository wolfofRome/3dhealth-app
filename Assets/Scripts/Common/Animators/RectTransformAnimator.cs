using System;
using UnityEngine;

namespace Assets.Scripts.Common.Animators
{
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformAnimator : MonoBehaviour
    {
        [Serializable]
        public struct AnimationParam {
            [SerializeField]
            private Vector2 _anchoredPosition;
            public Vector2 anchoredPosition
            {
                get
                {
                    return _anchoredPosition;
                }
            }

            [SerializeField]
            private Vector2 _sizeDelta;
            public Vector2 sizeDelta
            {
                get
                {
                    return _sizeDelta;
                }
            }
        }

        [SerializeField]
        private AnimationParam _expandParam = default;

        [SerializeField]
        private AnimationParam _collapseParam = default;

        [SerializeField]
        private float _duration = 0.2f;

        private bool isExpanded
        {
            get
            {
                return rectTransform.sizeDelta == _expandParam.sizeDelta &&
                       rectTransform.anchoredPosition == _expandParam.anchoredPosition;
            }
        }

        private bool isCollapsed
        {
            get
            {
                return rectTransform.sizeDelta == _collapseParam.sizeDelta &&
                       rectTransform.anchoredPosition == _collapseParam.anchoredPosition;
            }
        }

        private RectTransform _rectTransform;
        private RectTransform rectTransform
        {
            get
            {
                return _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
            }
        }

        public void StartExpandAnimation()
        {
            if (isExpanded) return;

            iTween.ValueTo(gameObject,
                iTween.Hash(
                    "from", 0f,
                    "to", 1f,
                    "time", _duration,
                    "delay", 0f,
                    "onupdate", "OnUpdateAnimation",
                    "onupdatetarget", gameObject,
                    "oncomplete", "OnFinishAnimation",
                    "oncompletetarget", gameObject));
        }

        public void StartCollapseAnimation()
        {
            if (isCollapsed) return;

            iTween.ValueTo(gameObject,
                iTween.Hash(
                    "from", 1f,
                    "to", 0f,
                    "time", _duration,
                    "delay", 0f,
                    "onupdate", "OnUpdateAnimation",
                    "onupdatetarget", gameObject,
                    "oncomplete", "OnFinishAnimation",
                    "oncompletetarget", gameObject));
        }
        
        public void OnUpdateAnimation(float value)
        {
            rectTransform.sizeDelta = Vector2.Lerp(_collapseParam.sizeDelta, _expandParam.sizeDelta, value);
            rectTransform.anchoredPosition = Vector2.Lerp(_collapseParam.anchoredPosition, _expandParam.anchoredPosition, value);
        }

        public void OnFinishAnimation()
        {
        }
    }
}
