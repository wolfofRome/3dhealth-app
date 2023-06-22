using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Common.InputEvent;

namespace Assets.Scripts.ThreeDView.Distortion
{
    public class BlockmanTouchHandler : BaseTouchEventListener
    {
        [Serializable]
        public class MeshTouchEvent : UnityEvent<BlockmanPart, RaycastHit> {}

        [SerializeField]
        private MeshTouchEvent _onClick = default;
        public MeshTouchEvent onClick {
            get {
                return _onClick;
            }
        }
        
        [SerializeField]
        private Camera _camera = default;

        [SerializeField]
        private float _clickTimeThreshold = 0.3f;

        [SerializeField]
        private GameObject _bone = default;

        private BlockmanPartController[] _blockmanPartList;
        private BlockmanPartController[] blockmanPartList
        {
            get
            {
                return _blockmanPartList = _blockmanPartList ?? _bone.GetComponentsInChildren<BlockmanPartController>();
            }
        }
        
        private RaycastHit _hitInfo = new RaycastHit();

        private float _beginDragTime;

        private bool _isLongTouch;
        
        public override void OnBeginDrag(int pointerId, TouchEventData data)
        {
            _beginDragTime = Time.time;
        }

        public override void OnDrag(int pointerId, TouchEventData data)
        {
        }

        public override void OnEndDrag(int pointerId, TouchEventData data)
        {
            _isLongTouch = (Time.time - _beginDragTime) > _clickTimeThreshold;
        }

        public override void OnPointerDoubleClick(PointerEventData data)
        {
        }

        public override void OnPointerSingleClick(PointerEventData data)
        {
            if (_isLongTouch)
            {
                _isLongTouch = false;
                return;
            }

            foreach (var item in blockmanPartList)
            {
                if (item.boxCollider.Raycast(_camera.ScreenPointToRay(data.position), out _hitInfo, _camera.farClipPlane))
                {
                    onClick.Invoke(item.part, _hitInfo);
                }
            }
        }

    }
}
