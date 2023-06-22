using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    [Serializable]
    public struct FontSizePresetParam
    {
        [SerializeField]
        private List<GameObject> _textContainerList;
        public List<GameObject> TextContainerList {
            get {
                return _textContainerList;
            }
            set {
                _textContainerList = value;
            }
        }

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
}
