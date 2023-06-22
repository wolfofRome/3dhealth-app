using System;
using System.Collections;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Response;
using Assets.Scripts.ThreeDView;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace Assets.Scripts.Common
{

    [RequireComponent(typeof(ContentsLoader))]
    public class FirebaseUserPropertySender : MonoBehaviour
    {
        private bool _isRunningDataLoadWaitProcess;
        private bool _isLatestContents;
        private bool _isTrial;

        private ContentsLoader _contentsLoader;

        private string _scannerId;
        private string _age;
        private string _gender;
        private string _scanLocation;

        // Start is called before the first frame update
        private void Start()
        {
#if !UNITY_EDITOR
            _contentsLoader = gameObject.GetComponent<ContentsLoader>();
            _contentsLoader.onLoadCompleted.AddListener(OnContentsLoadCompleted);
#endif
        }

        private IEnumerator WaitForDataLoadProcess(Action action)
        {
            if (_isRunningDataLoadWaitProcess)
            {
                yield break;
            }
            _isRunningDataLoadWaitProcess = true;

            while (_scanLocation == null || _scannerId == null)
            {
                yield return null;
            }

            action();

            _isRunningDataLoadWaitProcess = false;
        }

        public void OnContentsLoadCompleted(Contents contents)
        {
            InitializeFirebaseAnalysis(contents);

            StartCoroutine(WaitForDataLoadProcess(SendUserProperty));
        }

        private static string GetGenderText()
        {
            bool isMale = DataManager.Instance.Profile.Gender == Gender.Male;

            return isMale ? "Male" : "Female";
        }

        private void SendUserProperty()
        {
            if (_isTrial) { return; }
            if (!_isLatestContents) { return; }

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {

                var dependencyStatus = task.Result;

                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseAnalytics.SetUserProperty("SensorId", _scannerId);
                    FirebaseAnalytics.SetUserProperty("Age", _age);
                    FirebaseAnalytics.SetUserProperty("Gender", _gender);
                    FirebaseAnalytics.SetUserProperty("ScanLocation", _scanLocation);
                }
                else
                {
                    UnityEngine.Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }

        private void SetUserPropertyValue(DateTime latestDate)
        {
            StartCoroutine(ApiHelper.Instance.GetScannerInfo(res =>
            {
                if (res.ErrorCode != null)
                {
                    DebugUtil.LogError("GetScannerInfo -> " + res.ErrorCode + " : " + res.ErrorMessage);
                }

                _age = DataManager.Instance.Profile.GetAge(latestDate).ToString();
                _gender = GetGenderText();
                _scannerId = res.Resource.DeviceId;
                _scanLocation = res.Resource.StoreName;
            }));
        }

        private static bool IsLatestContents(Contents latestContents, Contents currentContents)
        {
            return latestContents.CreateTime == currentContents.CreateTime;
        }

        private void InitializeFirebaseAnalysis(Contents contents)
        {
            DataManager dm = DataManager.Instance;
            
            Contents latestContests = dm.GetLatestContents();
            
            _isLatestContents = IsLatestContents(latestContests, contents);
            _isTrial = dm.isTrialAccount;
            _isRunningDataLoadWaitProcess = false;

            SetUserPropertyValue(latestContests.CreateTimeAsDateTime);

        }
    }
}
