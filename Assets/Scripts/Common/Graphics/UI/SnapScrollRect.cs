using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

namespace Assets.Scripts.Common.Graphics.UI
{
    public class SnapScrollRect : ScrollRect
    {
        [Serializable]
        public class SnapScrollEvent : UnityEvent<int, int> { }
        [SerializeField]
        private SnapScrollEvent _onPositionChanged = new SnapScrollEvent();
        public SnapScrollEvent onPositionChanged {
            get {
                return _onPositionChanged;
            }
        }

        [SerializeField]
        private int _horizontalPages = 0;
        public int horizontalPages {
            get {
                return _horizontalPages;
            }
            set {
                _horizontalPages = value;
            }
        }

        [SerializeField]
        private int _verticalPages = 0;
        public int verticalPages {
            get {
                return _verticalPages;
            }
            set {
                _verticalPages = value;
            }
        }

        private const float PageSwitchThresholdMin = 0f;
        private const float PageSwitchThresholdMax = 0.5f;

        [Range(PageSwitchThresholdMin, PageSwitchThresholdMax)]
        private float _pageSwitchThreshold = 0.48f;
        public float pageSwitchThreshold {
            get {
                return _pageSwitchThreshold;
            }
            set {
                _pageSwitchThreshold = value;
            }
        }

        private float _smooth = 10f;
        public float smooth {
            get {
                return _smooth;
            }
            set {
                _smooth = value;
            }
        }

        private int? _curHorizontalPagePos;
        public int curHorizontalPagePos {
            get {
                return _curHorizontalPagePos ?? 0;
            }
            set {
                SetScrollPosition(value, 0, true);
            }
        }

        private int? _curVerticalPagePos;
        public int curVerticalPagePos {
            get {
                return _curVerticalPagePos ?? 0;
            }
            set {
                SetScrollPosition(0, value, true);
            }
        }

        private Vector2 _targetPosition;

        private bool _isDragging = false;
        
        protected override void Start()
        {
            base.Start();
        }

        void Update()
        {
            if (!_isDragging && normalizedPosition != _targetPosition)
            {
                if (Vector2.Distance(normalizedPosition, _targetPosition) > 0.002f)
                {
                    normalizedPosition = Vector2.Lerp(normalizedPosition, _targetPosition, smooth * Time.deltaTime);
                }
                else
                {
                    normalizedPosition = _targetPosition;
                }
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            _isDragging = true;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            _targetPosition = FindSnapPosition();
            _isDragging = false;
        }

        public void SetScrollPosition(int horizontalPagePos, int verticalPagePos, bool smoothScroll)
        {
            horizontalPagePos = Mathf.Clamp(horizontalPagePos, 0, Math.Max(horizontalPages - 1, 0));
            verticalPagePos = Mathf.Clamp(verticalPagePos, 0, Math.Max(verticalPages - 1, 0));

            _targetPosition = CalcPosition(horizontalPagePos, verticalPagePos);
            if (!smoothScroll)
            {
                normalizedPosition = _targetPosition;
            }

            if (_curHorizontalPagePos != horizontalPagePos || _curVerticalPagePos != verticalPagePos)
            {
                _curHorizontalPagePos = horizontalPagePos;
                _curVerticalPagePos = verticalPagePos;
                onPositionChanged.Invoke(horizontalPagePos, verticalPagePos);
            }
        }

        public void Invalidate()
        {
           onPositionChanged.Invoke(curHorizontalPagePos, curVerticalPagePos);
        }

        public void ResetScrollPosition()
        {
            SetScrollPosition(0, 0, false);
        }

        Vector2 CalcPosition(int horizontalPagePos, int verticalPagePos)
        {
            float x = Mathf.Clamp01(horizontalPagePos / (horizontalPages - 1f));
            float y = Mathf.Clamp01(verticalPagePos / (verticalPages - 1f));
            return new Vector2(x, y);
        }

        Vector2 FindSnapPosition()
        {
            float x = 0, y = 0;
            int horizontalPagePos = 0;
            int verticalPagePos = 0;

            if (horizontal)
            {
                if (horizontalPages > 1)
                {
                    float newPosition = normalizedPosition.x * (horizontalPages - 1f);
                    int page = (int)(newPosition + 0.5f);
                    float decimalPart = newPosition - curHorizontalPagePos;
                    float thres = PageSwitchThresholdMax - pageSwitchThreshold;
                    if (decimalPart > thres)
                    {
                        page = (int)Math.Min(Mathf.Ceil(newPosition), (horizontalPages - 1f));
                    }
                    else if (decimalPart < -thres)
                    {
                        page = (int)newPosition;
                    }
                    
                    x = page / (horizontalPages - 1f);
                    horizontalPagePos = page;
                }
            }
            else
            {
                x = normalizedPosition.x;
            }

            if (vertical)
            {
                if (verticalPages > 1)
                {
                    float newPosition = normalizedPosition.y * (verticalPages - 1f);
                    int page = (int)(newPosition + 0.5f);
                    float decimalPart = newPosition - curVerticalPagePos;
                    float thres = PageSwitchThresholdMax - pageSwitchThreshold;

                    if (decimalPart > thres)
                    {
                        page = (int)Math.Min(Mathf.Ceil(newPosition), (verticalPages - 1f));
                    }
                    else if (decimalPart < -thres)
                    {
                        page = (int)newPosition;
                    }

                    y = page / (verticalPages - 1f);
                    verticalPagePos = page;
                }
            }
            else
            {
                y = normalizedPosition.y;
            }

            if (_curHorizontalPagePos != horizontalPagePos || _curVerticalPagePos != verticalPagePos)
            {
                _curHorizontalPagePos = horizontalPagePos;
                _curVerticalPagePos = verticalPagePos;
                onPositionChanged.Invoke(horizontalPagePos, verticalPagePos);
            }

            return new Vector2(x, y);
        }
    }
}