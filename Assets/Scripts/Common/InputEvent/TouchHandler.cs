using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Common.InputEvent
{
    [RequireComponent(typeof(EventTrigger))]
    public class TouchHandler : MonoBehaviour
    {
        [SerializeField]
        private List<BaseTouchEventListener> _listenerList = new List<BaseTouchEventListener>();
        private TouchEventData _eventData = new TouchEventData();

        private int _pointerClickNum = 0;

        protected virtual void Awake()
        {
            EventTrigger trigger = GetComponent<EventTrigger>();
            EventTrigger.Entry entryBeginDrag = new EventTrigger.Entry();
            entryBeginDrag.eventID = EventTriggerType.BeginDrag;
            entryBeginDrag.callback.AddListener((eventData) => { OnBeginDrag(eventData); });
            trigger.triggers.Add(entryBeginDrag);

            EventTrigger.Entry entryEndDrag = new EventTrigger.Entry();
            entryEndDrag.eventID = EventTriggerType.EndDrag;
            entryEndDrag.callback.AddListener((eventData) => { OnEndDrag(eventData); });
            trigger.triggers.Add(entryEndDrag);

            EventTrigger.Entry entryDrag = new EventTrigger.Entry();
            entryDrag.eventID = EventTriggerType.Drag;
            entryDrag.callback.AddListener((eventData) => { OnDrag(eventData); });
            trigger.triggers.Add(entryDrag);
            
            EventTrigger.Entry entryPointerClick = new EventTrigger.Entry();
            entryPointerClick.eventID = EventTriggerType.PointerClick;
            entryPointerClick.callback.AddListener((eventData) => { OnPointerClick(eventData); });
            trigger.triggers.Add(entryPointerClick);
        }

        // Use this for initialization
        protected virtual void Start()
        {
        }

        // Update is called once per frame
        protected virtual void Update()
        {
        }

        public virtual void OnBeginDrag(BaseEventData eventData)
        {
            var data = eventData as PointerEventData;
            _eventData.pointer[data.pointerId] = data;
            foreach (var listener in _listenerList)
            {
                ExecuteEvents.Execute<ITouchEventListener>(listener.gameObject, null, (x, y) => x.OnBeginDrag(data.pointerId, _eventData));
            }
        }

        public virtual void OnDrag(BaseEventData eventData)
        {
            var data = eventData as PointerEventData;
            _eventData.pointer[data.pointerId] = data;
            foreach (var listener in _listenerList)
            {
                ExecuteEvents.Execute<ITouchEventListener>(listener.gameObject, null, (x, y) => x.OnDrag(data.pointerId, _eventData));
            }
        }

        public virtual void OnEndDrag(BaseEventData eventData)
        {
            var data = eventData as PointerEventData;
            _eventData.pointer[data.pointerId] = data;
            foreach (var listener in _listenerList)
            {
                ExecuteEvents.Execute<ITouchEventListener>(listener.gameObject, null, (x, y) => x.OnEndDrag(data.pointerId, _eventData));
            }
            _eventData.pointer.Remove(data.pointerId);
        }

        public virtual void OnPointerClick(BaseEventData eventData)
        {
            var data = eventData as PointerEventData;
            _eventData.pointer[data.pointerId] = data;

            if (_eventData.pointer.Count != 1)
            {
                return;
            }
            
            _pointerClickNum++;
            if (_pointerClickNum > 1)
            {
                return;
            }

            // 300msでダブルタップを判定.
            StartCoroutine(this.DelayTimeSeconds(0.3f, () => 
            {
                foreach (var listener in _listenerList)
                {
                    if (_pointerClickNum == 1)
                    {
                        ExecuteEvents.Execute<ITouchEventListener>(listener.gameObject, null, (x, y) => x.OnPointerSingleClick(data));
                    }
                    else
                    {
                        ExecuteEvents.Execute<ITouchEventListener>(listener.gameObject, null, (x, y) => x.OnPointerDoubleClick(data));
                    }
                }
                _pointerClickNum = 0;
            }));
        }
    }
}