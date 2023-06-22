using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Response;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common
{
    public class UpdateChecker : MonoBehaviour

    {
        [SerializeField]
        private bool CheckForAllScanData = true;
        [SerializeField]
        private bool CheckForLatestScanData = true;
        [SerializeField]
        private bool CheckForUserBodyComposition = true;

        [Serializable]
        public class ContentsUpdateEvent : UnityEvent<Contents[]> {};
        [SerializeField]
        public ContentsUpdateEvent OnContentsUpdated = default;

        void Awake()
        {
        }

        void Start()
        {
        }

        void Update()
        {
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                // バックグラウンドからの復帰.
                CheckForUpdate();
            }
        }

        private void CheckForUpdate() {
            StartCoroutine(ApiHelper.Instance.GetList(res => {
                if (res.ContentsList == null)
                {
                    return;
                }
                if ((CheckForAllScanData && IsUpdateAllScanData(res.ContentsList)) ||
                    (CheckForLatestScanData && IsUpdateLatestScanData(res.ContentsList)) ||
                    (CheckForUserBodyComposition && IsUserBodyComposition(res.ContentsList)))
                {
                    OnContentsUpdated.Invoke(res.ContentsList);
                }
            }));
        }

        private bool IsUpdateAllScanData(Contents[] newContentsList)
        {
            var curContentsList = DataManager.Instance.ContentsList;
            var isUpdate = false;

            if (curContentsList != null)
            {
                // 更新すべきスキャンデータがあるかチェック.
                foreach (var newContents in newContentsList)
                {
                    var curContents = (from t in curContentsList
                                       where t.ResourceId == newContents.ResourceId
                                       select t).FirstOrDefault();
                    if (curContents == null || newContents.ModifiedTime > curContents.ModifiedTime)
                    {
                        isUpdate = true;
                        DebugUtil.Log("Exist New ScanData");
                        break;
                    }
                }
            }
            return isUpdate;
        }

        private bool IsUpdateLatestScanData(Contents[] newContentsList)
        {
            var dm = DataManager.Instance;
            var isUpdate = false;
            var latestContent = (from t in newContentsList
                                 orderby t.CreateTime descending
                                 select t).FirstOrDefault();
            var curContent = dm.GetLatestContents();

            // 最新のスキャンデータの更新をチェック.
            if (latestContent.ModifiedTime > curContent.ModifiedTime)
            {
                isUpdate = true;
                DebugUtil.Log("Exist New ScanData");
            }
            return isUpdate;
        }
        
        private bool IsUserBodyComposition(Contents[] newContentsList)
        {
            var dm = DataManager.Instance;
            var isUpdate = false;

            // 更新すべき体組成データがあるかチェック.
            foreach (var contents in newContentsList)
            {
                var cache = dm.Cache.GetBodyComposition(dm.Profile.UserId, contents.ResourceId);
                if (cache == null || contents.ModifiedTime > cache.ModifiedTime)
                {
                    isUpdate = true;
                    DebugUtil.Log("Exist New UserBodyComposition");
                    break;
                }
            }
            return isUpdate;
        }
    }
}
