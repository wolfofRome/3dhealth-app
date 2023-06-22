using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Graphics.UI
{
    public class MultiImageButton : Button
    {
         private Graphic[] _graphics;
         protected Graphic[] graphics {
            get {
                return _graphics = _graphics ?? targetGraphic.GetComponentsInChildren<Graphic>();
            }
        }
 
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            Color color;
            switch (state)
            {
            case SelectionState.Normal:
                color = colors.normalColor;
                break;
            case SelectionState.Highlighted:
                color = colors.highlightedColor;
                break;
            case SelectionState.Pressed:
                color = colors.pressedColor;
                break;
            case SelectionState.Disabled:
                color = colors.disabledColor;
                break;
            default:
                color = Color.black;
                break;
            }
            if (gameObject.activeInHierarchy)
            {
                switch (transition)
                {
                case Transition.ColorTint:
                    ColorTween (color * colors.colorMultiplier, instant);
                    break;
                default:
                    throw new NotSupportedException();
                }
            }
        }
 
        private void ColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
            {
                return;
            }
 
            foreach(Graphic g in graphics)
            {
                g.CrossFadeColor (targetColor, (!instant) ? colors.fadeDuration : 0f, true, true);
            }
        }
    }
}