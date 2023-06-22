using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    public class MeshColorController : BaseMeshController
    {
        [SerializeField]
        private List<Color> _colorList = new List<Color>()
        {
            Color.red,
            Color.magenta,
            Color.blue,
            Color.cyan,
            Color.green,
            Color.yellow,
            Color.red,
            Color.white,
        };
        private List<Color> colorList
        {
            get
            {
                return _colorList;
            }
        }

        private Color _color = Color.white;
        
        protected override void Start()
        {
            base.Start();
        }

        public override void OnValueChanged(float value)
        {
            var colorProgress = (colorList.Count - 1) * value;
            var min = (int)colorProgress;
            var max = Mathf.CeilToInt(colorProgress);
            _color = Color.Lerp(colorList[min], colorList[max], colorProgress - min);
            Apply();
        }

        public override void Apply()
        {
            foreach (RendererController rc in rendererControllerList)
            {
                rc.SetColor(null, _color.r, _color.g, _color.b);
            }
        }
    }
}