using Assets.Scripts.Common;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Graphics.UI;
using Assets.Scripts.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Assets.Scripts.FullDisplaySupport;

namespace Assets.Scripts.WebView
{
    public class WebViewController : MonoBehaviour
    {
        [SerializeField]
        private SlideMenuController _root = default;

        [SerializeField]
        private Text _title = default;

        [SerializeField]
        private Button _navMenuButton = default;

        [SerializeField]
        private Button _closeButton = default;

        [SerializeField]
        private Button _backButton = default;

        [SerializeField]
        private Button _editButton = default;

#if !UNITY_EDITOR_WIN
        private WebViewObject _webViewObject;
        private readonly char[] _keyValueDelimiter = new char[] { '=' };
#endif

        private readonly char[] _callbackParamDelimiter = new char[] { '&' };
        
        private const string JSFunctionURL = AppConst.BaseURL + "app_pages/js/unityfunction.js";

        IEnumerator Start()
        {
            var args = SceneLoader.Instance.GetArgment<WebViewLoadParam.Argment>(gameObject.scene.name);

            var isLogin = ApiHelper.Instance.isLogin;

            if (!ApiHelper.Instance.Maintenance)
            {
                if (isLogin && args.MenuButtonVisibility)
                {
                    _navMenuButton.gameObject.SetActive(true);
                    _closeButton.gameObject.SetActive(false);
                }
                else
                {
                    _navMenuButton.gameObject.SetActive(false);
                    _closeButton.gameObject.SetActive(true);
                }
            }
            else
            {
                if (isLogin)
                {
                    _navMenuButton.gameObject.SetActive(false);
                    _closeButton.gameObject.SetActive(false);
                }
                else
                {
                    _navMenuButton.gameObject.SetActive(true);
                    _closeButton.gameObject.SetActive(false);
                }
            }
            
            _closeButton.onClick.AddListener(Close);

#if !UNITY_EDITOR_WIN
            var rt = GetComponent<RectTransform>();
            var rt_parent = transform.parent.GetComponent<RectTransform>();

            var scaleFactor = transform.root.GetComponent<Canvas>().scaleFactor;

            var offsetMax = (rt.offsetMax + rt_parent.offsetMax) * scaleFactor * (-1);
            var offsetMin = (rt.offsetMin + rt_parent.offsetMin) * scaleFactor;

            int left = (int)offsetMin.x;
            int top = (int)(offsetMax.y + StatusBarBackground.GetStatusbarHeight() * scaleFactor);
            int right = (int)offsetMax.x;
            int bottom = (int)offsetMin.y;
            
            if (SafeArea.IsFullDisplay() || SafeArea.HasSafeArea())
            {
                top += (int)(SafeArea.HeaderHeight / 2);
                bottom += (int)SafeArea.FooterHeight;
            }

            var jsFunctions = UnityWebRequest.Get(JSFunctionURL);
            yield return jsFunctions;

            var callbackAction = new Dictionary<string, Action>() {
                { "Close" , Close}
            };

            _webViewObject = gameObject.AddComponent<WebViewObject>();
            _webViewObject.Init(param =>
            {
                DebugUtil.Log("callback -> " + param);
                if (callbackAction.ContainsKey(param))
                {
                    callbackAction[param]();
                }
                else if (param.IndexOf("email", System.StringComparison.CurrentCulture) != -1)
                {
                    // オートログインの場合は保存情報を更新.
                    var cache = DataManager.Instance.Cache;
                    var user = cache.GetUser(UserData.AutoLoginUserId);
                    if (user != null)
                    {
                        var sepIndex = param.IndexOf("=", System.StringComparison.CurrentCulture);
                        if (sepIndex != -1)
                        {
                            user.LoginId = param.Substring(sepIndex + 1);
                            cache.InsertOrReplace(user);
                        }
                    }
                }
                else if (param.IndexOf("http", System.StringComparison.CurrentCulture) != -1) 
                {
                    Application.OpenURL(param);
                }
                
                else if (param.IndexOf("mapOpen", System.StringComparison.CurrentCulture) != -1)
                {
                    Debug.Log("Param: " + param);
                    string baseMapUrl = "https://maps.apple.com/";
                    baseMapUrl += param.Replace("mapOpen:","");
                    Application.OpenURL(baseMapUrl);
                }

                var paramMap = Parse(param);
                var value = "";

                // タイトル切替.
                if (paramMap.TryGetValue("pagename", out value))
                {
                    _title.text = value;
                }

                // ボタン表示切替.
                if (paramMap.TryGetValue("return", out value))
                {
                    _title.text = paramMap["pagename"];
                    var backButtonVisibility = value == "on";

                    _backButton.gameObject.SetActive(backButtonVisibility);

                    if (backButtonVisibility)
                    {
                        _navMenuButton.gameObject.SetActive(false);
                        _closeButton.gameObject.SetActive(false);
                        _editButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        _navMenuButton.gameObject.SetActive(isLogin);
                        _closeButton.gameObject.SetActive(!isLogin);
                        
                        if (args.Link == Link.Profile)
                        {
                            _editButton.gameObject.SetActive(true);
                        }
                    }
                }
            },
                enableWKWebView: true);
            
            _backButton.onClick.AddListener(() =>
            {
                _webViewObject.EvaluateJS(jsFunctions.downloadHandler.text + " returnPage();");
            });

            _editButton.onClick.AddListener(() =>
            {
                _webViewObject.EvaluateJS(jsFunctions.downloadHandler.text + " editProfile();");
            });
            
            if (ApiHelper.Instance.Maintenance)
            {
                _title.text = "";
                _webViewObject.LoadURL("https://stg-api.3dbodylab.jp/maintenance/maintenance.php");
            }
            else
            {
                _title.text = args.Link.ToTitle();
                _webViewObject.LoadURL(args.GetUrl(DataManager.Instance.isTrialAccount));
            }
            _webViewObject.SetVisibility(true);
            _webViewObject.SetMargins(left, top, right, bottom);

#endif

            StartCoroutine(this.DelayFrame(1, () =>
            {
                _root.Show();
            }));

            yield return 0;
        }

        private Dictionary<string, string> Parse(string src)
        {
            var param = new Dictionary<string, string>();
            var paramString = src.Split(_callbackParamDelimiter);
            foreach (var str in paramString)
            {
                var sepIndex = str.IndexOf("=", System.StringComparison.CurrentCulture);
                if (sepIndex != -1)
                {
                    param[str.Substring(0, sepIndex)] = Uri.UnescapeDataString(str.Substring(sepIndex + 1));
                }
            }
            return param;
        }

        private void OnDestroy()
        {
#if !UNITY_EDITOR_WIN
            _webViewObject.SetVisibility(false);
            Destroy(_webViewObject);
#endif
        }

        public void Close()
        {
            _root.slideAnimator.onFinishAnimation.RemoveAllListeners();

            _root.slideAnimator.onFinishAnimation.AddListener( go =>
            {
#if !UNITY_EDITOR_WIN
                _webViewObject.SetVisibility(false);
#endif
                SceneManager.UnloadSceneAsync(gameObject.scene.name);
            });
            _root.Hide();
        }

        public void OnNavMenuOpened()
        {
#if !UNITY_EDITOR_WIN
            _webViewObject.SetVisibility(false);
            _webViewObject.enabled = false;
#endif
        }

        public void OnNavMenuClosed()
        {
#if !UNITY_EDITOR_WIN
            _webViewObject.SetVisibility(true);
            _webViewObject.enabled = true;
#endif
        }

        public void OnNavMenuItemClicked()
        {
            Close();
        }
    }
}
