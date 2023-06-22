using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Common;
using System.Collections.Generic;

namespace Assets.Scripts.ThreeDView
{
    [RequireComponent(typeof(Toggle))]
    public class GridModeController : MonoBehaviour
    {
        [SerializeField]
        private List<Camera> _cameraList = default;

        // Use this for initialization
        void Start()
        {
            OnValueChanged(GetComponent<Toggle>().isOn);
        }

        public void OnValueChanged(bool isOn)
        {
            if (isOn)
            {
                foreach (var camera in _cameraList)
                {
                    camera.clearFlags = CameraClearFlags.Skybox;
                }
            }
            else
            {
                foreach (var camera in _cameraList)
                {
                    camera.clearFlags = CameraClearFlags.SolidColor;
                }
            }
        }
    }
}