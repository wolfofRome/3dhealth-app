using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Common.Graphics.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderValueConverter : MonoBehaviour
    {
        [Serializable]
        public class SliderValueConverterEvent : UnityEvent<float> { }

        [SerializeField]
        private SliderValueConverterEvent _onValueChanged = default;
        public SliderValueConverterEvent onValueChanged {
            get {
                return _onValueChanged;
            }
        }

        [SerializeField]
        private float _scale = 1f;
        private float scale
        {
            get
            {
                return _scale;
            }
        }

        private Slider _slider;
        private Slider slider {
            get {
                return _slider = _slider ?? GetComponent<Slider>();
            }
        }

        public float value {
            get {
                return Normalize(slider.value, slider.minValue, slider.maxValue) * scale;
            }
        }
        
        protected virtual void Start()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            OnSliderValueChanged(slider.value);
        }

        protected virtual void Update()
        {
        }

        private void OnSliderValueChanged(float orgValue)
        {
            onValueChanged.Invoke(Normalize(orgValue, slider.minValue, slider.maxValue) * scale);
        }

        private static float Normalize(float src, float min, float max)
        {
            return Mathf.Clamp((src - min) / (max - min), 0f, 1f);
        }
    }
}