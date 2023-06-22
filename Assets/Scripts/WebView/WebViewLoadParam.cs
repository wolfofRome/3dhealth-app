using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Assets.Scripts.WebView
{
    public enum Link
    {
        Profile = 0,
        Information = 1,
        NewRegistration = 2,
        PasswordReissue = 3,
        Appointment = 4,
        Reservation = 5,
        StandardValueTable = 6
    }

    public static class LinkExtension
    {
#if DEVELOP
        private const string BaseUrl = "https://stg-api.3dbodylab.jp/";
        private const string NoahBaseUrl = "https://stg.noah-plus.jp";
#else
        private const string BaseUrl = "https://api.3dbodylab.jp/";
        private const string NoahBaseUrl = "https://noah-plus.jp";
#endif

        private class Config
        {
            public string title { get; }
            public string url { get; }
            public Config(string title, string url)
            {
                this.title = title;
                this.url = url;
            }
        }

        private static readonly Dictionary<Link, Config> LinkMap = new Dictionary<Link, Config>
        {
            { Link.Profile, new Config("プロフィール", BaseUrl + "/app_pages/detail_finish.php") },
            { Link.Information, new Config("ジムについて", "https://www.3dbodylab.co.jp/lab/") },
            { Link.NewRegistration, new Config("「3Dbody-ID」登録", BaseUrl + "/app_pages/mail_input.php") },
            { Link.PasswordReissue, new Config("パスワード再発行", BaseUrl + "/app_pages/pass_remind_mail.php") },
            { Link.Appointment, new Config("3Dスキャン予約", BaseUrl + "/appointment/") },
#if DEVELOP
            { Link.Reservation , new Config("整骨院を探す", "https://noahplus.work/app") },
#else
            { Link.Reservation , new Config("整骨院を探す", NoahBaseUrl + "/app") },
#endif
            { Link.StandardValueTable, new Config("基準値表", BaseUrl + "/app_pages/body_composition_standard_value/table.html") }
        };

        public static string ToUrl(this Link link)
        {
            return LinkMap[link].url;
        }

        public static string ToTitle(this Link link)
        {
            return LinkMap[link].title;
        }
    }


    [Serializable]
    public class WebViewLoadParam : BaseSceneLoadParam
    {

        [Serializable]
        public class Argment : BaseArgment
        {

            [FormerlySerializedAs("_menuButtonVisibility")] [SerializeField]
            private bool menuButtonVisibility;
            public bool MenuButtonVisibility => menuButtonVisibility;

            [FormerlySerializedAs("_link")] [SerializeField]
            private Link link;
            public Link Link => link;

            [FormerlySerializedAs("_optionalParam")] [SerializeField]
            private string optionalParam;
            public string OptionalParam
            {
                get => optionalParam;
                set => optionalParam = value;
            }

            private IEnumerator Stop()
            {
                yield return new WaitForSeconds(1f);
            }

            public string GetUrl(bool isTrial)
            {
                var param = "?client_app=threedbodylab_pocket";

                switch (Link)
                {
                    case Link.Profile:
                    case Link.Information:
                    case Link.Appointment:
                        if (ApiHelper.Instance.isLogin && !isTrial)
                        {
                            param += "&session_id=" + ApiHelper.Instance.sessionId;
                        }
                        break;
                    case Link.Reservation:
                        param = "?3d_body_lab_id=" + DataManager.Instance.Cache.GetUser(UserData.AutoLoginUserId).LoginId + "&user_id=" + DataManager.Instance.Cache.GetUser(UserData.AutoLoginUserId).Id;
                        break;
                    case Link.NewRegistration:
                        break;
                    case Link.PasswordReissue:
                        break;
                    case Link.StandardValueTable:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!string.IsNullOrEmpty(OptionalParam))
                {
                    param += "&" + OptionalParam;
                }
                print("URL" + Link.ToUrl() + param);
                return Link.ToUrl() + param;
            }
        }

        [FormerlySerializedAs("_argment")] [SerializeField]
        private Argment webViewArgument;
        public Argment WebViewArgument
        {
            get => webViewArgument;
            set => webViewArgument = value;
        }

        [SceneName]
        private string _nextSceneName = "WebView";

        public override LoadSceneMode mode => LoadSceneMode.Additive;

        public override BaseArgment GetArgment()
        {
            return webViewArgument;
        }

        public override string GetNextSceneName()
        {
            return _nextSceneName;
        }
    }
}
