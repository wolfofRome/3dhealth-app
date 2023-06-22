using Assets.Scripts.Common.Config;
using Assets.Scripts.ThreeDView;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class MeshAlphaController : BaseMeshController
    {
        [SerializeField]
        private List<BodyTypeSelector> _bodyTypeSelectorList = new List<BodyTypeSelector>();

        [SerializeField]
        private List<Camera> _objCameraList = default;

        [SerializeField]
        protected LayerMask _cullingMaskForObj = default;
        protected virtual LayerMask cullingMaskForObj
        {
            get
            {
                return _cullingMaskForObj;
            }
        }

        [SerializeField]
        protected LayerMask _cullingMaskForAll = default;
        protected virtual LayerMask cullingMaskForAll
        {
            get
            {
                return _cullingMaskForAll;
            }
        }


        [SerializeField]
        private bool _isUpdateHumanLayerActivity = true;

        private float _alpha = 1.0f;
        private float _baseAlpha = 1.0f;

        public void AddObjCamera(Camera camera)
        {
            camera.cullingMask = Mathf.Approximately(_baseAlpha * _alpha, 1f) ? _cullingMaskForObj : _cullingMaskForAll;
            _objCameraList.Add(camera);
        }

        public void RemoveObjCamera(Camera camera)
        {
            _objCameraList.Remove(camera);
        }

        public override void OnValueChanged(float alpha)
        {
            _alpha = alpha;
            Apply();
        }

        public void SetBaseAlpha(float baseAlpha)
        {
            _baseAlpha = baseAlpha;
            Apply();
        }

        public override void Apply()
        {
            float alpha = _baseAlpha * _alpha;

            foreach (var rc in rendererControllerList)
            {
                rc.SetAlpha(alpha);
            }
            // 筋肉等がはみ出すため、OBJが透過なしの場合、FBXを非表示にする.
            var isObjActive = Mathf.Approximately(alpha, 1f);
            
            if (_isUpdateHumanLayerActivity)
            {
                foreach (var _selector in _bodyTypeSelectorList)
                {
                    _selector.SetActive(!isObjActive);
                }
            }

            _objCameraList.ForEach(c => c.cullingMask = (isObjActive ? cullingMaskForObj : cullingMaskForAll));
        }
    }
}