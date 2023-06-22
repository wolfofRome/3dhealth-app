using Assets.Scripts.Common.Graphics.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(SliderValueConverter))]
    public abstract class BaseMeshController : MonoBehaviour
    {
        private SliderValueConverter _valueConverter;
        private SliderValueConverter valueConverter {
            get {
                return _valueConverter = _valueConverter ?? GetComponent<SliderValueConverter>();
            }
        }

        private List<RendererController> _rendererControllerList = new List<RendererController>();
        protected List<RendererController> rendererControllerList {
            get {
                return _rendererControllerList;
            }
        }
        
        protected virtual void Start()
        {
            valueConverter.onValueChanged.AddListener(OnValueChanged);
            OnValueChanged(valueConverter.value);
        }

        protected virtual void Update()
        {
        }

        public void AddRendererController(RendererController renderer)
        {
            rendererControllerList.Add(renderer);
        }

        public void RemoveRendererController(RendererController renderer)
        {
            rendererControllerList.Remove(renderer);
        }

        public abstract void OnValueChanged(float value);
        
        public abstract void Apply();
    }
}