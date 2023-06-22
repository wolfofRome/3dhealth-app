using Assets.Scripts.Common;
using Assets.Scripts.Common.Util;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Compare
{
    [RequireComponent(typeof(Toggle))]
    public class FitModeController : MonoBehaviour
    {
        [SerializeField]
        private Transform _rightFbx = default;
        private Transform rightFbx
        {
            get
            {
                return _rightFbx;
            }
        }

        private Transform _leftObject;
        private Transform leftObject
        {
            get
            {
                return _leftObject;
            }
            set
            {
                _leftObject = value;
            }
        }
        
        private Transform _rightObject;
        private Transform rightObject
        {
            get
            {
                return _rightObject;
            }
            set
            {
                _rightObject = value;
            }
        }

        private Toggle _fitModeToggle;
        private Toggle fitModeToggle
        {
            get
            {
                return _fitModeToggle = _fitModeToggle ?? GetComponent<Toggle>();
            }
        }

        private bool _isRunning = false;
        private bool isRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
            }
        }
        
        private bool? _prevFitModeEnable = null;
        private bool? prevFitModeEnable
        {
            get
            {
                return _prevFitModeEnable;
            }
            set
            {
                _prevFitModeEnable = value;
            }
        }

        private void Start()
        {
            fitModeToggle.onValueChanged.AddListener( isOn =>
            {
                Apply(isOn);
            });

            StartCoroutine(WaitForDataLoadProcess(() =>
            {
                Apply(fitModeToggle.isOn);
            }));
        }

        IEnumerator WaitForDataLoadProcess(Action action)
        {
            if (isRunning)
            {
                yield break;
            }
            isRunning = true;

            // データロード完了まで待機.
            while (leftObject == null || rightObject == null)
            {
                yield return null;
            }

            action();

            isRunning = false;
        }

        public void OnLoadLeftObject(ObjLoader loader, GameObject obj)
        {
            leftObject = obj.transform;
        }

        public void OnLoadRightObject(ObjLoader loader, GameObject obj)
        {
            rightObject = obj.transform;
        }

        private void Apply(bool fitModeEnable)
        {
            if (fitModeEnable == prevFitModeEnable)
            {
                return;
            }

            if (fitModeEnable)
            {
                var leftBounds = RendererUtil.GetEncapsulateBounds(leftObject.GetComponentsInChildren<Renderer>());
                var rightBounds = RendererUtil.GetEncapsulateBounds(rightObject.GetComponentsInChildren<Renderer>());
                var scale = Vector3.one * (leftBounds.size.y / rightBounds.size.y);

                rightObject.localScale = scale;
                rightFbx.localScale = scale;
            }
            else
            {
                rightObject.localScale = Vector3.one;
                rightFbx.localScale = Vector3.one;
            }

            prevFitModeEnable = fitModeEnable;
        }
    }
}