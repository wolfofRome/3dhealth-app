using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Loading;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Response;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.History
{
    [RequireComponent(typeof(ProgressAdapter))]
    [RequireComponent(typeof(UserMeasurementDataLoader))]
    [RequireComponent(typeof(UserBodyCompositionDataLoader))]
    public class HistoryLoader : MonoBehaviour
    {
        [Serializable]
        public class HistoryLoaderEvent : UnityEvent<HistoryLoader> { }
        public HistoryLoaderEvent OnLoadStart;
        public HistoryLoaderEvent OnLoadCompleted;

        protected int _loadTargetCount;
        protected int _loadCompleteCount;

        protected UserMeasurementDataLoader _measurementDataLoader;
        public UserMeasurementDataLoader measurementDataLoader {
            get {
                return _measurementDataLoader = _measurementDataLoader ?? GetComponent<UserMeasurementDataLoader>();
            }
        }

        protected UserBodyCompositionDataLoader _bodyCompositionDataLoader;
        public UserBodyCompositionDataLoader bodyCompositionDataLoader {
            get {
                return _bodyCompositionDataLoader = _bodyCompositionDataLoader ?? GetComponent<UserBodyCompositionDataLoader>();
            }
        }

        private ProgressAdapter _progressAdapter;
        protected ProgressAdapter ProgressAdapter {
            get {
                return _progressAdapter = _progressAdapter ?? GetComponent<ProgressAdapter>();
            }
        }

        protected virtual void Awake()
        {
            // プログレスアニメーションに必要以上に時間がかかるため、フレーム数を指定する.
            ProgressAdapter.UpdateFrameNum = 10;
            LoadingSceneManager.Instance.AddProgressAdapter(ProgressAdapter);
            LoadingSceneManager.Instance.Show();
        }

        protected virtual void Start()
        {
            var dm = DataManager.Instance;
            StartCoroutine(ApiHelper.Instance.GetList(getListResponse =>
            {
                if (getListResponse.ErrorCode != null)
                {
                    DebugUtil.LogError("GetList Error -> " + getListResponse.ErrorCode + " : " + getListResponse.ErrorMessage);
                    return;
                }
                dm.UserBodyCompositionPath = getListResponse.UserBodyCompositionCsv;
                dm.UserMeasurementPath = getListResponse.UserMeasurementCsv;
                dm.ContentsList = new List<Contents>();
                dm.ContentsList.AddRange(getListResponse.ContentsList);
                StartLoading();
            }));
        }

        protected virtual void StartLoading()
        {
            // 体組成データのロード.
            if (bodyCompositionDataLoader.enabled)
            {
                _loadTargetCount++;
                bodyCompositionDataLoader.OnUpdateBodyComposition.AddListener( loader =>
                {
                    ProgressAdapter.SecondaryProgress = GetProgress(++_loadCompleteCount, _loadTargetCount);
                });
                bodyCompositionDataLoader.LoadFromCacheOrDownload(DataManager.Instance.ContentsList, DataManager.Instance.UserBodyCompositionPath);
            }

            if (measurementDataLoader.enabled)
            {
                _loadTargetCount++;
                measurementDataLoader.OnUpdateMeasurementData.AddListener(loader =>
                {
                    ProgressAdapter.SecondaryProgress = GetProgress(++_loadCompleteCount, _loadTargetCount);
                });
                measurementDataLoader.LoadFromCacheOrDownload(DataManager.Instance.ContentsList, DataManager.Instance.UserMeasurementPath);
            }

            _loadCompleteCount = 0;

            ProgressAdapter.SecondaryProgress = 0;
            ProgressAdapter.OnProgressUpdateCompleted.AddListener( (adapter, progress) =>
            {
                FinishLoading();
            });

            OnLoadStart.Invoke(this);
        }

        protected virtual void FinishLoading()
        {
            OnLoadCompleted.Invoke(this);
            LoadingSceneManager.Instance.Hide();
        }

        private int GetProgress(int completeNum, int totalNum)
        {
            return Mathf.Clamp(ProgressAdapter.MaxProgress * completeNum / totalNum, ProgressAdapter.MinProgress, ProgressAdapter.MaxProgress);
        }
    }
}
