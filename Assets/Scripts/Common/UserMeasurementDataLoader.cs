using Assets.Scripts.Common.Data;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Network.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Assets.Scripts.Common
{
    public class UserMeasurementDataLoader : BaseMeasurementDataLoader
    {
        [Serializable]
        public class MeasurementListUpdate : UnityEvent<UserMeasurementDataLoader> { }
        public MeasurementListUpdate OnUpdateMeasurementData;

        private CsvLoader _csvLoader;

        private List<Measurement> _measurementList;
        public List<Measurement> MeasurementList {
            get {
                return _measurementList;
            }
            private set {
                _measurementList = value;
                if (OnUpdateMeasurementData != null)
                {
                    OnUpdateMeasurementData.Invoke(this);
                }
            }
        }

        protected override void Awake()
        {
            _csvLoader = gameObject.AddComponent<CsvLoader>();
            _csvLoader.OnCsvLoadCompleted.AddListener(OnCsvLoadCompleted);
        }

        /// <summary>
        /// 全採寸データの読込.
        /// </summary>
        /// <param name="contentsList"></param>
        /// <param name="path"></param>
        public void LoadFromCacheOrDownload(List<Contents> contentsList, string path)
        {
            StartCoroutine(LoadFromCache(contentsList, retval =>
            {
                if (!retval)
                {
                    var param = new LoadParam()
                    {
                        Path = path,
                        OptionalParam = contentsList
                    };
                    _csvLoader.Load(param);
                }
            }));
        }

        /// <summary>
        /// 全採寸データの読込（キャッシュから読込）.
        /// </summary>
        /// <param name="contentsList"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private IEnumerator LoadFromCache(List<Contents> contentsList, Action<bool> result)
        {
            var isUpdate = false;
            var dm = DataManager.Instance;

            var cache = dm.Cache.GetMeasurement(dm.Profile.UserId);
            if (contentsList.Count > 0 && cache != null)
            {
                foreach (var content in contentsList)
                {
                    // 更新が必要かどうかチェック.
                    var updateItem = (from c in cache
                                      where c.ResourceId == content.ResourceId && content.ModifiedTime > c.ModifiedTime
                                      select c).FirstOrDefault();
                    if (updateItem != null)
                    {
                        isUpdate = true;
                        break;
                    }
                }
            }
            if (!isUpdate)
            {
                yield return 0;
                MeasurementList = cache.OrderByDescending(v => v.CreateTimeAsDateTime).ToList();
            }
            result(isUpdate);
            yield return 0;
        }

        /// <summary>
        /// 採寸データCSVの読込完了イベント.
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="csvData"></param>
        private void OnCsvLoadCompleted(CsvLoader loader, Csv csvData)
        {
            var userId = DataManager.Instance.Profile.UserId;
            var cache = DataManager.Instance.Cache;

            // キャッシュを更新.
            Measurement ms = null;
            var contentsList = (List<Contents>)loader.Param.OptionalParam;
            for (int i = 0; i < csvData.Length; i++)
            {
                ms = ParseCsvRow(csvData.GetRowValues(i));
                ms.UserId = userId;
                var contents = (from c in contentsList where c.ResourceId == ms.ResourceId select c).FirstOrDefault();
                if (contents == null)
                {
                    continue;
                }
                ms.CreateTime = contents.CreateTime;
                ms.ModifiedTime = contents.ModifiedTime;
                cache.InsertOrReplace(ms);
            }
            if (ms != null)
            {
                MeasurementList = cache.GetMeasurement(userId).OrderByDescending(v => v.CreateTimeAsDateTime).ToList();
            }
        }
    }
}