using Assets.Scripts.Common.Animators;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(SlideAnimator))]
    public class ButtonGroupController : FadeMenuController
    {
        [SerializeField]
        protected List<Button> _buttonList = default;
        protected virtual List<Button> buttonList {
            get {
                return _buttonList;
            }
        }

        private RectTransform _rectTransform;
        protected RectTransform rectTransform {
            get {
                return _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
            }
        }
        
        protected override void Awake()
        {
            buttonList.ForEach(button => {
                menuItemList.Add(button.GetComponent<RectTransform>());
            });
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            initAnimator();
        }
        
        private void initAnimator()
        {
            var animator = GetComponent<SlideAnimator>();
            animator.inPosition = rectTransform.anchoredPosition;
            animator.outPosition = -(rectTransform.anchoredPosition + new Vector2(0, rectTransform.rect.height));
        }
    }
}