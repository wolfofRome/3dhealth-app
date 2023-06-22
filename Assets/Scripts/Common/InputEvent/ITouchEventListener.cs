using UnityEngine.EventSystems;

namespace Assets.Scripts.Common.InputEvent
{
    interface ITouchEventListener : IEventSystemHandler
    {
        void OnBeginDrag(int pointerId, TouchEventData data);
        void OnDrag(int pointerId, TouchEventData data);
        void OnEndDrag(int pointerId, TouchEventData data);

        void OnPointerSingleClick(PointerEventData data);
        void OnPointerDoubleClick(PointerEventData data);
    }
}
