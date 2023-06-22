using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Assets.Scripts.FullDisplaySupport;

namespace Assets.Scripts.Common.Animators
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(LayoutElement))]
    public class LayoutElementAnimator : MonoBehaviour
    {
        [Serializable]
        public class LayoutElementAnimationEvent : UnityEvent<LayoutElementAnimator> { }

        [SerializeField]
        private LayoutElementAnimationEvent _onFinishExpandAnimation = default;

        [SerializeField]
        private LayoutElementAnimationEvent _onFinishCollapseAnimation = default;

        // preferred[Width/Height]には、LayoutElementのpreferred[Width/Height]の初期値を設定しておく.
        [Serializable]
        public class AnimationParam {
            [SerializeField]
            private float _preferredWidth = -1;
            public float preferredWidth
            {
                get
                {
                    return _preferredWidth;
                }
            }

            [SerializeField]
            private float _preferredHeight = -1;
            public float preferredHeight
            {
                get
                {
                    return _preferredHeight;
                }
            }
        }
        
        [SerializeField]
        public AnimationParam _expandParam = default;

        [SerializeField]
        private AnimationParam _collapseParam = default;

        [SerializeField]
        private float _duration = 0.2f;

        private bool isExpanded
        {
            get
            {
                return Mathf.Approximately(layoutElement.preferredHeight, _expandParam.preferredHeight) &&
                       Mathf.Approximately(layoutElement.preferredWidth, _expandParam.preferredWidth);
            }
        }

        private bool isCollapsed
        {
            get
            {
                return Mathf.Approximately(layoutElement.preferredHeight, _collapseParam.preferredHeight) &&
                       Mathf.Approximately(layoutElement.preferredWidth, _collapseParam.preferredWidth);
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

        private LayoutElement _layoutElement;
        private LayoutElement layoutElement
        {
            get
            {
                return _layoutElement = _layoutElement ?? GetComponent<LayoutElement>();
            }
        }

        /// <summary>
        /// Expandアニメーションの開始
        /// </summary>
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
                    "oncomplete", "OnFinishExpandAnimationInternal",
                    "oncompletetarget", gameObject));
        }

        /// <summary>
        /// Collapseアニメーションの開始.
        /// </summary>
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
                    "oncomplete", "OnFinishCollapseAnimationInternal",
                    "oncompletetarget", gameObject));
        }

        /// <summary>
        /// アニメーションフレーム単位のレイアウト更新処理.
        /// </summary>
        /// <param name="value"></param>
        public void OnUpdateAnimation(float value)
        {
            if (!Mathf.Approximately(_collapseParam.preferredHeight, _expandParam.preferredHeight))
            {
#if UNITY_IPHONE
                //iPhoneXのノッチ対応
                if (SafeArea.HasSafeArea())
                {
                    layoutElement.preferredHeight = Mathf.Lerp(_collapseParam.preferredHeight, _expandParam.preferredHeight - SafeArea.HeaderHeight, value);

                    LayoutElement headerRect = gameObject.transform.parent.Find("Header").GetComponent<LayoutElement>();

                    RectTransform advicePanelRect = transform.Find("AdvicePanel").GetComponent<RectTransform>();
                    headerRect.preferredHeight = SafeArea.HeaderHeight;
                    advicePanelRect.anchorMax = new Vector2(1, 0.95f);
                    advicePanelRect.offsetMin = Vector2.zero;
                }
                else
                {
                    layoutElement.preferredHeight = Mathf.Lerp(_collapseParam.preferredHeight, _expandParam.preferredHeight, value);
                }
#else
                layoutElement.preferredHeight = Mathf.Lerp(_collapseParam.preferredHeight, _expandParam.preferredHeight, value);
#endif
            }
            if (!Mathf.Approximately(_collapseParam.preferredWidth, _expandParam.preferredWidth))
            {
                layoutElement.preferredWidth = Mathf.Lerp(_collapseParam.preferredWidth, _expandParam.preferredWidth, value);
            }
        }

        /// <summary>
        /// Expandアニメーション完了イベント.
        /// </summary>Expand
        public void OnFinishExpandAnimationInternal()
        {
            _onFinishExpandAnimation.Invoke(this);
        }

        /// <summary>
        /// Collapseアニメーション完了イベント.
        /// </summary>
        public void OnFinishCollapseAnimationInternal()
        {
            _onFinishCollapseAnimation.Invoke(this);
        }
    }
}
