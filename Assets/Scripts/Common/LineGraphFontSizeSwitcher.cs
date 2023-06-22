using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(WMG_Axis_Graph))]
    public class LineGraphFontSizeSwitcher : MonoBehaviour
    {
        [SerializeField]
        private FontSizePresetParam _xAxisLabelSize;
        [SerializeField]
        private FontSizePresetParam _yAxisLabelSize;

        [Serializable]
        private struct FontSizePresetParam
        {
            [SerializeField]
            private int _webGLFontSize;
            public int WebGLFontSize {
                get {
                    return _webGLFontSize;
                }
                set {
                    _webGLFontSize = value;
                }
            }

            [SerializeField]
            private int _mobileFontSize;
            public int MobileFontSize {
                get {
                    return _mobileFontSize;
                }
                set {
                    _mobileFontSize = value;
                }
            }
        }

        void Awake()
        {
            var graph = GetComponent<WMG_Axis_Graph>();
#if UNITY_WEBGL
            graph.xAxis.AxisLabelSize = _xAxisLabelSize.WebGLFontSize;
            graph.yAxis.AxisLabelSize = _yAxisLabelSize.WebGLFontSize;
#else
            graph.xAxis.AxisLabelSize = _xAxisLabelSize.MobileFontSize;
            graph.yAxis.AxisLabelSize = _yAxisLabelSize.MobileFontSize;
#endif
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
