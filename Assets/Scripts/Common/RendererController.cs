using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(Renderer))]
    public class RendererController : MonoBehaviour
    {
        private struct MaterialParam
        {
            public Shader shader;
            public Color color;
        }

        private List<MaterialParam> _baseMaterialParamList;
        private List<MaterialParam> baseMaterialParamList {
            get {
                if (_baseMaterialParamList == null)
                {
                    _baseMaterialParamList = new List<MaterialParam>();
                    foreach (var material in Renderer.materials)
                    {
                        var param = new MaterialParam();
                        param.color = material.color;
                        param.shader = material.shader;
                        _baseMaterialParamList.Add(param);
                    }
                }
                return _baseMaterialParamList; 
            }
        }
        
        private Renderer _renderer;
        public Renderer Renderer {
            get {
                return _renderer = _renderer ?? GetComponent<Renderer>();
            }
        }

        private Color _flatModeColor = Color.gray;
        public Color FlatModeColor {
            get {
                return _flatModeColor;
            }
            set {
                _flatModeColor = value;
            }
        }

        private Shader _flatModeShader;
        private Shader FlatModeShader {
            get {
                return _flatModeShader = _flatModeShader ?? Resources.Load<Material>("FlatModeMaterial").shader;
            }
        }
        
        void Start()
        {
        }
        
        void Update()
        {
        }

        public virtual void SetAlpha(float alpha)
        {
            var i = 0;
            var baseMatParam = baseMaterialParamList;
            var materials = Renderer.materials;
            foreach (Material material in materials)
            {
                // 元のアルファ値をベースにして変更する
                var color = material.color;
                var baseAlpha = baseMatParam[i].color.a;
                color.a = baseAlpha * alpha;
                if (Mathf.Approximately(color.a, 1f))
                {
                    MaterialUtil.SetBlendMode(material, MaterialUtil.BlendMode.Opaque);
                }
                else {
                    MaterialUtil.SetBlendMode(material, MaterialUtil.BlendMode.Fade);
                }
                material.SetColor("_Color", color);
                i++;
            }
        }

        /// <summary>
        /// フラットモードのON/OFF切替
        /// </summary>
        /// <param name="enable"></param>
        public virtual void SetEnableFlatMode(bool enable)
        {
            var i = 0;
            var baseMatParam = baseMaterialParamList;
            var materials = Renderer.materials;
            foreach (var material in materials)
            {
                var color = material.GetColor("_Color");
                if (enable)
                {
                    // FlatModeのシェーダーに変更
                    _flatModeColor.a = color.a;
                    material.shader = FlatModeShader;
                    material.SetColor("_Color", _flatModeColor);
                }
                else {
                    // シェーダーを元に戻し、アルファ値を反映
                    var baseParam = baseMatParam[i];
                    baseParam.color.a = color.a;
                    material.shader = baseParam.shader;
                    material.SetColor("_Color", baseParam.color);
                }
                i++;
            }
        }
        
        public virtual void SetColor(string materialName, float r, float g, float b)
        {
            SetColor(materialName, "_Color", r, g, b);
        }

        public virtual void SetSpecularColor(string materialName, float r, float g, float b)
        {
            SetColor(materialName, "_SpecColor", r, g, b);
        }

        private void SetColor(string materialName, string propertyName, float r, float g, float b)
        {
            var materials = Renderer.materials;
            if (!string.IsNullOrEmpty(materialName))
            {
                materials = Renderer.materials.Where(mat => mat.name.StartsWith(materialName)).ToArray();
            }
            foreach (Material material in materials)
            {
                var color = material.GetColor(propertyName);
                color.r = r;
                color.g = g;
                color.b = b;
                material.SetColor(propertyName, color);
            }
        }
    }
}