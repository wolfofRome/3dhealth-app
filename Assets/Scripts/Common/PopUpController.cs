using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common
{
    public class PopUpController : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _onOpenAnimationStart = new UnityEvent();
        public UnityEvent OnOpenAnimationStart {
            get {
                return _onOpenAnimationStart;
            }
        }

        [SerializeField]
        private UnityEvent _onOpenAnimationFinish = new UnityEvent();
        public UnityEvent OnOpenAnimationFinish {
            get {
                return _onOpenAnimationFinish;
            }
        }

        [SerializeField]
        private UnityEvent _onCloseAnimationStart = new UnityEvent();
        public UnityEvent OnCloseAnimationStart {
            get {
                return _onCloseAnimationStart;
            }
        }

        [SerializeField]
        private UnityEvent _onCloseAnimationFinish = new UnityEvent();
        public UnityEvent OnCloseAnimationFinish {
            get {
                return _onCloseAnimationFinish;
            }
        }

        [SerializeField]
        private float _animationDuration = 0.2f;

        [SerializeField]
        private Vector2 _dispScale = Vector2.one;
        private Vector2 dispScale {
            get {
                return _dispScale;
            }
        }
        
        private RectTransform _rectTransform;
        private RectTransform rectTransform {
            get {
                return _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
            }
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
            StartOpenAnimation();
        }

        protected virtual void Update()
        {

        }

        public virtual void StartOpenAnimation()
        {
            rectTransform.localScale = Vector2.zero;
            iTween.ScaleTo(gameObject,
                iTween.Hash(
                    "x", dispScale.x,
                    "y", dispScale.y,
                    "time", _animationDuration,
                    "delay", 0f,
                    "onstart", "OnStartOpenAnimation",
                    "onstarttarget", gameObject,
                    "oncomplete", "OnFinishOpenAnimation",
                    "oncompletetarget", gameObject));
        }

        public virtual void StartCloseAnimation()
        {
            rectTransform.localScale = dispScale;
            iTween.ScaleTo(gameObject,
                iTween.Hash(
                    "x", 0f,
                    "y", 0f,
                    "time", _animationDuration,
                    "delay", 0f,
                    "onstart", "OnStartCloseAnimation",
                    "onstarttarget", gameObject,
                    "oncomplete", "OnFinishCloseAnimation",
                    "oncompletetarget", gameObject));
        }

        protected virtual void OnStartOpenAnimation()
        {
            OnOpenAnimationStart.Invoke();
        }

        protected virtual void OnFinishOpenAnimation()
        {
            OnOpenAnimationFinish.Invoke();
        }

        protected virtual void OnStartCloseAnimation()
        {
            OnCloseAnimationStart.Invoke();
        }

        protected virtual void OnFinishCloseAnimation()
        {
            OnCloseAnimationFinish.Invoke();
        }
    }
}
