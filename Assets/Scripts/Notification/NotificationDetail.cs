using Assets.Scripts.Common;
using Assets.Scripts.Network.Response;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Notification
{
    [RequireComponent(typeof(SlideMenuController))]
    public class NotificationDetail : BaseNotificationItem
    {
        [SerializeField]
        private Text _content = default;
        protected Text content {
            get {
                return _content;
            }
        }

        private SlideMenuController _menu;
        public SlideMenuController menu
        {
            get
            {
                if (_menu == null)
                {
                    _menu = GetComponent<SlideMenuController>();
                    var rt = GetComponent<RectTransform>();
                    _menu.slideAnimator.inPosition = new Vector2(-rt.rect.size.x, 0);
                    _menu.slideAnimator.outPosition = new Vector2(0, 0);
                }
                return _menu;
            }
        }

        public override Network.Response.Notification notification {
            get {
                return base.notification;
            }

            set {
                base.notification = value;
                content.text = notification.Body;
            }
        }
    }
}
