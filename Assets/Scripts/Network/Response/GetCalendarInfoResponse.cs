using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class GetCalendarInfoResponse : BaseResponse
    {
        [SerializeField]
        private CalendarInfo[] calendar_list = default;
        public CalendarInfo[] CalendarInfoList {
            get {
                return calendar_list;
            }
            set {
                calendar_list = value;
            }
        }
    }

    [Serializable]
    public class CalendarInfo : BaseCalendarInfo
    {
        [SerializeField]
        private Appointment appointment = default;
        public Appointment Appointment {
            get {
                return appointment;
            }
            set {
                appointment = value;
            }
        }

        public Contents Contents { get; set; }

        public CalendarInfo()
        {
        }

        public CalendarInfo(BaseCalendarInfo src)
        {
            Set(src);
        }

        public override void Set(BaseCalendarInfo src)
        {
            base.Set(src);
            if (src != null && src.GetType() == typeof(CalendarInfo))
            {
                Appointment = ((CalendarInfo)src).Appointment;
                Contents = ((CalendarInfo)src).Contents;
            }
            else
            {
                Appointment = null;
                Contents = null;
            }
        }
    }

    [Serializable]
    public abstract class BaseCalendarInfo
    {
        [SerializeField]
        private string date = default;
        public string Date {
            get {
                return date;
            }
            set {
                date = value;
            }
        }

        public DateTime DateAsDateTime {
            get {
                return DateTime.Parse(date);
            }
        }

        [SerializeField]
        private string memo = default;
        public string Memo {
            get {
                return memo;
            }
            set {
                memo = value;
            }
        }

        [SerializeField]
        private Exercise[] exercise = default;
        public Exercise[] ExerciseList {
            get {
                return exercise;
            }
            set {
                exercise = value;
            }
        }

        public virtual void Set(BaseCalendarInfo src)
        {
            if (src != null)
            {
                Date = src.Date;
                Memo = src.Memo;
                ExerciseList = src.ExerciseList;
            }
            else
            {
                Date = null;
                Memo = null;
                ExerciseList = null;
            }
        }
    }
}
