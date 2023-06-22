using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(Camera))]
    public class RenderTextureController : MonoBehaviour
    {
        [SerializeField]
        private RawImage _targetImage = default;
        public RawImage targetImage {
            get {
                return _targetImage;
            }
            set {
                _targetImage = value;
            }
        }

        [SerializeField]
        private bool _enableManualUpdate = false;

        private Camera _camera;
        public Camera targetCamera {
            get {
                return _camera = _camera ?? GetComponent<Camera>();
            }
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
            InitRenderTexture();
        }

        protected virtual void Update()
        {
        }

        public virtual void UpdateRenderTexture()
        {
            targetCamera.enabled = true;
        }

        protected virtual void OnPostRender()
        {
            if (_enableManualUpdate)
            {
                targetCamera.enabled = false;
            }
        }

        protected RenderTexture CreateRenderTexture(int width, int height)
        {
            var rt = new RenderTexture(width, height, 24);
            rt.enableRandomWrite = false;
            return rt;
        }

        protected virtual void InitRenderTexture()
        {
            var rect = targetCamera.pixelRect;
            var rt = CreateRenderTexture((int)rect.width, (int)rect.height);
            targetImage.texture = rt;
            targetCamera.targetTexture = rt;
            targetCamera.rect = new Rect(0, 0, 1, 1);
        }
    }
}
