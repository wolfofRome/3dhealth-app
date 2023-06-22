using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView
{
    public class DefaultPoseController : MonoBehaviour
    {
        [SerializeField]
        private Camera _objCamera = default;

        [SerializeField]
        public Vector3 _objCameraOffset = Vector3.zero;
        
        [SerializeField]
        private ObjController _objController = default;

        [SerializeField]
        private ObjTouchHandler _touchHandler = default;
        
        [SerializeField]
        private GameObject _frontAngleBone = default;

        [SerializeField]
        private GameObject _sideAngleBone = default;
        
        [SerializeField]
        private Slider _defaultPosePhysiqueAlphaSlider = default;
        private Slider defaultPosePhysiqueAlphaSlider
        {
            get
            {
                return _defaultPosePhysiqueAlphaSlider;
            }
        }

        [SerializeField]
        private Slider _physiqueAlphaSlider = default;
        private Slider physiqueAlphaSlider
        {
            get
            {
                return _physiqueAlphaSlider;
            }
        }

        [SerializeField]
        private Slider _fatAlphaSlider = default;
        private Slider fatAlphaSlider
        {
            get
            {
                return _fatAlphaSlider;
            }
        }

        [SerializeField]
        private Slider _muscleAlphaSlider = default;
        private Slider muscleAlphaSlider
        {
            get
            {
                return _muscleAlphaSlider;
            }
        }

        [SerializeField]
        private ToggleGroup _boneControllToggleGroup = default;
        private ToggleGroup boneControllToggleGroup
        {
            get
            {
                return _boneControllToggleGroup;
            }
        }

        [SerializeField]
        private float _defaultPhysiqueSliderValue = default;
        private float defaultPhysiqueSliderValue
        {
            get
            {
                return _defaultPhysiqueSliderValue;
            }
        }

        private RectTransform _rectTransform;
        private RectTransform rectTransform {
            get {
                return _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
            }
        }

        private bool _anyTogglesOn = false;
        private bool anyTogglesOn
        {
            get
            {
                return _anyTogglesOn;
            }
            set
            {
                _anyTogglesOn = value;
            }
        }

        private int? _presetIndex = null;
        
        private float? _prevPhysiqueAlphaValue = null;
        private float? _prevFatAlphaValue = null;
        private float? _prevMuscleAlphaValue = null;

        void Start()
        {
        }
        
        void Update()
        {
        }

        /// <summary>
        /// 人体のポーズ変形完了イベント.
        /// </summary>
        public void OnAvatarUpdated()
        {
            var objBounds = _objController.objBounds;
            var objSize = _objCamera.WorldToViewportPoint(new Vector3(objBounds.size.x, objBounds.size.y, objBounds.center.z)) - _objCamera.WorldToViewportPoint(Vector3.zero);
            var boneImageSize = Camera.main.ScreenToViewportPoint(rectTransform.rect.size * transform.root.GetComponent<Canvas>().scaleFactor);
            var scale = boneImageSize.y / objSize.y;

            _presetIndex = _touchHandler.AddPresetScale(scale);
            _objCameraOffset.y = (_objCameraOffset.y + _objController.objBounds.min.y) * scale;
        }

        /// <summary>
        /// 3Dモデルの透過度を骨格表示モードのデフォルト値に設定する.
        /// </summary>
        public void SetDefaultAlphaValues()
        {
            _prevPhysiqueAlphaValue = physiqueAlphaSlider.value;
            _prevFatAlphaValue = fatAlphaSlider.value;
            _prevMuscleAlphaValue = muscleAlphaSlider.value;

            defaultPosePhysiqueAlphaSlider.value = _defaultPhysiqueSliderValue;
            fatAlphaSlider.value = 0;
            muscleAlphaSlider.value = 0;
        }

        /// <summary>
        /// 3Dモデルの透過度設定を骨格表示モード前の値に戻す.
        /// </summary>
        public void RestoreAlphaValues()
        {
            if (_prevPhysiqueAlphaValue != null)
            {
                physiqueAlphaSlider.value = (float)_prevPhysiqueAlphaValue;
                defaultPosePhysiqueAlphaSlider.value = (float)_prevPhysiqueAlphaValue;
            }

            if (_prevFatAlphaValue != null)
            {
                fatAlphaSlider.value = (float)_prevFatAlphaValue;
            }

            if (_prevMuscleAlphaValue != null) {
                muscleAlphaSlider.value = (float)_prevMuscleAlphaValue;
            }
        }

        /// <summary>
        /// 前アングルの骨格画像の表示.
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActiveFrontAngleBone(bool isActive)
        {
            _frontAngleBone.SetActive(isActive);
            if (isActive)
            {
                AdjustToObj(0, _objCameraOffset);

                if (!anyTogglesOn)
                {
                    physiqueAlphaSlider.value = _defaultPhysiqueSliderValue;
                    fatAlphaSlider.value = 0;
                    muscleAlphaSlider.value = 0;
                }
            }
            anyTogglesOn = boneControllToggleGroup.AnyTogglesOn();
        }

        /// <summary>
        /// 横アングルの骨格画像の表示.
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActiveSideAngleBone(bool isActive)
        {
            _sideAngleBone.SetActive(isActive);
            if (isActive)
            {
                AdjustToObj(-90, _objCameraOffset);
                
                if (!anyTogglesOn)
                {
                    physiqueAlphaSlider.value = _defaultPhysiqueSliderValue;
                    fatAlphaSlider.value = 0;
                    muscleAlphaSlider.value = 0;
                }
            }
            anyTogglesOn = boneControllToggleGroup.AnyTogglesOn();
        }
        
        /// <summary>
        /// 骨格と3Dモデルの表示サイズを合わせる.
        /// </summary>
        /// <param name="angleY"></param>
        /// <param name="offset"></param>
        private void AdjustToObj(float angleY, Vector3 offset)
        {
            if (_presetIndex != null)
            {
                _touchHandler.ApplyPresetScale((int)_presetIndex);
            }
            _touchHandler.SetAngle(Quaternion.Euler(0, angleY, 0));
            _touchHandler.SetCameraPosition(offset);
        }
    }
}
