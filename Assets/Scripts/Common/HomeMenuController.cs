using System;
using Assets.Scripts.Common.Animators;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(SlideAnimator))]
    public class HomeMenuController : ButtonGroupController
    {
        [FormerlySerializedAs("_memoButton")] [SerializeField]
        private Button memoButton;
        [FormerlySerializedAs("_lineModeButton")] [SerializeField]
        private Button lineModeButton;
        [FormerlySerializedAs("_displayMenuButton")] [SerializeField]
        private Button displayMenuButton;
        [FormerlySerializedAs("_defaultPoseButton")] [SerializeField]
        private Button defaultPoseButton;
        [FormerlySerializedAs("_dataListButton")] [SerializeField]
        private Button dataListButton;
        
        protected override List<Button> buttonList {
            get {
                if (_buttonList == null)
                {
                    _buttonList = new List<Button>();
                }
                if (_buttonList.Count != 0) return _buttonList;
                if (memoButton != null) _buttonList.Add(memoButton);
                if (lineModeButton != null) _buttonList.Add(lineModeButton);
                if (displayMenuButton != null) _buttonList.Add(displayMenuButton);
                if (defaultPoseButton != null) _buttonList.Add(defaultPoseButton);
                if (dataListButton != null) _buttonList.Add(dataListButton);

                return _buttonList;
            }
        }

        [FormerlySerializedAs("_dataListModeAnchorMin")] [SerializeField]
        private Vector2 dataListModeAnchorMin;
        [FormerlySerializedAs("_dataListModeAnchorMax")] [SerializeField]
        private Vector2 dataListModeAnchorMax;

        private Vector2 _anchorMinDefault;
        private Vector2 _anchorMaxDefault;

        [SerializeField] private Button standardValueTableButton;

        protected override void Start()
        {
            base.Start();
            _anchorMinDefault = rectTransform.anchorMin;
            _anchorMaxDefault = rectTransform.anchorMax;
        }

        /// <summary>
        /// モード変更時の表示切替処理.
        /// </summary>
        /// <param name="mode"></param>
        public void OnModeChanged(Mode mode)
        {
            standardValueTableButton.gameObject.SetActive(false);
            switch (mode)
            {
                case Mode.ThreeDView:
                    rectTransform.anchorMin = _anchorMinDefault;
                    rectTransform.anchorMax = _anchorMaxDefault;
                    buttonList.ForEach(button => { button.gameObject.SetActive(true); });
                    break;
                case Mode.DataList:
                    rectTransform.anchorMin = dataListModeAnchorMin;
                    rectTransform.anchorMax = dataListModeAnchorMax;
                    buttonList.ForEach(button => { button.gameObject.SetActive(false); });
                    standardValueTableButton.gameObject.SetActive(true);
                    break;
                case Mode.Posture:
                    rectTransform.anchorMin = _anchorMinDefault;
                    rectTransform.anchorMax = _anchorMaxDefault;
                    buttonList.ForEach(button => { button.gameObject.SetActive(false); });
                    displayMenuButton.gameObject.SetActive(true);
                    break;
                case Mode.BodyAnalysis:
                    rectTransform.anchorMin = _anchorMinDefault;
                    rectTransform.anchorMax = _anchorMaxDefault;
                    buttonList.ForEach(button => { button.gameObject.SetActive(false); });
                    break;
                case Mode.Distortion:
                    rectTransform.anchorMin = _anchorMinDefault;
                    rectTransform.anchorMax = _anchorMaxDefault;
                    buttonList.ForEach(button => { button.gameObject.SetActive(false); });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}