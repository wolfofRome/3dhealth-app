using UnityEngine;

namespace Assets.Scripts.Graphics
{
    public static class DisplayUtil
    {
#if UNITY_ANDROID
        private static readonly float BASE_DPI = 160;
#elif UNITY_IOS
        private static readonly float BASE_DPI = 163;
#else
        private static readonly float BASE_DPI = Screen.dpi;
#endif

        public static Vector2 GetDisplaySizeInDP()
        {
            int wDp = (int)(Screen.width * GetDensityScale() + 0.5f);
            int hDp = (int)(Screen.height * GetDensityScale() + 0.5f);
            return new Vector2(wDp, hDp);
        }

        public static float GetDensityScale()
        {
            return BASE_DPI / Screen.dpi;
        }
    }
}
