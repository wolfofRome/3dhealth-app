using Assets.Scripts.Common.Config;
using Assets.Scripts.Common.InputEvent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Common
{
    public class ObjTouchHandler : BaseTouchEventListener
    {
        public float Sensitivity = 2.0f;
        [Range(0.6f, 10.0f)]
        public float MaxScale = 10.0f;
        [Range(0.6f, 10.0f)]
        public float MinScale = 0.6f;
        public CameraController _cameraController;

        private bool _moveEnabled = true;
        public bool moveEnabled
        {
            get 
            {
                return _moveEnabled;
            }
            set
            {
                _moveEnabled = value;
            }
        }

        private bool _rotateEnabled = true;
        public bool rotateEnabled
        {
            get 
            {
                return _rotateEnabled;
            }
            set
            {
                _rotateEnabled = value;
            }
        }

        private bool _scaleEnabled = true;
        public bool scaleEnabled
        {
            get 
            {
                return _scaleEnabled;
            }
            set
            {
                _scaleEnabled = value;
            }
        }

        private Vector3 _prevPos0 = Vector3.zero;
        private Vector3 _prevPos1 = Vector3.zero;

        private float _prevPinchLength = 0;
        private float _curScale = 1.0f;
        
        private List<float> _presetScaleList = new List<float>();

        private void OnValidate()
        {
            MaxScale = Mathf.Max(MaxScale, MinScale);
            MinScale = Mathf.Min(MinScale, MaxScale);
        }

        protected override void Awake()
        {
        }

        protected override void Start()
        {
        }

        protected override void Update()
        {
        }

        public void Reset()
        {
            _cameraController.Reset();
            _moveEnabled = true;
            _rotateEnabled = true;
            _scaleEnabled = true;
            _curScale = 1;
        }

        public int AddPresetScale(float scale)
        {
            _presetScaleList.Add(scale);
            return _presetScaleList.Count - 1;
        }

        public void ApplyPresetScale(int index)
        {
            _curScale = SetScale(_cameraController, _presetScaleList[index]);
        }

        public void SetAngle(Quaternion rotation)
        {
            _cameraController.SetAngle(rotation);
        }

        public void SetCameraPosition(Vector3 position)
        {
            _cameraController.SetCameraPosition(position);
        }

        private float SetScale(CameraController camera, float reqScale)
        {
            float scale = Mathf.Clamp(reqScale, MinScale, MaxScale);
            camera.SetViewScale(scale);
            return scale;
        }

        private void Translate(CameraController camera, Vector3 prevPos, Vector3 curPos)
        {
            Vector3 prevViewportPoint = Camera.main.ScreenToViewportPoint(prevPos);
            Vector3 curViewportPoint = Camera.main.ScreenToViewportPoint(curPos);
            camera.TranslateByScreenCoordinate((prevViewportPoint - curViewportPoint) * Sensitivity);
        }

        private void Rotate(CameraController camera, float delta_x, float delta_y)
        {
            camera.Rotate(360f * delta_x / Screen.width * Sensitivity, 360f * delta_y / Screen.height * Sensitivity);
        }

        public override void OnBeginDrag(int pointerId, TouchEventData data)
        {
            switch (data.pointer.Count)
            {
                case 1:
                    _prevPos0 = data.pointer[pointerId].position;
                    break;
                case 2:
                    var points = data.pointer.Values.ToArray();
                    var pinchLength = Vector2.Distance(points[0].position, points[1].position);
                    _prevPinchLength = pinchLength;
                    _prevPos0 = points[0].position;
                    _prevPos1 = points[1].position;
                    break;
            }
        }

        public override void OnDrag(int pointerId, TouchEventData data)
        {
            switch (data.pointer.Count)
            {
                case 1:
                    var pos = data.pointer[pointerId].position;
                    if (_prevPos0 != Vector3.zero)
                    {
                        if (Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                        {
                            // move
                            if (_moveEnabled)
                            {
                                Translate(_cameraController, _prevPos0, pos);
                            }
                        }
                        else if (Input.GetMouseButton(1))
                        {
                            Vector2 delta = Camera.main.ScreenToViewportPoint(pos) - Camera.main.ScreenToViewportPoint(_prevPos0);
                            if (!Mathf.Approximately(Mathf.Abs(delta.x), 0))
                            {
                                // zoom in/out
                                if (_scaleEnabled)
                                {
                                    _curScale = SetScale(_cameraController, _curScale * (1 + delta.x * Sensitivity));
                                }
                            }
                        }
                        else
                        {
                            // rotate
                            if (_rotateEnabled)
                            {
                                float x_delta = pos.x - _prevPos0.x;
                                float y_delta = pos.y - _prevPos0.y;
                                Rotate(_cameraController, x_delta, y_delta);
                            }
                        }
                    }
                    _prevPos0 = pos;
                    break;
                case 2:
                    var points = data.pointer.Values.ToArray();

                    var pinchLength = Vector2.Distance(points[0].position, points[1].position);

                    if (_prevPos0 != Vector3.zero && _prevPos1 != Vector3.zero && _prevPinchLength > 0.0001f)
                    {
                        float deg0 = Mathf.Atan2(points[0].delta.y, points[0].delta.x) * Mathf.Rad2Deg;
                        float deg1 = Mathf.Atan2(points[1].delta.y, points[1].delta.x) * Mathf.Rad2Deg;
                        if (Mathf.Abs(deg0 - deg1) > 90)
                        {
                            // pinch in/out
                            if (_scaleEnabled)
                            {
                                _curScale = SetScale(_cameraController, _curScale * (pinchLength / _prevPinchLength));
                            }
                        }
                        else
                        {
                            // move
                            if (_moveEnabled)
                            {
                                Translate(_cameraController, (_prevPos0 + _prevPos1) / 2, (points[0].position + points[1].position) / 2);
                            }
                        }
                    }
                    _prevPinchLength = pinchLength;
                    _prevPos0 = points[0].position;
                    _prevPos1 = points[1].position;
                    break;
            }
        }

        public override void OnEndDrag(int pointerId, TouchEventData data)
        {
            _prevPos0 = Vector3.zero;
            _prevPos1 = Vector3.zero;
            _prevPinchLength = 0;
        }

        public override void OnPointerSingleClick(PointerEventData data)
        {
        }

        public override void OnPointerDoubleClick(PointerEventData data)
        {
        }
    }
}