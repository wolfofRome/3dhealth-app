using UnityEngine;

namespace Assets.Scripts.Common.Config
{
    public class ObjLoadConfig : ScriptableObject
    {
        [SerializeField]
        private Material _opaqueMaterial = default;
        public Material opaqueMaterial {
            get {
                return _opaqueMaterial;
            }
        }

        [SerializeField]
        private Material _fadeMaterial = default;
        public Material fadeMaterial {
            get {
                return _fadeMaterial;
            }
        }
    }
}
