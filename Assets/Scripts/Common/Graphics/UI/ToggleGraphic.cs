using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Graphics.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleGraphic : MonoBehaviour
    {
        [SerializeField]
        protected Color _onColor = Color.white;
        public Color onColor
        {
            get
            {
                return _onColor;
            }
            set
            {
                _onColor = value;
                UpdateColor(toggle.isOn);
            }
        }

        [SerializeField]
        protected Color _offColor = Color.blue;
        public Color offColor
        {
            get
            {
                return _offColor;
            }
            set
            {
                _offColor = value;
                UpdateColor(toggle.isOn);
            }
        }

        [SerializeField]
        private List<Graphic> _toggleGraphicList = new List<Graphic>();

        private Toggle _toggle;
        protected Toggle toggle {
            get {
                return _toggle = _toggle ?? GetComponent<Toggle>();
            }
        }

        private void OnValidate()
        {
            UpdateColor(toggle.isOn);
        }

        public virtual void Start()
        {
            UpdateColor(toggle.isOn);
            toggle.onValueChanged.AddListener(UpdateColor);
        }
        
        public virtual void Update()
        {
        }

        public void UpdateColor(bool isOn)
        {
            foreach (var graphic in _toggleGraphicList)
            {
                graphic.color = _toggle.isOn ? onColor : offColor;
            }
        }
    }
}