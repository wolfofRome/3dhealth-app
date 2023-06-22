using Assets.Scripts.Common.Animators;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(Scrollbar))]
    [RequireComponent(typeof(CanvasFadeAnimator))]
    public class ScrollbarAutoFadeoutController : MonoBehaviour
    {
        [SerializeField]
        private float _displayTime = 2;

        private float _timeElapsed = 0;

        private CanvasFadeAnimator _animator;
        private CanvasFadeAnimator animator
        {
            get
            {
                return _animator = _animator ?? GetComponent<CanvasFadeAnimator>();
            }
        }

        private Scrollbar _scrollbar;
        private Scrollbar scrollbar
        {
            get
            {
                return _scrollbar = _scrollbar ?? GetComponent<Scrollbar>();
            }
        }

        void Start () {
            scrollbar.onValueChanged.AddListener(value =>
            {
                if (!animator.isRunning && !Mathf.Approximately(animator.maxAlpha, animator.targetCanvasGroup.alpha))
                {
                    animator.FadeIn();
                }
                _timeElapsed = 0;
            });
	    }

        void Update ()
        {
            _timeElapsed += Time.deltaTime;
            if (_displayTime < _timeElapsed && !animator.isRunning && Mathf.Approximately(animator.maxAlpha, animator.targetCanvasGroup.alpha))
            {
                animator.FadeOut();
            }
        }
    }
}