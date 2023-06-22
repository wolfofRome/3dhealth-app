using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Compare
{
    [RequireComponent(typeof(RectTransform))]
    public class CompareObjImage : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private RectTransform rectTransform
        {
            get
            {
                return _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
            }
        }

        // DistanceSliderのOnValueChangedイベントからset呼び出し(Inspector設定)
        public float anchorOffsetX
        {
            set
            {
                Vector2 anchorMin = rectTransform.anchorMin;
                anchorMin.x = value;
                rectTransform.anchorMin = anchorMin;

                Vector2 anchorMax = rectTransform.anchorMax;
                anchorMax.x = value + 1.0f;
                rectTransform.anchorMax = anchorMax;
            }
        }
    }
}
