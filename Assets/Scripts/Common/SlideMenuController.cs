using UnityEngine;
using Assets.Scripts.Common.Animators;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(SlideAnimator))]
    public class SlideMenuController : BaseMenuController
    {
        private SlideAnimator _slideAnimator;
        public SlideAnimator slideAnimator {
            get {
                return _slideAnimator = _slideAnimator ?? GetComponent<SlideAnimator>();
            }
        }

        [SerializeField]
        private bool isOutPosition = true;

        protected override void Start()
        {
            RectTransform rt = GetComponent<RectTransform>();
            slideAnimator.inPosition = rt.anchoredPosition - (isOutPosition ? rt.offsetMin : Vector2.zero);
            slideAnimator.outPosition = rt.anchoredPosition - (isOutPosition ? Vector2.zero : rt.offsetMin);
        }

        public override void Show()
        {
            base.Show();
            slideAnimator.SlideIn();
        }

        public override void Hide()
        {
            base.Hide();
            slideAnimator.SlideOut();
        }
    }
}