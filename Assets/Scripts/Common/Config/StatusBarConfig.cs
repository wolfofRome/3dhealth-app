using UnityEngine;

namespace Assets.Scripts.Common.Config
{
    public class StatusBarConfig : ScriptableObject
    {
        public enum Style
        {
            Default = 0,
            Translucent,
            Opaque,
        }

        [SerializeField]
        private bool _hideStatusBar = false;

        [SerializeField]
        private bool _hideNavigationBar = false;

        [SerializeField]
        private Style _style = Style.Default;
        
        void OnEnable()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_hideStatusBar)
            {
                ApplicationChrome.statusBarState = ApplicationChrome.States.Hidden;
            }
            else
            {
                switch (_style)
                {
                    case Style.Default:
                    case Style.Opaque:
                        ApplicationChrome.statusBarState = ApplicationChrome.States.Visible;
                        break;
                    case Style.Translucent:
                        ApplicationChrome.statusBarState = ApplicationChrome.States.TranslucentOverContent;
                        break;
                }
            }

            if (_hideNavigationBar)
            {
                ApplicationChrome.navigationBarState = ApplicationChrome.States.Hidden;
            }
            else
            {
                ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
            }
#endif
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            UnityEditor.PlayerSettings.statusBarHidden = _hideStatusBar;

            switch (_style)
            {
                case Style.Default:
                    UnityEditor.PlayerSettings.iOS.statusBarStyle = UnityEditor.iOSStatusBarStyle.Default;
                    break;
                case Style.Opaque:
                    UnityEditor.PlayerSettings.iOS.statusBarStyle = UnityEditor.iOSStatusBarStyle.LightContent;
                    break;
                case Style.Translucent:
                    UnityEditor.PlayerSettings.iOS.statusBarStyle = UnityEditor.iOSStatusBarStyle.LightContent;
                    break;
            }
        }
#endif
    }
}
