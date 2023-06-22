using Assets.Scripts.Common;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Network.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Assets.Scripts.Network
{
    public class ApiHelper
    {
        public const int GetFlgUserData = 1;
        public const int GetFlgResource = 1 << 1;

        public const string DateFormat = "yyyy-MM-dd";

        private readonly string GetNotificationRequest = Resources.Load<TextAsset>("WordPressRequests/GetNotification").text;

        private Dictionary<string, BaseArgment> _argments = new Dictionary<string, BaseArgment>();

        private static class URL
        {
            public const string Login = AppConst.BaseURL + "api/user_data/login.php";
            public const string Logout = AppConst.BaseURL + "api/user_data/logout.php";
            public const string AddUserHeight = AppConst.BaseURL + "api/user_data/add_height.php";
            public const string RegisterDevice = AppConst.BaseURL + "api/push_notification/register_sns.php";
            public const string DeleteDevice = AppConst.BaseURL + "api/push_notification/delete_sns.php";
            public const string GetLoginData = AppConst.BaseURL + "api/user_data/get_login_data.php";
            public const string GetDeviceList = AppConst.BaseURL + "api/device/get_list.php";
            public const string GetList = AppConst.BaseURL + "api/resource/get_list.php";
            public const string GetFile = AppConst.BaseURL + "api/resource/get_file.php";
            public const string GetCalendarInfo = AppConst.BaseURL + "api/calendar/get.php";
            public const string UpdateExercise = AppConst.BaseURL + "api/calendar/update_exercise.php";
            public const string UpdateMemo = AppConst.BaseURL + "api/calendar/update_memo.php";
            public const string GetScannerInfo = AppConst.BaseURL + "api/resource/get_latest_resource.php";

            /// 姿勢アドバイス関連.
            public const string GetPurchasedAdvice = AppConst.BaseURL + "api/position/get.php";
            public const string PurchaseAdvice = AppConst.BaseURL + "api/position/purchase.php";

            // 有名人関連.
            public const string GetTalentList = AppConst.BaseURL + "api/talent/get_list.php";
            public const string PurchaseTalent = AppConst.BaseURL + "api/talent/purchase.php";

            // ポイント購入関連.
            public const string GetPoint = AppConst.BaseURL + "api/point/get.php";
            public const string SetPoint = AppConst.BaseURL + "api/point/purchase.php";

            public const string GetNotification = AppConst.BaseURL + "api/news/get.php";
        }
        
        private static ApiHelper _instance;
        public static ApiHelper Instance {
            get {
                return _instance = _instance ?? new ApiHelper();
            }
        }
        
        private ApiHelper() {
        }

        public bool isLogin {
            get {
                return sessionId != null ? true : false;
            }
        }

        private string _sessionId;
        public string sessionId {
            get {
                return _sessionId;
            }
            set {
                _sessionId = value;
            }
        }

        private bool _isMaintenance;
        public bool Maintenance
        {
            get
            {
                return _isMaintenance;
            }
            set
            {
                _isMaintenance = value;
            }
        }

        [SerializeField]
        private bool _isMachineOs;
        public bool isMachineOs
        {
            get
            {
                return _isMachineOs;
            }
            set
            {
                _isMachineOs = value;
            }
        }

        [SerializeField]
        private string _to;
        public string to
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
            }
        }

        [SerializeField]
        private string _from;
        public string from
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="login_id">ログインID</param>
        /// <param name="password">パスワード</param>
        /// <param name="getFlg">取得対象データ(ORで複数指定可)</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator Login(string login_id, string password, int getFlg, Action<LoginResponse> callback)
        {
            var param = new Dictionary<string, string>()
            {
                {"login_id", login_id },
                {"password", password },
            };
            if ((getFlg & GetFlgUserData) != 0)
            {
                param["get_user_data"] = "1";
            }
            if ((getFlg & GetFlgResource) != 0)
            {
                param["get_resource"] = "1";
            }
            return HttpGet(URL.Login, param, www => {
                LoginResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

                    //メンテナンスモードの実装
                    if (res.ErrorCode == null)
                    {
                        // ログイン成功時は、次回以降の通信のためセッションIDを設定.
                        sessionId = res.SessionId;
                      
                        _isMaintenance = res.Maintenance.isMaintenance;
                        _to = res.Maintenance._to;
                        _from = res.Maintenance._from;
#if UNITY_ANDROID && !UNITY_EDITOR
                        _isMachineOs = res.Maintenance._isAndroid;
#elif UNITY_IOS && !UNITY_EDITOR
                        _isMachineOs = res.Maintenance._isIos;
#endif                  
                    }
                }
                callback(res);
            });
        }

        /// <summary>
        /// ログアウト
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator Logout(Action<LogoutResponse> callback)
        {
            // クッキーに設定したSessionIDを削除.
            return HttpGet(URL.Logout, null, www => {
                LogoutResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<LogoutResponse>(www.downloadHandler.text);
                }
                sessionId = null;
                callback(res);
            });
        }

        /// <summary>
        /// 身長の登録
        /// </summary>
        /// <param name="height">身長</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator AddUserHeight(int height, Action<AddUserHeightResponse> callback)
        {
            var param = new Dictionary<string, string>()
            {
                { "height", UnityWebRequest.EscapeURL(height.ToString()) }
            };

            return HttpGet(URL.AddUserHeight, param, www => {
                AddUserHeightResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<AddUserHeightResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// デバイスの登録
        /// </summary>
        /// <param name="tokenId">登録済トークンのID</param>
        /// <param name="deviceToken">デバイストークン</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator RegisterDevice(int? tokenId, string deviceToken, Action<RegisterDeviceResponse> callback)
        {
            var param = new Dictionary<string, string>()
            {
                {"token", UnityWebRequest.EscapeURL(deviceToken) }
            };

            SetAppInfo(ref param);

            if (tokenId != null)
            {
                param.Add("token_id", tokenId.ToString());
            }

            return HttpGet(URL.RegisterDevice, param, www => {
                RegisterDeviceResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<RegisterDeviceResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// デバイスの削除
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator DeleteDevice(int tokenId, Action<DeleteDeviceResponse> callback)
        {
            var param = new Dictionary<string, string>()
            {
                {"token_id", tokenId.ToString() },
            };

            return HttpGet(URL.DeleteDevice, param, www => {
                DeleteDeviceResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<DeleteDeviceResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// ログイン情報の取得
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetLoginData(Action<GetLoginDataResponse> callback)
        {
            return HttpGet(URL.GetLoginData, null, www => {
                GetLoginDataResponse res = null;
                if (GetMimeType(www.GetResponseHeaders()) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<GetLoginDataResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// 筐体（スキャナー）一覧の取得
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetDeviceList(Action<GetDeviceListResponse> callback)
        {
            return HttpGet(URL.GetDeviceList, null, www => {
                GetDeviceListResponse res = null;
                if (GetMimeType(www.GetResponseHeaders()) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<GetDeviceListResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// リソース一覧の取得
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetList(Action<GetListResponse> callback)
        {
            return HttpGet(URL.GetList, null, www => {
                GetListResponse res = null;
                if (GetMimeType(www.GetResponseHeaders()) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<GetListResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// コンテンツ（バイナリ形式）の取得
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetFileAsBinary(string path, Action<GetFileResponse<byte[]>> callback)
        {
            var param = new Dictionary<string, string>()
            {
                {"resource_path", path },
            };
            return HttpGet(URL.GetFile, param, www => {
                GetFileResponse<byte[]> res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<GetFileResponse<byte[]>>(www.downloadHandler.text);
                }
                else
                {
                    res = new GetFileResponse<byte[]>();
                    res.Data = www.downloadHandler.data;
                }
                callback(res);
            });
        }

        /// <summary>
        /// コンテンツ（テキスト形式）の取得
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetFileAsString(string path, Action<GetFileResponse<string>> callback)
        {
            var param = new Dictionary<string, string>()
            {
                {"resource_path", path },
            };
            return HttpGet(URL.GetFile, param, www => {
                GetFileResponse<string> res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<GetFileResponse<string>>(www.downloadHandler.text);
                }
                else
                {
                    res = new GetFileResponse<string>();
                    res.Data = www.downloadHandler.text;
                }
                callback(res);
            });
        }
        
        /// <summary>
        /// カレンダー情報の取得
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetCalendarInfo(DateTime? from, DateTime? to, Action<GetCalendarInfoResponse> callback)
        {
            var param = new Dictionary<string, string>();

            if (from != null)
            {
                param["from"] = ((DateTime)from).ToString(DateFormat);
            }

            if (to != null)
            {
                param["to"] = ((DateTime)to).ToString(DateFormat);
            }

            return HttpGet(URL.GetCalendarInfo, param, www => {
                GetCalendarInfoResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<GetCalendarInfoResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// 運動履歴の更新
        /// </summary>
        /// <param name="exerciseList"></param>
        /// <param name="date"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        /// 
        public IEnumerator UpdateCalendarInfo(ExerciseId[] exerciseList, DateTime date, Action<UpdateCalendarResponse> callback)
        {
            return UpdateCalendarInfo(exerciseList, date.ToString(DateFormat), callback);
        }

        public IEnumerator UpdateCalendarInfo(ExerciseId[] exerciseList, string date, Action<UpdateCalendarResponse> callback)
        {
            var param = new Dictionary<string, string>()
            {
                { "date", date }
            };

            if (exerciseList != null)
            {
                var len = exerciseList.Length;
                for (var i = 0; i < len; i++)
                {
                    param[string.Format("exercise[{0}]", i)] = ((int)exerciseList[i]).ToString();
                }
            }

            return HttpGet(URL.UpdateExercise, param, www => {
                UpdateCalendarResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<UpdateCalendarResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// メモの更新
        /// </summary>
        /// <param name="memo"></param>
        /// <param name="date"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        /// 
        public IEnumerator UpdateCalendarInfo(string memo, DateTime date, Action<UpdateCalendarResponse> callback)
        {
            return UpdateCalendarInfo(memo, date.ToString(DateFormat), callback);
        }

        public IEnumerator UpdateCalendarInfo(string memo, string date, Action<UpdateCalendarResponse> callback)
        {
            var param = new Dictionary<string, string>()
            {
                { "date", date }
            };

            if (memo != null)
            {
                param["memo"] = Uri.EscapeDataString(memo);
            }
            
            return HttpGet(URL.UpdateMemo, param, www => {
                UpdateCalendarResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<UpdateCalendarResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }
        
        /// <summary>
        /// ポイントの取得.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetPoint(Action<GetPointResponse> callback)
        {
            return HttpGet(URL.GetPoint, null, www => {
                GetPointResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<GetPointResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }
        
        /// <summary>
        /// ポイントの設定
        /// </summary>
        /// <param name="point"></param>
        /// <param name="price"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator SetPoint(int point, int price, Action<SetPointResponse> callback)
        {
            var param = new Dictionary<string, string>()
            {
                { "point", point.ToString() },
                { "price", price.ToString() }
            };

            SetAppInfo(ref param);

            return HttpGet(URL.SetPoint, param, www => {
                SetPointResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<SetPointResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// タレント一覧の取得.
        /// </summary>
        /// <param name="categoryId">取得対象のカテゴリーID</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetTalentList(int? categoryId, Action<GetTalentResponse> callback)
        {
            var param = new Dictionary<string, string>();

            if (categoryId != null)
            {
                param.Add("category_id", categoryId.ToString());
            }

            return HttpGet(URL.GetTalentList, param, www => {
                GetTalentResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<GetTalentResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// タレントスキャンデータの購入.
        /// </summary>
        /// <param name="consumePoint"></param>
        /// <param name="talentResourceId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator PurchaseTalent(int consumePoint, int talentResourceId, Action<PurchaseTalentResponse> callback)
        {
            var param = new Dictionary<string, string>()
            {
                { "point", consumePoint.ToString() },
                { "talent_resource_id", talentResourceId.ToString() }
            };

            return HttpGet(URL.PurchaseTalent, param, www => {
                PurchaseTalentResponse res = null;
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<PurchaseTalentResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// WordPressのお知らせ取得.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetNotification(Action<GetNotificationResponse> callback)
        {
            return HttpGet(URL.GetNotification, null, www => {
                var res = new GetNotificationResponse();
                var headers = www.GetResponseHeaders();
                if (GetMimeType(headers) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<GetNotificationResponse>(www.downloadHandler.text);
                }


                callback(res);
            });
        }

        /// <summary>
        /// MIME-Typeの取得
        /// </summary>
        /// <param name="httpHeaders"></param>
        /// <returns></returns>
        private MimeType GetMimeType(Dictionary<string, string> httpHeaders)
        {
            foreach (KeyValuePair<string, string> pair in httpHeaders)
            {
                if (pair.Key.ToLower().Equals("content-type"))
                {
                    var values = pair.Value.Split(new string[] { "; " }, StringSplitOptions.None);

                    foreach (var v in values)
                    {
                        MimeType type;
                        if (MimeTypeExtension.TryParse(v, out type))
                        {
                            return type;
                        }
                    }
                }
            }
            return MimeType.Unknown;
        }

        public IEnumerator GetScannerInfo(Action<GetScannerInfoResponse> callback)
        {
            return HttpGet(URL.GetScannerInfo, null, www => {
                GetScannerInfoResponse res = null;
                if (GetMimeType(www.GetResponseHeaders()) == MimeType.Json)
                {
                    res = JsonUtility.FromJson<GetScannerInfoResponse>(www.downloadHandler.text);
                }
                callback(res);
            });
        }

        /// <summary>
        /// ファイル読込み
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private byte[] ReadFile(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader bin = new BinaryReader(fileStream);
            byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

            bin.Close();

            Assert.IsNotNull(values);

            return values;
        }


        /// <summary>
        /// アプリ情報の付与.
        /// </summary>
        /// <param name="param"></param>
        private void SetAppInfo(ref Dictionary<string, string> param)
        {
#if UNITY_ANDROID
            param.Add("app_os", "Android");
#endif
#if UNITY_IOS
            param.Add("app_os", "iOS");
#endif
            param.Add("app_version", Application.version);
        }

        /// <summary>
        /// GETリクエストの送信
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator HttpGet(string url, Dictionary<string, string> param, Action<UnityWebRequest> callback, bool useCommonParams = true)
        {
            if (useCommonParams)
            {
                var commonParams = new Dictionary<string, string>();

                // API認証キーを設定.
                commonParams.Add("key", UnityWebRequest.EscapeURL(GetEncryptedAuthKey()));

                if (isLogin)
                {
                    // セッションIDを設定.
                    commonParams.Add("session_id", sessionId);
                }
                url = FetchGetParams(url, commonParams);
            }

            if (param != null && param.Count > 0)
            {
                url = FetchGetParams(url, param);
            }

            var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            
            if (www.error != null)
            {
                DebugUtil.LogError("HttpGet response -> " + www.error);
            }
            callback(www);
        }

        /// <summary>
        /// POSTリクエストの送信
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator HttpPost(string url, Dictionary<string, string> param, string data, Dictionary<string, string> headers, Action<UnityWebRequest> callback, bool useCommonParams = true)
        {
            if (useCommonParams)
            {
                var commonParams = new Dictionary<string, string>();

                // API認証キーを設定.
                commonParams.Add("key", UnityWebRequest.EscapeURL(GetEncryptedAuthKey()));

                if (isLogin)
                {
                    // セッションIDを設定.
                    commonParams.Add("session_id", sessionId);
                }
                url = FetchGetParams(url, commonParams);
            }

            if (param != null && param.Count > 0)
            {
                url = FetchGetParams(url, param);
            }

            var www = UnityWebRequest.Post(url, data);
            foreach (var header in headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }

            yield return www.SendWebRequest();

            if (www.error != null)
            {
                DebugUtil.LogError("HttpPost response -> " + www.error);
            }
            callback(www);
        }

        /// <summary>
        /// 暗号化されたAPI認証キーの取得.
        /// </summary>
        /// <returns></returns>
        private string GetEncryptedAuthKey()
        {
            const string encryptKey = "D5dP5ZIkk9hxfh4pPRkGZhB4YXuyMhem";
            const string encryptIV = "19511840405955688082655089010650";
            string srcString = "3DBodyLab" + DateTime.Now.ToString("yyyyMMddHHmmss");

            return Common.Util.Security.EncryptString(srcString, encryptKey, encryptIV);
        }

        /// <summary>
        /// GETパラメータを連結したURLの生成.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private string FetchGetParams(string url, Dictionary<string, string> param)
        {
            foreach (var pair in param)
            {
                url += (url.Contains("?") ? "&" : "?");
                url += pair.Key + "=" + pair.Value;
            }
            return url;
        }
    }
}
