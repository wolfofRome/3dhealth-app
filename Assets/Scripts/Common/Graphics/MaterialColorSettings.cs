using UnityEngine;

namespace Assets.Scripts.Graphics
{
    public class MaterialColorSettings
    {
        private Color _albedoColor;
        public Color albedoColor
        {
            get
            {
                return _albedoColor;
            }
        }

        private Color _specularColor;
        public Color specularColor
        {
            get
            {
                return _specularColor;
            }
        }

        public MaterialColorSettings(Color albedoColor, Color specularColor)
        {
            _albedoColor = albedoColor;
            _specularColor = specularColor;
        }
    }
}