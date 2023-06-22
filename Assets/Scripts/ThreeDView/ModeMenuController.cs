using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Animators;
using Assets.Scripts.Network.Response;
using Assets.Scripts.ThreeDView.BodyAnalysis;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView {
    [RequireComponent(typeof(ToggleGroup))]
    public class ModeMenuController : FadeMenuController {
        [FormerlySerializedAs("_titleText")] [SerializeField]
        private Text titleText;
        [Serializable]
        public class ModeChangeEvent : UnityEvent<Mode> { }
        [FormerlySerializedAs("_onModeChanged")] [SerializeField]
        private ModeChangeEvent onModeChanged;

        [FormerlySerializedAs("_threeDMode")] [SerializeField]
        private CoordinateAnimationToggle threeDMode;

        [FormerlySerializedAs("_dataListMode")] [SerializeField]
        private CoordinateAnimationToggle dataListMode;

        [FormerlySerializedAs("_postureMode")] [SerializeField]
        private CoordinateAnimationToggle postureMode;

        [FormerlySerializedAs("_bodyAnalysisMode")] [SerializeField]
        private CoordinateAnimationToggle bodyAnalysisMode;

        [FormerlySerializedAs("_distortionMode")] [SerializeField]
        private CoordinateAnimationToggle distortionMode;

        [FormerlySerializedAs("_objImages")] [SerializeField]
        private List<RawImage> objImages;

        [FormerlySerializedAs("_backgroundCamera")] [SerializeField]
        private Camera backgroundCamera;

        [FormerlySerializedAs("_cameraController")] [SerializeField]
        private CameraController cameraController;

        [FormerlySerializedAs("_objTouchHandler")] [SerializeField]
        private ObjTouchHandler objTouchHandler;

        [FormerlySerializedAs("_threeDModeScale")] [SerializeField]
        private float threeDModeScale;

        [FormerlySerializedAs("_threeDModeAngle")] [SerializeField]
        private Vector3 threeDModeAngle;
        private int _presetScaleIndex;

        [FormerlySerializedAs("_dataSource")] [SerializeField]
        private ThreeDModeDataSource dataSource;

        [FormerlySerializedAs("_trialMenu")] [SerializeField]
        private FadeMenuController trialMenu;

        [FormerlySerializedAs("_contentsLoader")] [SerializeField]
        private ContentsLoader contentsLoader;

        private Dictionary<Mode, CoordinateAnimationToggle> _toggleMap;

        [Serializable]
        public class CoordinateAnimationToggle {
            [FormerlySerializedAs("_mode")] [SerializeField]
            private Mode mode;

            [FormerlySerializedAs("_toggle")] [SerializeField]
            private Toggle toggle;
            public Toggle Toggle => toggle;

            [FormerlySerializedAs("_activeObjectList")] [SerializeField]
            private List<GameObject> activeObjectList = new List<GameObject>();

            private Action<Mode> _modeChangeAction;

            public void OnValueChanged(bool isOn) {
                foreach (var obj in activeObjectList) {
                    // Mode切替時、アニメーションに対応しているObjectはアニメーション完了後にアクティブ／非アクティブを反映.
                    var animator = obj.GetComponent<CanvasFadeAnimator>();
                    if (animator != null) {
                        obj.SetActive(true);
                        if (isOn) {
                            animator.FadeIn();
                        } else {
                            animator.FadeOut();
                        }
                    } else {
                        obj.SetActive(isOn);
                    }
                }
                if (isOn) {
                    _modeChangeAction.Invoke(mode);
                }
            }

            public void Init(Action<Mode> modeChangeAction) {
                toggle.onValueChanged.AddListener(OnValueChanged);
                _modeChangeAction = modeChangeAction;

                foreach (var animator in activeObjectList.Select(obj => obj.GetComponent<CanvasFadeAnimator>()).Where(animator => animator != null))
                {
                    animator.onFinishAnimation.AddListener(OnFinishAnimation);
                }
            }

            public void OnFinishAnimation(GameObject go) {
                go.SetActive(toggle.isOn);
            }
        }

        private ToggleGroup _toggleGroup;

        private Contents _contents;

        private GameObject _threeDObject;

        [FormerlySerializedAs("_human")] [SerializeField]
        private BodyTypeSelector human;

        [FormerlySerializedAs("_bodyAnalysisController")] [SerializeField]
        private BodyAnalysisController bodyAnalysisController;
        private MarshmallowManController marshmallowMan => bodyAnalysisController.marshmallowMan;

        [FormerlySerializedAs("_blockman")] [SerializeField]
        private GameObject blockman;

        private bool _isAvatarUpdated;
        private bool _isRunning;

        protected override void Start() {
            base.Start();
            contentsLoader.onLoadStart.AddListener(OnLoadStart);

            threeDMode.Init(OnModeChanged);
            dataListMode.Init(OnModeChanged);
            postureMode.Init(OnModeChanged);
            bodyAnalysisMode.Init(OnModeChanged);
            distortionMode.Init(OnModeChanged);

            _presetScaleIndex = objTouchHandler.AddPresetScale(threeDModeScale);

            objTouchHandler.SetAngle(Quaternion.Euler(threeDModeAngle));
            objTouchHandler.ApplyPresetScale(_presetScaleIndex);

            SetObjImages(dataSource.mode);

            StartCoroutine(this.DelayFrame(10, () => {
                switch (dataSource.mode) {
                    case Mode.ThreeDView:
                        threeDMode.Toggle.isOn = true;
                        break;
                    case Mode.DataList:
                        dataListMode.Toggle.isOn = true;
                        break;
                    case Mode.Posture:
                        postureMode.Toggle.isOn = true;
                        break;
                    case Mode.BodyAnalysis:
                        bodyAnalysisMode.Toggle.isOn = true;
                        break;
                    case Mode.Distortion:
                        distortionMode.Toggle.isOn = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }));
        }

        /// <summary>
        /// モード切替イベント.
        /// </summary>
        /// <param name="mode"></param>
        private void OnModeChanged(Mode mode) {
            dataSource.mode = mode;
            titleText.text = mode.GetTitle();

            objTouchHandler.Reset();

            switch (mode) {
                case Mode.ThreeDView:
                    // サムネイルが生成されていない場合はサムネイルを生成.
                    StartCoroutine(WaitDataLoadProcess(obj => {
                        objTouchHandler.SetAngle(Quaternion.Euler(threeDModeAngle));
                        objTouchHandler.ApplyPresetScale(_presetScaleIndex);
                        SetActiveThreeDObj(mode);
                        contentsLoader.ScreenShot(_contents);
                    }));
                    human.ResetBoneColor();
                    trialMenu.Show();
                    break;
                case Mode.DataList:
                    StartCoroutine(WaitDataLoadProcess(obj => {
                        SetActiveThreeDObj(mode);
                        human.ResetBoneColor();
                    }));
                    trialMenu.Hide();
                    break;
                case Mode.Posture:
                    StartCoroutine(WaitDataLoadProcess(obj => {
                        SetActiveThreeDObj(mode);
                    }));
                    trialMenu.Show();
                    break;
                case Mode.BodyAnalysis:
                    objTouchHandler.moveEnabled = false;
                    objTouchHandler.scaleEnabled = false;
                    objTouchHandler.SetAngle(Quaternion.Euler(marshmallowMan.defaultRotation));
                    cameraController.SetViewScale(marshmallowMan.defaultScale);
                    bodyAnalysisController.OnUpdateBalloonLine();
                    StartCoroutine(WaitDataLoadProcess(obj => {
                        SetActiveThreeDObj(mode);
                    }));
                    trialMenu.Hide();
                    break;
                case Mode.Distortion:
                    StartCoroutine(WaitDataLoadProcess(obj => {
                        SetActiveThreeDObj(mode);
                        human.ResetBoneColor();
                    }));
                    trialMenu.Show();
                    break;
            }

            SetObjImages(mode);

            onModeChanged.Invoke(mode);
        }

        /// <summary>
        /// データのロード完了待機処理.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IEnumerator WaitDataLoadProcess(Action<GameObject> callback) {
            if (_isRunning) {
                yield break;
            }
            _isRunning = true;

            // 必要なデータのロード完了まで待機.
            while (!_isAvatarUpdated) {
                yield return null;
            }
            callback.Invoke(_threeDObject);

            _isRunning = false;
        }

        /// <summary>
        /// 3Dオブジェクト画像のテクスチャ座標設定.
        /// </summary>
        public void SetObjImages() {
            SetObjImages(dataSource.mode);
        }

        /// <summary>
        /// 3Dオブジェクト画像のテクスチャ座標設定.
        /// </summary>
        /// <param name="mode"></param>
        private void SetObjImages(Mode mode) {
            CameraClearFlags clearFlags;
            Rect uvRect;

            switch (mode) {
                case Mode.ThreeDView:
                    clearFlags = CameraClearFlags.Skybox;
                    uvRect = new Rect(0, -0.03f, 1, 1);
                    break;
                case Mode.DataList:
                    clearFlags = CameraClearFlags.SolidColor;
                    uvRect = new Rect(0.25f, 0, 1, 1);
                    break;
                case Mode.Posture:
                    clearFlags = CameraClearFlags.Skybox;
                    uvRect = new Rect(0, 0, 1, 1);
                    break;
                case Mode.BodyAnalysis:
                    clearFlags = CameraClearFlags.Skybox;
                    uvRect = new Rect(marshmallowMan.imageRectOffset.x, marshmallowMan.imageRectOffset.y, 1, 1);
                    break;
                case Mode.Distortion:
                    clearFlags = CameraClearFlags.Skybox;
                    uvRect = new Rect(0, 0, 1, 1);
                    break;
                default:
                    clearFlags = CameraClearFlags.Skybox;
                    uvRect = new Rect(0, 0, 1, 1);
                    break;
            }

            SetObjImages(clearFlags, uvRect);
        }

        /// <summary>
        /// 3Dオブジェクト画像のテクスチャ座標設定.
        /// </summary>
        private void SetObjImages(CameraClearFlags clearFlags, Rect uvRect) {
            backgroundCamera.clearFlags = clearFlags;
            foreach (var image in objImages) {
                image.uvRect = uvRect;
            }
        }

        /// <summary>
        /// 3Dオブジェクト画像のテクスチャ座標初期化設定.
        /// </summary>
        public void SetObjImagesReset() {
            SetObjImages(CameraClearFlags.Skybox, new Rect(0, 0, 1, 1));
        }

        /// <summary>
        /// 3Dオブジェクト表示切替処理.
        /// </summary>
        /// <param name="mode"></param>
        private void SetActiveThreeDObj(Mode mode) {
            switch (mode) {
                case Mode.ThreeDView:
                case Mode.DataList:
                case Mode.Posture:
                    _threeDObject.SetActive(true);
                    human.SetActive(true);
                    marshmallowMan.SetActive(false);
                    blockman.SetActive(false);
                    break;
                case Mode.BodyAnalysis:
                    _threeDObject.SetActive(false);
                    human.SetActive(false);
                    marshmallowMan.SetActive(true);
                    blockman.SetActive(false);
                    break;
                case Mode.Distortion:
                    _threeDObject.SetActive(false);
                    human.SetActive(false);
                    marshmallowMan.SetActive(false);
                    blockman.SetActive(true);
                    break;
                default:
                    _threeDObject.SetActive(false);
                    human.SetActive(false);
                    marshmallowMan.SetActive(false);
                    blockman.SetActive(false);
                    break;
            }
        }

        /// <summary>
        /// スキャンデータのロード開始イベント.
        /// </summary>
        /// <param name="contents"></param>
        public void OnLoadStart(Contents contents) {
            _contents = contents;
        }

        /// <summary>
        /// スキャンデータのロード完了イベント.
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="obj"></param>
        public void OnLoadCompleted(ObjLoader loader, GameObject obj) {
            _threeDObject = obj;
        }

        /// <summary>
        /// 人体のポーズ変形完了イベント.
        /// </summary>
        public void OnAvatarUpdated() {
            if (dataSource.mode == Mode.ThreeDView) {
                StartCoroutine(WaitDataLoadProcess(obj => {
                    objTouchHandler.SetAngle(Quaternion.Euler(threeDModeAngle));
                    objTouchHandler.ApplyPresetScale(_presetScaleIndex);
                }));
            }
            _isAvatarUpdated = true;
        }
    }
}