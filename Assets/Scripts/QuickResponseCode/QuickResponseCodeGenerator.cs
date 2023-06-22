using System;
using System.Collections;
using Assets.Scripts.Common;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZXing;
using ZXing.QrCode;

namespace QuickResponseCode
{
    public class QuickResponseCodeGenerator : MonoBehaviour {

        private string _bodyId;

        private Texture2D _targetTexture;
        protected static Sprite QrSprite;

        private bool _isGenerated = false;

        // Use this for initialization
        private void Start ()
        {
            StartCoroutine(WaitForLogin(() =>
            {
                if (DataManager.Instance.isTrialAccount)
                    return;
                User user = DataManager.Instance.Cache.GetUser(UserData.AutoLoginUserId);
                _bodyId = user.LoginId;

                _targetTexture = QrCodeHelper.CreateQrCode(_bodyId, 256, 256);
                QrSprite = Sprite.Create(_targetTexture, new Rect(27, 28, 200, 200), Vector2.zero);
            }));
        }

        public void LoadQRcodeScene()
        {
            SceneManager.LoadScene("QRcode");
        }

        private IEnumerator WaitForLogin(Action action)
        {
            if (_isGenerated)
                yield break;
            
            while (!ApiHelper.Instance.isLogin)
            {
                yield return null;
            }

            _isGenerated = true;
            action();
        }
    }

    public static class QrCodeHelper
    {
        public static Texture2D CreateQrCode(string str, int w, int h)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            Color32[] content = Write(str, w, h);
            tex.SetPixels32(content);
            tex.Apply();
            return tex;
        }

        private static Color32[] Write(string content, int w, int h)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = w,
                    Height = h
                }
            };
            return writer.Write(content);
        }
    }
}