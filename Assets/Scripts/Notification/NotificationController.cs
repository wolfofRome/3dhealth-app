using Assets.Scripts.Common;
using Assets.Scripts.Network;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Notification
{
    public class NotificationController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _notificationListParent;
        private RectTransform notificationListParent {
            get {
                return _notificationListParent;
            }
        }

        [SerializeField]
        private NotificationDetail _notificationDetail;
        private NotificationDetail notificationDetail {
            get {
                return _notificationDetail;
            }
        }

        [SerializeField]
        private Button _navMenuButton;
        private Button navMenuButton {
            get {
                return _navMenuButton;
            }
        }

        [SerializeField]
        private Button _detailBackButton;
        private Button detailBackButton
        {
            get
            {
                return _detailBackButton;
            }
        }

        [SerializeField]
        private NotificationListItem _prefabNotificationListItem;
        private NotificationListItem prefabNotificationListItem {
            get {
                return _prefabNotificationListItem;
            }
        }

        [SerializeField]
        private GameObject _prefabDevider;
        private GameObject prefabDevider {
            get {
                return _prefabDevider;
            }
        }

        [SerializeField]
        private GameObject _titleBar;
        private GameObject titleBar
        {
            get
            {
                return _titleBar;
            }
        }

        private List<NotificationListItem> notificationItemList { get; set; }

        private void Start()
        {
            detailBackButton.onClick.AddListener(() => {
                SetDetailVisibility(false);
            });

            notificationItemList = new List<NotificationListItem>();

            StartCoroutine(ApiHelper.Instance.GetNotification(response =>
            {
                if (response.ErrorCode != null)
                {
                    DebugUtil.LogError("GetNotification -> " + response.ErrorCode + " : " + response.ErrorMessage);
                    return;
                }

                foreach (var item in response.NewsList)
                {
                    var listItem = Instantiate(prefabNotificationListItem);
                    listItem.notification = item;
                    listItem.transform.SetParent(notificationListParent, false);
                    Instantiate(prefabDevider).transform.SetParent(notificationListParent, false);

                    listItem.button.onClick.AddListener(() =>
                    {
                        notificationDetail.notification = listItem.notification;
                        SetDetailVisibility(true);
                    });
                    notificationItemList.Add(listItem);
                }
            }));
        }

        private void SetDetailVisibility(bool visibility)
        {
            navMenuButton.gameObject.SetActive(!visibility);

            notificationListParent.gameObject.SetActive(!visibility);
            
            if (visibility)
            {
                titleBar.SetActive(false);
                notificationDetail.menu.Show();
            }
            else
            {
                titleBar.SetActive(true);
                notificationDetail.menu.Hide();
            }
        }
    }
}
