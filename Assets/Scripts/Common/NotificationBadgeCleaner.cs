#if UNITY_IOS && !UNITY_EDITOR
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
using System.Runtime.InteropServices;
#endif

namespace Assets.Scripts.Common
{
    public class NotificationBadgeCleaner : BaseSingletonMonoBehaviour<NotificationBadgeCleaner>
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void _ClearIconBadge();
#endif
        void Awake()
        {
            ClearIconBadge();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                // バックグラウンドからの復帰.
                ClearIconBadge();
            }
        }

        private static void ClearIconBadge()
        {
#if UNITY_IOS && !UNITY_EDITOR
            _ClearIconBadge();
#endif
        }
    }
}
