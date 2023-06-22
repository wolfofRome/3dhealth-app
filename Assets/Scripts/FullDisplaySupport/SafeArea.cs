using UnityEngine;

namespace Assets.Scripts.FullDisplaySupport
{
    public static class SafeArea
    {
        public const float HeaderHeight = 60.0f;
        public const float FooterHeight = 60.0f;

        public static bool HasSafeArea()
        {
            string model = SystemInfo.deviceModel;
#if UNITY_EDITOR
            model = "iPhone10,3";
#endif
            string[] strings = model.Split(',');
            if (!model.Contains("iPhone"))
                return false;
            int version = int.Parse(strings[0].Replace("iPhone", ""));
            int generation = int.Parse(strings[1]);
            if (version == 10)
                return generation == 3 || generation == 6;
            
            return version >= 11;
        }
        
        public static bool IsFullDisplay()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            //below magic number 2.15 is based on iPhoneX resolution 2436x1125
            // 2436 / 1125 ≈ 2.15
            return screenHeight / screenWidth > 2.15f;
        }
    }
}