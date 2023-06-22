using Assets.Scripts.Network;
using Assets.Scripts.PopUpConfirm;
using Assets.Scripts.WebView;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class ExternalBrowserController : MonoBehaviour
    {
        [EnumAction(typeof(Link))]
        public void StartExternalBrowser(int link)
        {
            string url = ((Link)link).ToUrl();
#if UNITY_EDITOR
            Application.OpenURL(url);
#elif UNITY_WEBGL
            Application.ExternalEval(string.Format("window.open('{0}','_blank')", url));
#else
            Application.OpenURL(url);
#endif
        }
    }
}
