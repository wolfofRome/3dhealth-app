using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Common.Animators;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(CanvasFadeAnimator))]
    public class FadeMenuController : BaseMenuController
    {
        [SerializeField]
        private List<RectTransform> _menuItemList = new List<RectTransform>();
        protected List<RectTransform> menuItemList {
            get {
                return _menuItemList;
            }
        }

        [SerializeField]
        private float _menuIconAnimationScale = 0.8f;
        
        private List<Vector2> _menuItemDefaultScaleList = new List<Vector2>();

        protected enum AnimationType : int
        {
            Show = 0,
            Hide = 1
        }

        private struct LayoutParam
        {
            public float menuIconScale { get; set; }
        }

        private LayoutParam _from = new LayoutParam();
        private LayoutParam _to = new LayoutParam();

        private CanvasFadeAnimator _fadeAnimator;
        private CanvasFadeAnimator fadeAnimator {
            get {
                return _fadeAnimator = _fadeAnimator ?? GetComponent<CanvasFadeAnimator>();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            foreach (var item in _menuItemList)
            {
                _menuItemDefaultScaleList.Add(item.localScale);
            }
            fadeAnimator.onStartAnimation.AddListener(OnStartAnimation);
            fadeAnimator.onProcessAnimation.AddListener(OnProcessAnimation);
            fadeAnimator.onFinishAnimation.AddListener(OnFinishAnimation);
        }
        
        public override void Show()
        {
            base.Show();
            SetAnimationParam(AnimationType.Show);
            fadeAnimator.FadeIn();
        }

        public override void Hide()
        {
            base.Hide();
            SetAnimationParam(AnimationType.Hide);
            fadeAnimator.FadeOut();
        }

        public virtual void OnStartAnimation(GameObject go)
        {
            SetLayoutParams(_from.menuIconScale);
        }

        public virtual void OnProcessAnimation(GameObject go, float progress)
        {
            SetLayoutParams(Mathf.Lerp(_from.menuIconScale, _to.menuIconScale, progress));
        }

        public virtual void OnFinishAnimation(GameObject go)
        {
            SetLayoutParams(_to.menuIconScale);
        }

        protected virtual void SetAnimationParam(AnimationType type)
        {
            switch (type)
            {
                case AnimationType.Show:
                    _from.menuIconScale = _menuIconAnimationScale;
                    _to.menuIconScale = 1;
                    break;
                case AnimationType.Hide:
                    _from.menuIconScale = 1;
                    _to.menuIconScale = _menuIconAnimationScale;
                    break;
            }
        }
        
        private void SetLayoutParams(float menuIconScale)
        {
            var itemNum = _menuItemList.Count;
            for (int i = 0; i< itemNum; i++)
            {
                _menuItemList[i].localScale = _menuItemDefaultScaleList[i] * menuIconScale;
            }
        }
    }
}