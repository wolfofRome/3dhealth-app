using Assets.Scripts.Common.Data;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.ThreeDView;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Assets.Scripts.Common
{
    public class MeasurementDataLoader : BaseMeasurementDataLoader
    {
        [Serializable]
        public class MeasurementUpdate : UnityEvent<MeasurementDataLoader> { }
        [FormerlySerializedAs("OnUpdateMeasurementData")] public MeasurementUpdate onUpdateMeasurementData;
        
        private CsvLoader _csvLoader;
        private CsvLoader CsvLoader {
            get {
                if (_csvLoader != null) return _csvLoader;
                _csvLoader = gameObject.AddComponent<CsvLoader>();
                _csvLoader.OnCsvLoadCompleted.AddListener(OnCsvLoadCompleted);
                return _csvLoader;
            }
        }

        private Measurement _measurement;
        public Measurement Measurement {
            get => _measurement;
            private set {
                _measurement = value;
                onUpdateMeasurementData?.Invoke(this);
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
            var cache = dm.Cache.GetMeasurement(dm.Profile.UserId, param.ResourceId);
            if (cache != null && cache.ModifiedTime >= param.ModifiedTime)
            {
                yield return 0;
                Measurement = cache;
                isLoaded = true;
            }
            result(isLoaded);
            yield return 0;
        }

        private void OnCsvLoadCompleted(CsvLoader loader, Csv csvData)
        {
            var ms = ParseCsvRow(csvData.GetRowValues(0));
            ms.UserId = DataManager.Instance.Profile.UserId;
            ms.CreateTime = loader.Param.CreateTime;
            ms.ModifiedTime = loader.Param.ModifiedTime;

            // キャッシュを更新.
            DataManager.Instance.Cache.InsertOrReplace(ms);

            Measurement = ms;
        }

        public void OnAvatarUpdated(AvatarController avatar)
        {
            try
            {
                var data = Measurement;

                // オブジェクトの座標と股下高を元に身長の値を補正.
                var buttonLeft = (Vector3)avatar.GetPoint(AvatarBones.LeftFoot);
                var buttonRight = (Vector3)avatar.GetPoint(AvatarBones.RightFoot);
                var buttonCenter = (buttonLeft + buttonRight) / 2;

                buttonLeft.y = 0;
                buttonRight.y = 0;
                buttonCenter.y = 0;

                // CSVの股下高に両足の中心座標.
                var inseam = new Vector3(buttonCenter.x, data.Inseam, buttonCenter.z);

                // 両足の長さの平均値を算出.
                var length = (Vector3.Distance(buttonLeft, inseam) + Vector3.Distance(buttonRight, inseam)) / 2;

                // 身長と股下高を足を閉じた状態の値に補正する.
                var correctionValue = Mathf.Max(length - inseam.y, 0);

                data.correctionValue = correctionValue;
                Measurement = data;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }
        }
    }
}