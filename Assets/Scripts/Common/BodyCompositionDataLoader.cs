using Assets.Scripts.Common.Data;
using Assets.Scripts.Database.DataRow;
using System;
using System.Collections;
using Database.DataRow.BodyComposition;
using UnityEngine.Events;

namespace Assets.Scripts.Common
{
    public class BodyCompositionDataLoader : BaseBodyCompositionDataLoader
    {
        [Serializable]
        public class BodyCompositionUpdate : UnityEvent<BodyCompositionDataLoader> { }
        public BodyCompositionUpdate OnUpdateBodyCompositionData;

        private CsvLoader _csvLoader;
        private CsvLoader CsvLoader {
            get {
                if (_csvLoader == null)
                {
                    _csvLoader = gameObject.AddComponent<CsvLoader>();
                    _csvLoader.OnCsvLoadCompleted.AddListener(OnCsvLoadCompleted);
                }
                return _csvLoader;
            }
        }

        private BodyComposition _bodyComposition;
        public BodyComposition BodyComposition {
            get {
                return _bodyComposition;
            }
            set {
                _bodyComposition = value;
                if (OnUpdateBodyCompositionData != null)
                {
                    OnUpdateBodyCompositionData.Invoke(this);
                }
            }
        }

        public void LoadFromCacheOrDownload(LoadParam param)
        {
            StartCoroutine(LoadFromCache(param, isLoaded =>
            {
                if (!isLoaded)
                {
                    CsvLoader.Load(param);
                }
            }));
        }

        private IEnumerator LoadFromCache(LoadParam param, Action<bool> result)
        {
            var isLoaded = false;
            var dm = DataManager.Instance;
            var cache = dm.Cache.GetBodyComposition(dm.Profile.UserId, param.ResourceId);
            if (cache != null && cache.ModifiedTime >= param.ModifiedTime)
            {
                yield return 0;
                BodyComposition = cache;
                isLoaded = true;
            }
            result(isLoaded);
            yield return 0;
        }

        private void OnCsvLoadCompleted(CsvLoader loader, Csv csvData)
        {
            BodyComposition bc = PerseCsvRow(csvData.GetRowValues(0));
            bc.UserId = DataManager.Instance.Profile.UserId;
            bc.CreateTime = loader.Param.CreateTime;
            bc.ModifiedTime = loader.Param.ModifiedTime;

            // キャッシュを更新.
            DataManager.Instance.Cache.InsertOrReplace(bc);

            BodyComposition = bc;
        }
    }
}
