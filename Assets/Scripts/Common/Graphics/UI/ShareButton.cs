using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Loading;
using Assets.Scripts.Common;

namespace Assets.Scripts.ThreeDView
{
    [RequireComponent(typeof(Button))]
    public class ShareButton : MonoBehaviour
    {
        [SerializeField]
        private Sprite spriteIos = default;

        [SerializeField]
        private Sprite spriteAndroid = default;

        private Button _button;
        private Button button
        {
            get
            {
                return _button = _button ?? GetComponent<Button>();
            }
        }

        private Image _image;
        private Image image
        {
            get
            {
                return _image = _image ?? (Image)button.targetGraphic;
            }
        }

        private const string imageFileName = "ShareImage.png";
        private string imageFilePath
        {
            get
            {
#if UNITY_IPHONE || UNITY_ANDROID
                return Application.persistentDataPath + "/" + imageFileName;
#else
                return imageFileName;
#endif
            }
        }

        private void Awake()
        {
#if UNITY_IPHONE
            image.sprite = spriteIos;
#else
            image.sprite = spriteAndroid;
#endif
        }

        public void OnClick()
        {
            if (File.Exists(imageFilePath))
            {
                File.Delete(imageFilePath);
            }

            StartCoroutine(Share());
        }

        private IEnumerator Share()
        {
#if !UNITY_IOS && !UNITY_ANDROID
            yield break;
#endif

            // インジケーター表示
            LoadingSceneManager.Instance.Show();

            // スクリーンショットをとる
            ScreenCapture.CaptureScreenshot(imageFileName);

            // スクリーンショットが保存されるまで待機
            long filesize = 0;
            while (filesize == 0)
            {
                yield return null;

                if (File.Exists(imageFilePath))
                {
                    //ファイルのサイズを取得
                    FileInfo fi = new FileInfo(imageFilePath);
                    if (fi != null)
                    {
                        filesize = fi.Length;
                    }
                }
            }

            // インジケーター非表示
            LoadingSceneManager.Instance.Hide();

            // shareのテキスト内容
            string text = "";
            string url = "";

            // 共有
            SocialConnector.SocialConnector.Share(text, url, imageFilePath);
        }
    }
}
