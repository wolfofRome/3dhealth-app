 using UnityEngine;
 using UnityEngine.UI;
 using UnityEngine.Events;
 using UnityEngine.EventSystems;
 using System;

namespace Assets.Scripts.Graphics.UI
{
    [AddComponentMenu("Aeronauts/MultifunctionalButton")]
    public class MultifunctionalButton : Button
    {
        [SerializeField]
        MultifunctionalButtonEvent _onDown = new MultifunctionalButtonEvent();
        
        [SerializeField]
        MultifunctionalButtonEvent _onUp = new MultifunctionalButtonEvent();
        
        protected MultifunctionalButton() { }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
    
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
    
            _onDown.Invoke();
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
    
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
    
            _onUp.Invoke();
        }
        
        public MultifunctionalButtonEvent onDown
        {
            get { return _onDown; }
            set { _onDown = value; }
        }
        
        public MultifunctionalButtonEvent onUp
        {
            get { return _onUp; }
            set { _onUp = value; }
        }
        
        [Serializable]
        public class MultifunctionalButtonEvent : UnityEvent { }
    }
}