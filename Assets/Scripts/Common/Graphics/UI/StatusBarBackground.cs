using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Graphics.UI
{
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(Image))]
    public class StatusBarBackground : MonoBehaviour
    {
        [SerializeField]
        protected Color _color = Color.black;
        
        private Image _backgroundImage;
        private Image backgroundImage {
            get {
                return _backgroundImage = _backgroundImage ?? GetComponent<Image>();
            }
        }

        private LayoutElement _layout;
        private LayoutElement layout {
            get {
                return _layout = _layout ?? GetComponent<LayoutElement>();
            }
        }

        void Start()
        {
            var rootCanvas = transform.root.GetComponent<Canvas>();
            layout.preferredHeight = GetStatusbarHeight() * rootCanvas.scaleFactor;
            backgroundImage.color = _color;
        }
        
        void Update()
        {
        }
        

#if UNITY_IOS && !UNITY_EDITOR 
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern float _GetStatusBarHeight();
#endif

        public static float GetStatusbarHeight()
        {
            float height = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var window = activity.Call<AndroidJavaObject>("getWindow"))
            using (var view = window.Call<AndroidJavaObject>("getDecorView"))
            using (var rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                view.Call("getWindowVisibleDisplayFrame", rect);
                height = rect.Get<int>("top");
            }
#elif UNITY_IOS && !UNITY_EDITOR
            height = _GetStatusBarHeight();
#endif
            return height;
        }
    }
}