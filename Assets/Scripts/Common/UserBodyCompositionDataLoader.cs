using Assets.Scripts.Common.Data;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Network.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Database.DataRow.BodyComposition;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Common
{
    public class UserBodyCompositionDataLoader : BaseBodyCompositionDataLoader
    {
        [Serializable]
        public class BodyCompositionListUpdateEvent : UnityEvent<UserBodyCompositionDataLoader> { }
        public BodyCompositionListUpdateEvent OnUpdateBodyComposition;

        private CsvLoader _csvLoader;

        private List<BodyComposition> _bodyCompositionList;
        public List<BodyComposition> BodyCompositionList {
            get {
                return _bodyCompositionList;
            }
            private set {
                _bodyCompositionList = value;
                if (OnUpdateBodyComposition != null)
                {
                    OnUpdateBodyComposition.Invoke(this);
                }
            }
        }

        protected override void Awake()
        {
            _csvLoader = gameObject.AddComponent<CsvLoader>();
            _csvLoader.OnCsvLoadCompleted.AddListener(OnCsvLoadCompleted);
        }

        /// <summary>
        /// 全体組成データの読込.
        /// </summary>
        /// <param name="contentsList"></param>
        /// <param name="path"></param>
        public void LoadFromCacheOrDownload(List<Contents> contentsList, string path)
        {
            StartCoroutine(LoadFromCache(contentsList, isLoaded =>
            {
                if (!isLoaded)
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
        /// 全体組成データの読込（キャッシュから読込）.
        /// </summary>
        /// <param name="contentsList"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private IEnumerator LoadFromCache(List<Contents> contentsList, Action<bool> result)
        {
            var loadFromCache = true;
            var dm = DataManager.Instance;
            var cache = dm.Cache.GetBodyComposition(dm.Profile.UserId);

            // 更新が必要かどうかチェック.
            foreach (var content in contentsList)
            {
                var updateItem = (from c in cache
                                  where c.ResourceId == content.ResourceId && c.ModifiedTime >= content.ModifiedTime
                                  select c).FirstOrDefault();
                if (updateItem == null)
                {
                    loadFromCache = false;
                    break;
                }
            }
            if (loadFromCache)
            {
                yield return 0;
                BodyCompositionList = cache.OrderByDescending(v => v.CreateTimeAsDateTime).ToList();
            }
            result(loadFromCache);
            yield return 0;
        }

        /// <summary>
        /// 体組成データCSVの読込完了イベント.
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="csvData"></param>
        private void OnCsvLoadCompleted(CsvLoader loader, Csv csvData)
        {
            var userId = DataManager.Instance.Profile.UserId;
            var cache = DataManager.Instance.Cache;

            BodyComposition bc = null;
            var contentsList = (List<Contents>)loader.Param.OptionalParam;

            // キャッシュを更新.
            for (int i = 0; i < csvData.Length; i++)
            {
                bc = PerseCsvRow(csvData.GetRowValues(i));
                bc.UserId = userId;
                var contents = (from c in contentsList where c.ResourceId == bc.ResourceId select c).FirstOrDefault();
                if (contents == null)
                {
                    continue;
                }
                bc.CreateTime = contents.CreateTime;
                bc.ModifiedTime = contents.ModifiedTime;
                cache.InsertOrReplace(bc);
            }
            BodyCompositionList = cache.GetBodyComposition(userId).OrderByDescending(v => v.CreateTimeAsDateTime).ToList();
        }
        
        /// <summary>
        /// 体組成データの更新イベント.
        /// </summary>
        /// <param name="newContentsList"></param>
        public void OnContentsUpdated(Contents[] newContentsList)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}