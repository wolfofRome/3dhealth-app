using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class OptimalRenderTextureController : RenderTextureController
    {
        private FPSHistory _fpsHistory;
        private const int FPSSamplingNum = 10;
        private const int LowResolutionSwitchFPS = 10;
        private const int HigResolutionSwitchhFPS = 30;

        private Dictionary<TextureType, RenderTexture> _renderTextureList;
        private TextureType _curTextureType = TextureType.HighResolution;
        private enum TextureType
        {
            HighResolution,
            LowResolution,
        }

        private class FPSHistory
        {
            private float[] _fpsList;
            private long _samplingCount;

            public FPSHistory(int historyNum)
            {
                _fpsList = new float[historyNum];
                _samplingCount = 0;
            }

            public void Clear()
            {
                _fpsList.Initialize();
                _samplingCount = 0;
            }

            public long SamplingCount {
                get {
                    return _samplingCount;
                }
            }

            public void Add(float fps)
            {
                long index = _samplingCount % _fpsList.Length;
                _fpsList[index] = fps;
                _samplingCount++;
            }

            public float Average()
            {
                var average = 0f;
                if (_samplingCount < _fpsList.Length)
                {
                    average = _fpsList.Take((int)_samplingCount).Average();
                }
                else
                {
                    average = _fpsList.Average();
                }
                return average;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            _fpsHistory.Add(1f / Time.deltaTime);

            if (_fpsHistory.SamplingCount >= FPSSamplingNum)
            {
                var fpsAverage = _fpsHistory.Average();
                if (_curTextureType == TextureType.HighResolution)
                {
                    // 高解像度表示で閾値を下回る場合は低解像度表示に切替.
                    if (LowResolutionSwitchFPS > fpsAverage)
                    {
                        SwitchTextureType(TextureType.LowResolution);
                    }
                }
                else
                {
                    // 低解像度表示で閾値を上回る場合は高解像度表示に切替.
                    if (fpsAverage > HigResolutionSwitchhFPS)
                    {
                        SwitchTextureType(TextureType.HighResolution);
                    }
                }
            }
        }

        protected override void InitRenderTexture()
        {
            var rect = targetCamera.pixelRect;
            _fpsHistory = new FPSHistory(FPSSamplingNum);
            _renderTextureList = new Dictionary<TextureType, RenderTexture>();
            _renderTextureList.Add(TextureType.HighResolution, CreateRenderTexture((int)rect.width, (int)rect.height));
            _renderTextureList.Add(TextureType.LowResolution, CreateRenderTexture((int)(rect.width / 2), (int)(rect.height / 2)));
            SwitchTextureType(_curTextureType);
        }

        private void SwitchTextureType(TextureType type)
        {
            var rt = _renderTextureList[type];
            targetImage.texture = rt;
            targetCamera.targetTexture = rt;
            targetCamera.rect = new Rect(0, 0, 1, 1);

            _fpsHistory.Clear();
            _curTextureType = type;
        }
    }
}
