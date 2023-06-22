using Assets.Scripts.Network;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System;

#if UNITY_IOS && !UNITY_EDITOR
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#endif
namespace Assets.Scripts.Common
{
    public class NotificationRegistrar : BaseSingletonMonoBehaviour<NotificationRegistrar>　
    {
        private string _deviceToken = null;
        private string deviceToken
        {
            get
            {
                return _deviceToken;
            }
            set
            {
                DebugUtil.Log("Set device token : " + deviceToken);
                _deviceToken = value;
            }
        }
        protected bool isFirebaseInitialized = false;
        private string topic = "TestTopic";
        protected bool LogTaskCompletion(Task task, string operation)
        {
            bool complete = false;
            if (task.IsCanceled)
            {
                UnityEngine.Debug.Log(operation + " canceled.");
            }
            else if (task.IsFaulted)
            {
                UnityEngine.Debug.Log(operation + " encounted an error.");
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    string errorCode = "";
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        errorCode = string.Format("Error.{0}: ",
                          ((Firebase.Messaging.Error)firebaseEx.ErrorCode).ToString());
                    }
                    UnityEngine.Debug.Log(errorCode + exception.ToString());
                }
            }
            else if (task.IsCompleted)
            {
                UnityEngine.Debug.Log(operation + " completed");
                complete = true;
            }
            return complete;
        }
        void Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            
            InitializeFirebase();
            
#endif
#if UNITY_IOS && !UNITY_EDITOR
            NotificationServices.RegisterForNotifications(
                NotificationType.Alert |
                NotificationType.Badge |
                NotificationType.Sound);
            StartCoroutine(WaitGetDeviceTokenProcess());
#endif
        }
#if UNITY_IOS && !UNITY_EDITOR
        IEnumerator WaitGetDeviceTokenProcess()
        {
            while (NotificationServices.deviceToken == null)
            {
                yield return null;
            }
            deviceToken = System.BitConverter.ToString(NotificationServices.deviceToken).Replace("-", "");
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        public void OnRegister(string registerId)
        {
            deviceToken = registerId;
        }
#endif

        public void RegisterDeviceToken()
        {
#if !UNITY_WEBGL
            StartCoroutine(WaitRegistrationProcess());
#endif
        }

        private IEnumerator WaitRegistrationProcess()
        {
            while (string.IsNullOrEmpty(deviceToken))
            {
                yield return null;
            }
            int savedTokenId = UserData.RegistrationTokenId;
            int? tokenId = (savedTokenId > 0 ? (int?)savedTokenId : null);
            StartCoroutine(ApiHelper.Instance.RegisterDevice(tokenId, deviceToken, res =>
            {
                if (!string.IsNullOrEmpty(res.ErrorCode))
                {
                    DebugUtil.LogError("RegisterDevice -> " + res.ErrorMessage);
                }
                else
                {
                    DebugUtil.Log("Register completed : " + deviceToken);
                    UserData.RegistrationTokenId = res.TokenId;
                    UserData.Save();
                }
            }));
        }
        void InitializeFirebase()
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic).ContinueWith(task => {
                LogTaskCompletion(task, "SubscribeAsync");
            });
            UnityEngine.Debug.Log("Firebase Messaging Initialized");
            // This will display the prompt to request permission to receive
            // notifications if the prompt has not already been displayed before. (If
            // the user already responded to the prompt, thier decision is cached by
            // the OS and can be changed in the OS settings).
            Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWith(task => {
                LogTaskCompletion(task, "RequestPermissionAsync");
            });
            isFirebaseInitialized = true;
        }
        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            deviceToken = token.Token;
            UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        }
    }
}