using Assets.Scripts.Common;
using Assets.Scripts.Common.Animators;
using Assets.Scripts.Network.Response;
using Assets.Scripts.ThreeDView;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.TalentDetail
{
    public class ModeController : MonoBehaviour
    {
        [Serializable]
        public class ModeChangeEvent : UnityEvent<Mode> { }
        [SerializeField]
        private ModeChangeEvent _onModeChanged = default;
        public ModeChangeEvent onModeChanged
        {
            get
            {
                return _onModeChanged;
            }
        }

        [SerializeField]
        private CoordinateAnimationButton _threeDMode = default;

        [SerializeField]
        private CoordinateAnimationButton _dataListMode = default;

        [SerializeField]
        private List<RawImage> _objImages = default;

        [SerializeField]
        private Camera _backgroundCamera = default;

        [SerializeField]
        private ObjTouchHandler _objTouchHandler = default;
        
        [SerializeField]
        private float _threeDModeScale = default;

        [SerializeField]
        private Vector3 _threeDModeAngle = default;
        private int _presetScaleIndex;

        [SerializeField]
        private GameObject _purchaseButtonArea = default;

        private TalentoContents _talent;

        private Dictionary<Mode, CoordinateAnimationButton> _toggleMap;

        [Serializable]
        public class CoordinateAnimationButton
        {
            [SerializeField]
            private Button _button = default;
            public Button button
            {
                get
                {
                    return _button;
                }
            }
            
            [SerializeField]
            private List<GameObject> _activeObjectList = new List<GameObject>();

            private bool _isOn;
            public bool isOn
            {
                get
                {
                    return _isOn;
                }
                set
                {
                    _isOn = value;

                    foreach (var obj in _activeObjectList)
                    {
                        // Mode切替時、アニメーションに対応しているObjectはアニメーション完了後にアクティブ／非アクティブを反映.
                        var animator = obj.GetComponent<CanvasFadeAnimator>();
                        if (animator != null)
                        {
                            obj.SetActive(true);
                            if (value)
                            {
                                animator.FadeIn();
                            }
                            else
                            {
                                animator.FadeOut();
                            }
                        }
                        else
                        {
                            obj.SetActive(value);
                        }
                    }
                }
            }

            public void Init()
            {
                foreach (var obj in _activeObjectList)
                {
                    var animator = obj.GetComponent<CanvasFadeAnimator>();
                    if (animator != null)
                    {
                        animator.onFinishAnimation.AddListener(OnFinishAnimation);
                    }
                }
            }

            public void OnFinishAnimation(GameObject go)
            {
                go.SetActive(go.GetComponent<CanvasFadeAnimator>().isShown);
            }
        }
        
        /// <summary>
        /// モード設定処理.
        /// </summary>
        /// <param name="mode"></param>
        [EnumAction(typeof(Mode))]
        public void SetMode(int mode)
        {
            SetMode((Mode)mode);
        }

        /// <summary>
        /// モード設定処理.
        /// </summary>
        /// <param name="mode"></param>
        public void SetMode(Mode mode)
        {
            var clearFlags = CameraClearFlags.Skybox;
            var uvRect = new Rect(0, 0, 1, 1);

            _objTouchHandler.Reset();

            switch (mode)
            {
                case Mode.DataList:
                    _threeDMode.isOn = false;
                    _dataListMode.isOn = true;
                    clearFlags = CameraClearFlags.SolidColor;
                    uvRect = new Rect(0.25f, 0, 1, 1);
                    _purchaseButtonArea.gameObject.SetActive(false);
                    break;
                case Mode.ThreeDView:
                    _threeDMode.isOn = true;
                    _dataListMode.isOn = false;
                    _objTouchHandler.SetAngle(Quaternion.Euler(_threeDModeAngle));
                    _objTouchHandler.ApplyPresetScale(_presetScaleIndex);
                    _purchaseButtonArea.gameObject.SetActive(_talent.purchaseStatus != TalentList.PurchaseStatus.Purchased);
                    break;
                default:
                    break;

            }

            _backgroundCamera.clearFlags = clearFlags;
            foreach (var image in _objImages)
            {
                image.uvRect = uvRect;
            }

            _onModeChanged.Invoke(mode);
        }

        protected void Start()
        {
            _threeDMode.Init();
            _dataListMode.Init();

            _presetScaleIndex = _objTouchHandler.AddPresetScale(_threeDModeScale);
            _objTouchHandler.SetAngle(Quaternion.Euler(_threeDModeAngle));
            _objTouchHandler.ApplyPresetScale(_presetScaleIndex);


            var args = SceneLoader.Instance.GetArgment<TalentDetailLoadParam.Argment>(gameObject.scene.name);
            _talent = args.talentInfo;
        }
    }
}