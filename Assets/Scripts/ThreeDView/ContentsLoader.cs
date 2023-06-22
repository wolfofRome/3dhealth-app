using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Common;
using System.IO;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Loading;
using Assets.Scripts.Network;
using Assets.Scripts.Common.Data;
using Assets.Scripts.Network.Response;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Calendar;
using Assets.Scripts.Record;
using System.Collections;
using Assets.Scripts.PointPurchase;
using Assets.Scripts.PopUpHeight;
using Assets.Scripts.ThreeDView.Posture;
using Assets.Scripts.TalentList;

namespace Assets.Scripts.ThreeDView
{
    [RequireComponent(typeof(ScreenShooter))]
    public class ContentsLoader : BaseContentsLoader
    {
        [SerializeField]
        private Camera _mainObjCamera = default;

        [SerializeField]
        private List<TrialAccountConfig> _trialAccountConfigList = default;

        [SerializeField]
        private Text _measurementInfoText = default;

        [SerializeField]
        private List<Text> _dateTextList = default;
        
        [SerializeField]
        private RecordMenuController _recordMenu = default;

        [SerializeField]
        private CalendarDataSource _calendarDataSource = default;

        [SerializeField]
        private PointPurchaseDataSource _pointDataSource = default;

        [SerializeField]
        private ThreeDModeDataSource _threeDModeDataSource = default;

        [SerializeField]
        private TalentDataSource _talentDataSource = default;
        
        protected override bool useObjMemoryCache
        {
            get
            {
                return true;
            }
        }

        private class LoginHandler : BaseLoginHandler
        {
            protected override BaseSceneLoadParam GetLoadParam()
            {
                // 使用しないため、未実装.
                throw new NotImplementedException();
            }
        };

        public void SetSessionId(string sessionId)
        {
            if (ApiHelper.Instance.isLogin)
            {
                ApiHelper.Instance.Logout( res => {} );
            }
            ApiHelper.Instance.sessionId = sessionId;
        }

        protected override void Awake()
        {
            base.Awake();
            LoadingSceneManager.Instance.AddProgressAdapter(ProgressAdapter);
            LoadingSceneManager.Instance.Show();
        }

        protected override void Start()
        {
            base.Start();

            Action loadResourcesAction = () => {
                
                if (DataManager.Instance.Profile.Height > 0 ||
                    DataManager.Instance.ContentsList.Count > 0)
                {
                    LoadResources();
                }
                else
                {
                    LoadingSceneManager.Instance.Hide();

                    PopUpHeightLoadParam param = GetComponent<PopUpHeightLoadParam>();
                    param.argment.successAction = () => {
                        LoadingSceneManager.Instance.AddProgressAdapter(ProgressAdapter);
                        LoadingSceneManager.Instance.Show();
                        LoadResources();
                    };
                    param.argment.failedAction = () => {};

                    SceneLoader.Instance.LoadSceneWithParams(param);
                }
            };
#if UNITY_WEBGL

            if (ApiHelper.Instance.isLogin)
            {
                loadResourcesAction();
            }
            else
            {
                // ロード開始通知.
                // 未ログインの場合は外部からセッションIDを設定してもらう.
                // その後、任意のタイミングでLoadResourcesを呼び出してもらう.
                Application.ExternalCall("OnLoadStart");
            }
#else
            loadResourcesAction();
#endif
        }

        public void LoadResources()
        {
            var dm = DataManager.Instance;
            var args = SceneLoader.Instance.GetArgment<ThreeDViewLoadParam.Argment>(gameObject.scene.name);
            var resourceId = (args == null ? -1 : args.resourceId);
#if UNITY_WEBGL
            // プロフィール取得
            StartCoroutine(ApiHelper.Instance.GetLoginData(getLoginDataResponse => {
                if (getLoginDataResponse.ErrorCode != null)
                {
                    DebugUtil.LogError("GetLoginData Error -> " + getLoginDataResponse.ErrorCode + " : " + getLoginDataResponse.ErrorMessage);
                    return;
                }

                dm.Profile = getLoginDataResponse.Profile;

                // ユーザー情報を設定.
                dm.Cache.InsertOrReplace(new User
                {
                    Id = getLoginDataResponse.Profile.UserId,
                    LoginId = "",
                    Password = "",
                    Height = getLoginDataResponse.Profile.Height
                });

                // リソース一覧取得
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
                    var contents = dm.GetThreeDViewContents(resourceId);
                    if (contents != null)
                    {
                        StartCoroutine(_calendarDataSource.UpdateDataSource(contents.CreateTimeAsDateTime));
                        _recordMenu.date = contents.CreateTimeAsDateTime.ToString(ApiHelper.DateFormat);
                        StartLoading(contents);
                    }
                }));
            }));
#else
            // リソース一覧取得
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
                var contents = dm.GetThreeDViewContents(resourceId);
                if (contents == null)
                {
                    LoadingSceneManager.Instance.Hide();
                    SceneManager.LoadScene("PopUpForNewMember", LoadSceneMode.Additive);
                }
                else
                {
                    StartLoading(contents);
                    StartCoroutine(UpdateDataSources(contents));
                }
            }));
#endif
        }

        private IEnumerator UpdateDataSources(Contents contents)
        {
            // デバイス一覧を更新.
            yield return ApiHelper.Instance.GetDeviceList(response =>
            {
                if (response.ErrorCode != null)
                {
                    DebugUtil.LogError("GetDeviceList Error -> " + response.ErrorCode + " : " + response.ErrorMessage);
                    return;
                }
                DataManager.Instance.DeviceList = new List<Device>(response.DeviceList);
            });

            // カレンダーのデータを更新.
            var dateTime = contents.CreateTimeAsDateTime;
            yield return _calendarDataSource.UpdateDataSource(dateTime);
            _recordMenu.date = dateTime.ToString(ApiHelper.DateFormat);

            // ポイント残高を更新.
            yield return _pointDataSource.UpdateDataSource();

            // 有名人データ更新.
            yield return _talentDataSource.UpdateDataSource();
        }

        /// <summary>
        /// 体験版OBJのロード
        /// </summary>
        public void LoadTrialResources(int idx = 0)
        {
#if UNITY_WEBGL
            var args = SceneLoader.Instance.GetArgment<ThreeDViewLoadParam.Argment>(gameObject.scene.name);
            var resourceId = (args == null ? -1 : args.resourceId);
            var config = _trialAccountConfigList[Mathf.Clamp(idx, 0, _trialAccountConfigList.Count - 1)];
            var dm = DataManager.Instance;
            var handler = gameObject.AddComponent<LoginHandler>();
            handler.Login(config.LoginId, config.Password, (res) => {
                if (res.ErrorCode != null)
                {
                    DebugUtil.LogError("Login Error -> " + res.ErrorCode + " : " + res.ErrorMessage);
                    return;
                }
                var contents = dm.GetThreeDViewContents(resourceId);
                if (contents != null)
                {
                    StartLoading(contents);
                }
            });
#endif
        }

        protected override void StartLoading(Contents contents)
        {
            base.StartLoading(contents);

            var scanDate = contents.CreateTimeAsDateTime;
            var diff = DataManager.Instance.Profile.BirthdayOfDateTime.MonthDiff(scanDate);

            DataManager.Instance.Profile.GetAge(scanDate);
            var date = scanDate.ToString(AppConst.DateTimeFormat);
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                    _measurementInfoText.text = date + string.Format("  {0} 歳 {1} ヶ月", (diff / 12), (diff % 12));
                    break;
                case "Chinese":
                    _measurementInfoText.text = date + string.Format("  {0} 岁 {1} 几个月", (diff / 12), (diff % 12));
                    break;
                case "English":
                default:
                    _measurementInfoText.text = date + string.Format("  {0} Years Old {1} Months", (diff / 12), (diff % 12));
                    break;
            }
            _dateTextList.ForEach(dateText => { dateText.text = date; });
        }

        protected override void FinishLoading(Contents contents)
        {
            base.FinishLoading(contents);
            if (_threeDModeDataSource.mode == Mode.ThreeDView)
            {
                ScreenShot(contents);
            }
            LoadingSceneManager.Instance.Hide();
        }

        public void ScreenShot(Contents contents)
        {
            var dm = DataManager.Instance;
            var sd = dm.Cache.GetScanData(dm.Profile.UserId, contents.ResourceId, FileType.Thumbnail);
            // サムネイルが存在しない、または古い場合はサムネイル画像を生成
            if (sd == null || sd.Path == null || !File.Exists(sd.Path) || contents.ModifiedTime > sd.ModifiedTime)
            {
                string path = dm.CachePath.GetThumbnailFilePath(contents.ResourceId);

#if UNITY_WEBGL
                var targetCamera = _subObjCamera;
#else
                var targetCamera = _mainObjCamera;
#endif               
                GetComponent<ScreenShooter>().SaveScreenshot(path, targetCamera, savedPath =>
                {
                    if (savedPath != null && File.Exists(savedPath))
                    {
                        var thumbnail = new ScanData()
                        {
                            UserId = dm.Profile.UserId,
                            ResourceId = contents.ResourceId,
                            Type = FileType.Thumbnail,
                            CreateTime = contents.CreateTime,
                            ModifiedTime = contents.ModifiedTime,
                            Path = savedPath
                        };
                        dm.Cache.InsertOrReplace(thumbnail);
                    }
                });
                
            }
            
        }

        [SerializeField]
        private LayerMask _copyObjLayer = default;
        [SerializeField]
        private ObjController _orgObjController = default;

        public void OnLoadObj(ObjLoader loader, GameObject obj)
        {
#if UNITY_WEBGL
            // OBJのコピーを生成.
            var copyObj = Instantiate(obj) as GameObject;

            copyObj.layer = (int)Mathf.Log(_copyObjLayer.value, 2);
            var materials = obj.GetComponent<MeshRenderer>().materials;
            var copyMaterials = copyObj.GetComponent<MeshRenderer>().materials;
            
            // アルファ値等を別々に操作するため、マテリアルもコピーしておく.
            int i = 0;
            foreach (var mat in materials)
            {
                copyMaterials[i] = Instantiate(mat);
                i++;
            }

            var _objController = copyObj.AddComponent<ObjController>();
            _objController.FlatModeController = _orgObjController.FlatModeController;
            _objController.RegisterRendererController(copyObj);
#endif
        }

        public void OnContentsUpdated(Contents[] newContentsList)
        {
            SceneLoader.Instance.LoadScene(gameObject.scene.name);
        }
    }
}