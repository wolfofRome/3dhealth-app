using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class UpdateCalendarResponse : BaseResponse
    {
        [SerializeField]
        private CalendarUpdateInfo update_calendar = default;
        public CalendarUpdateInfo UpdateInfo {
            get {
                return update_calendar;
            }
            set {
                update_calendar = value;
            }
        }
    }
    
    [Serializable]
    public class CalendarUpdateInfo : BaseCalendarInfo
    {
    }
}
