using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class ScreenShooter : MonoBehaviour
    {
        void Start()
        {
        }

        void Update()
        {
        }

        public void SaveScreenshot(string savePath, Camera camera, Action<string> callback)
        {
            StartCoroutine(Save(savePath, camera, callback));
        }

        IEnumerator Save(string savePath, Camera camera, Action<string> callback)
        {
            yield return new WaitForEndOfFrame();

            // Resources/Sprites/loading.pngと同じアスペクト比でキャプチャを作成する
            var prevTexture = camera.targetTexture;
            var prevRect = camera.pixelRect;

            int baseWidth = 128;
            int baseHeight = 256;

            Texture2D capture = new Texture2D(baseWidth, baseHeight, TextureFormat.ARGB32, false);
            RenderTexture rt = new RenderTexture(baseWidth, baseHeight, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = rt;
            camera.pixelRect = new Rect(0, 0, baseWidth, baseHeight);
            camera.Render();

            RenderTexture.active = rt;

            var pixelRect = camera.pixelRect;
            int width = (int)pixelRect.width;
            int height = (int)pixelRect.height;
            int offsetX = (int)(pixelRect.center.x - (pixelRect.width / 2));
            int offsetY = (int)(pixelRect.center.y - (pixelRect.height / 2));
            capture.ReadPixels(new Rect(offsetX, offsetY, width, height), 0, 0);
            capture.Apply();

            camera.targetTexture = prevTexture;
            camera.pixelRect = prevRect;

            using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
            {
                writer.Write(capture.EncodeToPNG());
                writer.Flush();
            }

            if (callback != null)
            {
                callback(savePath);
            }
        }
    }
}
