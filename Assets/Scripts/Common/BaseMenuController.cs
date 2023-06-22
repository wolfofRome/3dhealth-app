using UnityEngine;

namespace Assets.Scripts.Common
{
    public abstract class BaseMenuController : MonoBehaviour
    {
        private bool _isShown = true;
        public bool isShown
        {
            get
            {
                return _isShown;
            }
            private set
            {
                _isShown = value;
            }
        }

        protected virtual void Awake() {}

        protected virtual void Start() {}
        
        protected virtual void Update() {}
        
        public virtual void Show()
        {
            isShown = true;
        }

        public virtual void Hide()
        {
            isShown = false;
        }
    }
}