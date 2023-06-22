using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private List<Camera> _cameraList = new List<Camera>();
        private Dictionary<Camera, DefaultCameraParam> _defaultCameraParams = new Dictionary<Camera, DefaultCameraParam>();

        private struct DefaultCameraParam
        {
            public Vector3 localPosition;
            public float fieldOfView;
            public Rect viewport;
        }

        private void Awake()
        {
            foreach (var camera in _cameraList)
            {
                _defaultCameraParams[camera] = new DefaultCameraParam()
                {
                    localPosition = camera.transform.localPosition,
                    fieldOfView = camera.fieldOfView,
                    viewport = camera.rect
                };
            }
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetViewPort(Rect viewport)
        {
            foreach (var camera in _cameraList)
            {
                camera.rect = viewport;
            }
        }

        public void Reset()
        {
            transform.localRotation = Quaternion.identity;

            foreach (var camera in _cameraList)
            {
                camera.fieldOfView = _defaultCameraParams[camera].fieldOfView;
                camera.transform.localPosition = _defaultCameraParams[camera].localPosition;
                camera.rect = _defaultCameraParams[camera].viewport;
            }
        }

        public void SetCameraClearFlags(CameraClearFlags flags)
        {
            foreach (var camera in _cameraList)
            {
                camera.clearFlags = flags;
            }
        }

        public void SetViewScale(float scale)
        {
            foreach (var camera in _cameraList)
            {
                camera.fieldOfView = _defaultCameraParams[camera].fieldOfView / scale;
            }
        }

        public void TranslateByScreenCoordinate(Vector3 moveValue)
        {
            // 拡縮率に合わせてスクロール量を調整.
            foreach (var camera in _cameraList)
            {
                float scale = camera.fieldOfView / _defaultCameraParams[camera].fieldOfView;
                camera.transform.Translate(moveValue * scale);
            }
        }

        public void Rotate(float deltaAngleX, float deltaAngleY)
        {
            transform.RotateAround(transform.position, Vector2.up, deltaAngleX);
            transform.RotateAround(transform.position, transform.right, deltaAngleY);
        }

        public void SetAngle(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        public void SetCameraPosition(Vector3 position)
        {
            foreach (var camera in _cameraList)
            {
                camera.transform.localPosition = position;
            }
        }
    }
}