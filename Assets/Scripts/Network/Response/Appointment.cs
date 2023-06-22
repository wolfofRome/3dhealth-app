using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class Appointment
    {
        [SerializeField]
        private long start_datetime = default;
        public long StartTime {
            get {
                return start_datetime;
            }
            set {
                start_datetime = value;
            }
        }

        public DateTime StartTimeAsDateTime {
            get {
                return DateTimeExtension.UnixTimestamp2LocalTime(StartTime);
            }
        }

        [SerializeField]
        private long end_datetime = default;
        public long EndTime {
            get {
                return end_datetime;
            }
            set {
                end_datetime = value;
            }
        }

        public DateTime EndTimeAsDateTime {
            get {
                return DateTimeExtension.UnixTimestamp2LocalTime(EndTime);
            }
        }

        [SerializeField]
        private string store_name = default;
        public string StoreName {
            get {
                return store_name;
            }
            set {
                store_name = value;
            }
        }

        [SerializeField]
        private string course_name = default;
        public string CourseName {
            get {
                return course_name;
            }
            set {
                course_name = value;
            }
        }
    }
}
