using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Assets.Scripts.Common.InputEvent;

namespace Assets.Scripts.ThreeDView.BodyAnalysis
{
    public class MarshmallowManTouchHandler : BaseTouchEventListener
    {
        [Serializable]
        public class MarshmallowManTouchHandlerEvent : UnityEvent { }

        [SerializeField]
        private MarshmallowManTouchHandlerEvent _onDrag = default;
        public MarshmallowManTouchHandlerEvent onDrag
        {
            get
            {
                return _onDrag;
            }
        }

        public override void OnBeginDrag(int pointerId, TouchEventData data)
        {
        }

        public override void OnDrag(int pointerId, TouchEventData data)
        {
            _onDrag.Invoke();
        }

        public override void OnEndDrag(int pointerId, TouchEventData data)
        {
        }

        public override void OnPointerDoubleClick(PointerEventData data)
        {
        }

        public override void OnPointerSingleClick(PointerEventData data)
        {
        }
    }
}
