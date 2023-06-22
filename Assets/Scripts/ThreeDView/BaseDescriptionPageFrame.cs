using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Graphics.UI.ViewPager;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView
{
    [RequireComponent(typeof(RawImage))]
    public abstract class BaseDescriptionPageFrame : PageFrame
    {
        public PostureVerifyer.Result Result { get; set; }

        public Transform CameraParent { get; set; }

        [FormerlySerializedAs("_prefabObjRenderTextureController")] [SerializeField]
        private RenderTextureController prefabObjRenderTextureController;
        protected RenderTextureController PrefabObjRenderTextureController => prefabObjRenderTextureController;

        protected RenderTextureController objRenderTextureController { get; set; }

        private RawImage _objImage;
        protected RawImage objImage {
            get {
                return _objImage = _objImage ? _objImage : GetComponent<RawImage>();
            }
        }

        protected const float CameraPositionOffsetY = 0.2f;

        public override GameObject Instantiate(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            return base.Instantiate(pagePosition, anchorMin, anchorMax);
        }

        protected static Vector3[] TransformCoordinate(Vector3[] worldPoints, Camera srcCamera, Camera dstCamera)
        {
            return worldPoints.Select(point => TransformCoordinate(point, srcCamera, dstCamera)).ToArray();
        }

        private static Vector3 TransformCoordinate(Vector3 worldPoint, Camera srcCamera, Camera dstCamera)
        {
            return dstCamera.ViewportToWorldPoint(srcCamera.WorldToViewportPoint(worldPoint));
        }
    }
}
