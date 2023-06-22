using UnityEngine;

#if UNITY_IOS

using System.Runtime.InteropServices;

#endif

namespace SocialConnector
{
	public class SocialConnector
	{
#if UNITY_IOS
		[DllImport ("__Internal")]
		private static extern void SocialConnector_Share (string text, string url, string textureUrl);


#elif UNITY_ANDROID
		private static AndroidJavaObject clazz = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		private static AndroidJavaObject activity = clazz.GetStatic<AndroidJavaObject> ("currentActivity");
#endif

#if UNITY_IOS
		
		private static void _Share (string text, string url, string textureUrl)
		{
			SocialConnector_Share (text, url, textureUrl);
		}


#elif UNITY_ANDROID
		
		private static void _Share (string text, string url, string textureUrl)
		{
			using (var intent = new AndroidJavaObject ("android.content.Intent")) {
				intent.Call<AndroidJavaObject> ("setAction", "android.intent.action.SEND");
				intent.Call<AndroidJavaObject> ("setType", string.IsNullOrEmpty (textureUrl) ? "text/plain" : "image/png");

				if (!string.IsNullOrEmpty (url))
					text += "\t" + url;
				if (!string.IsNullOrEmpty (text))
					intent.Call<AndroidJavaObject> ("putExtra", "android.intent.extra.TEXT", text);

				if (!string.IsNullOrEmpty (textureUrl)) {
                    int FLAG_GRANT_READ_URI_PERMISSION = intent.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION");
                    intent.Call<AndroidJavaObject>("addFlags", FLAG_GRANT_READ_URI_PERMISSION);

                    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaObject unityContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

                    string packageName = unityContext.Call<string>("getPackageName");
                    string authority = packageName + ".fileprovider";

                    AndroidJavaObject fileObj = new AndroidJavaObject("java.io.File", textureUrl);
                    AndroidJavaClass fileProvider = new AndroidJavaClass("androidx.core.content.FileProvider");
                    AndroidJavaObject uri = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", unityContext, authority, fileObj);

                    intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.STREAM", uri);
                }
				var chooser = intent.CallStatic<AndroidJavaObject> ("createChooser", intent, "");
				chooser.Call<AndroidJavaObject> ("putExtra", "android.intent.extra.EXTRA_INITIAL_INTENTS", intent);
				activity.Call ("startActivity", chooser);
			}
		}
#endif

        public static void Share (string text)
		{
			Share (text, null, null);
		}

		public static void Share (string text, string url)
		{
			Share (text, url, null);
		}

		public static void Share (string text, string url, string textureUrl)
		{
#if UNITY_ANDROID || UNITY_IOS
			_Share (text, url, textureUrl);
#endif
		}
	}
}