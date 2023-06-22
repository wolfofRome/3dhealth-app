using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Notification
{
    public class NotificationListItem : BaseNotificationItem
    {
        [SerializeField]
        private Button _button = default;
        public Button button {
            get {
                return _button;
            }
        }
    }
}
