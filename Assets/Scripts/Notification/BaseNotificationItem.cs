using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Notification
{
    public abstract class BaseNotificationItem : MonoBehaviour
    {
        [SerializeField]
        private Text _date = default;
        protected Text date {
            get {
                return _date;
            }
        }

        [SerializeField]
        private Text _title = default;
        protected Text title {
            get {
                return _title;
            }
        }

        private Network.Response.Notification _notification;
        public virtual Network.Response.Notification notification {
            get {
                return _notification;
            }
            set {
                _notification = value;
                date.text = notification.DisplayFrom;
                title.text = notification.Title;
            }
        }
    }
}
