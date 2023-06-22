using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Common.InputEvent
{
    public abstract class BaseTouchEventListener : MonoBehaviour, ITouchEventListener
    {
        protected virtual void Awake()
        {
        }
        
        protected virtual void Start()
        {
        }
        
        protected virtual void Update()
        {
        }

        public abstract void OnBeginDrag(int pointerId, TouchEventData data);

        public abstract void OnDrag(int pointerId, TouchEventData data);

        public abstract void OnEndDrag(int pointerId, TouchEventData data);

        public abstract void OnPointerDoubleClick(PointerEventData data);

        public abstract void OnPointerSingleClick(PointerEventData data);
    }
}
