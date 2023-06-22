using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Calendar
{
    public class Week : MonoBehaviour
    {
        [SerializeField]
        private Day _prefabDay = default;
        
        public Dictionary<DayOfWeek, Day> _days = new Dictionary<DayOfWeek, Day>();
        public Dictionary<DayOfWeek, Day> days {
            get {
                return _days;
            }
            set {
                _days = value;
            }
        }

        void Awake()
        {
            var dayOfWeekList = Enum.GetValues(typeof(DayOfWeek));
            foreach (DayOfWeek dw in dayOfWeekList)
            {
                var day = Instantiate(_prefabDay);
                day.transform.SetParent(transform, false);
                days[dw] = day;
            }
        }
    }
}
