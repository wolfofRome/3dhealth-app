using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Common.InputEvent
{
    public class TouchEventData
    {
        private Dictionary<int, PointerEventData> _pointer;
        public Dictionary<int, PointerEventData> pointer {
            get {
                return _pointer;
            }
        }

        public TouchEventData()
        {
            _pointer = new Dictionary<int, PointerEventData>();
        }
    }
}
