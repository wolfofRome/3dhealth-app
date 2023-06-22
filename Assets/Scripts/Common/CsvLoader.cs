using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Assets.Scripts.Network;
using Assets.Scripts.Common.Data;

namespace Assets.Scripts.Common
{
    public class CsvLoader : BaseLoader
    {
        public class CsvLoadEvent : UnityEvent<CsvLoader, Csv> { }
        public CsvLoadEvent OnCsvLoadCompleted = new CsvLoadEvent();

        public bool IsLoaded { get; private set; }

        public Csv CsvData { get; private set; }
        public LoadParam Param { get; private set; }

        public void Load(LoadParam param)
        {
            if (CsvData != null)
            {
                CsvData = null;
            }
            Param = param;
            StartCoroutine(DownloadAndImportFile(param));
        }

        protected virtual IEnumerator DownloadAndImportFile(LoadParam param)
        {
            CsvData = new Csv();

            yield return StartCoroutine(ApiHelper.Instance.GetFileAsString(param.Path, res =>
            {
                if (res.ErrorCode != null)
                {
                    // TODO:エラー処理
                    DebugUtil.Log("GetFileAsString -> " + res.ErrorCode + " : " + res.ErrorMessage);
                }
                else
                {
                    CsvData.SetSourceString(res.Data, true);
                    IsLoaded = true;
                }
            }));

            if (OnCsvLoadCompleted != null)
            {
                OnCsvLoadCompleted.Invoke(this, CsvData);
                yield return 0;
            }
        }
    }
}
