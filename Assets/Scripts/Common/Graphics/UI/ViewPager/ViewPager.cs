using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common.Graphics.UI.ViewPager
{
    [RequireComponent(typeof(SnapScrollRect))]
    public class ViewPager : MonoBehaviour
    {
        public enum PageScrollDirection
        {
            Horizontal,
            Vertical
        }

        [SerializeField]
        private PageScrollDirection _direction = default;
        public PageScrollDirection direction {
            get {
                return _direction;
            }
        }

        [SerializeField]
        private int _bufferingNum = 1;
        public int bufferingNum {
            get {
                return _bufferingNum;
            }
        }

        [Serializable]
        public class ViewPagerEvent : UnityEvent<int> { }
        [SerializeField]
        private ViewPagerEvent _onPageChanged = new ViewPagerEvent();
        public ViewPagerEvent onPageChanged {
            get {
                return _onPageChanged;
            }
        }

        [SerializeField]
        private BasePagerAdapter _pagerAdapter = default;
        public BasePagerAdapter adapter {
            get {
                return _pagerAdapter;
            }
        }

        [SerializeField]
        private int _defaultPagePosition = 0;
        protected int defaultPagePosition {
            get {
                return _defaultPagePosition;
            }
        }

        private Dictionary<int, GameObject> _itemStateList = new Dictionary<int, GameObject>();
        
        private int? _prevPagePosition = null;

        private SnapScrollRect _snapScrollRect;
        private SnapScrollRect snapScrollRect {
            get {
                return _snapScrollRect = _snapScrollRect ?? GetComponent<SnapScrollRect>();
            }
            set {
                _snapScrollRect = value;
            }
        }

        protected RectTransform contentRectTransform {
            get {
                return snapScrollRect.content;
            }
        }

        public void Invalidate()
        {
            snapScrollRect.Invalidate();
        }

        public int currentPagePosition {
            get {
                return _direction == PageScrollDirection.Horizontal ? snapScrollRect.curHorizontalPagePos : snapScrollRect.curVerticalPagePos;
            }
            set {
                if (_direction == PageScrollDirection.Horizontal)
                {
                    snapScrollRect.curHorizontalPagePos = value;
                }
                else
                {
                    snapScrollRect.curVerticalPagePos = value;
                }
            }
        }

        public void SetPagePosition(int position, bool smoothScroll)
        {
            if (_direction == PageScrollDirection.Horizontal)
            {
                snapScrollRect.SetScrollPosition(position, 0, smoothScroll);
            }
            else
            {
                snapScrollRect.SetScrollPosition(0, position, smoothScroll);
            }
        }

        public void ToNextPage()
        {
            currentPagePosition++;
        }

        public void ToPrevPage()
        {
            currentPagePosition--;
        }

        protected virtual void OnValidate()
        {
            if (_direction == PageScrollDirection.Horizontal)
            {
                snapScrollRect.vertical = false;
                snapScrollRect.verticalPages = 0;
                snapScrollRect.horizontal = true;
                snapScrollRect.horizontalPages = _pagerAdapter.GetCount();
            }
            else
            {
                snapScrollRect.vertical = true;
                snapScrollRect.verticalPages = _pagerAdapter.GetCount();
                snapScrollRect.horizontal = false;
                snapScrollRect.horizontalPages = 0;
            }
        }

        protected virtual void Awake()
        {
            _pagerAdapter.OnNotifyDataSetChanged.AddListener(OnNotifyDataSetChanged);
            snapScrollRect.onPositionChanged.AddListener(OnPositionChanged);
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }
        
        protected void FitToPageItem()
        {
            if (_direction == PageScrollDirection.Horizontal)
            {
                contentRectTransform.anchorMax = new Vector2(_pagerAdapter.GetCount(), contentRectTransform.anchorMax.y);
            }
            else
            {
                contentRectTransform.anchorMax = new Vector2(contentRectTransform.anchorMax.x, _pagerAdapter.GetCount());
            }
        }
        
        protected virtual void OnPositionChanged(int horizontalPagePos, int verticalPagePos)
        {
            var pagePosition = (direction == PageScrollDirection.Horizontal ? horizontalPagePos : verticalPagePos);

            // 前後のページを先読みする.
            BindBufferingItems(pagePosition);

            adapter.OnGotFocusItem(pagePosition);

            if (_prevPagePosition != null)
            {
                adapter.OnLostFocusItem((int)_prevPagePosition);
            }

            onPageChanged.Invoke(pagePosition);
            _prevPagePosition = pagePosition;
        }

        protected void BindBufferingItems(int pagePosition)
        {
            // 前後のページを先読みする.

            var itemList = adapter.GetItemList();
            int itemCount = itemList.Count;

            var min = Mathf.Max(pagePosition - bufferingNum, 0);
            var max = Mathf.Min(pagePosition + bufferingNum + 1, adapter.GetCount());

            for (int i = 0; i < itemCount; i++)
            {
                if (min <= i && i < max)
                {
                    // バインド
                    var anchor = GetItemAnchor(i, direction);
                    adapter.BindItem(i, anchor[0], anchor[1]);
                }
                else
                {
                    // アンバインド
                    adapter.UnBindItem(i);
                }
            }
        }

        protected Vector2[] GetItemAnchor(int pagePosition, PageScrollDirection direction)
        {
            var anchorSize = 1f / _pagerAdapter.GetCount();
            var anchor = new Vector2[2];
            if (direction == PageScrollDirection.Horizontal)
            {
                anchor[0] = new Vector2(pagePosition * anchorSize, contentRectTransform.anchorMin.y);
                anchor[1] = new Vector2((pagePosition + 1) * anchorSize, contentRectTransform.anchorMax.y);
            }
            else
            {
                anchor[0] = new Vector2(contentRectTransform.anchorMin.x, pagePosition * anchorSize);
                anchor[1] = new Vector2(contentRectTransform.anchorMax.x, (pagePosition + 1) * anchorSize);
            }
            //DebugUtil.Log(string.Format("[{0}]  min({1},{2}) miax({3},{4})", pagePosition, anchor[0].x, anchor[0].y, anchor[1].x, anchor[1].y));
            return anchor;
        }
        
        protected virtual void OnNotifyDataSetChanged()
        {
            if (_prevPagePosition != null)
            {
                adapter.OnLostFocusItem((int)_prevPagePosition);
            }

            _prevPagePosition = null;
            _itemStateList.Clear();
            FitToPageItem();
            
            var pageNum = _pagerAdapter.GetCount();
            for (var i = 0; i < pageNum; i++)
            {
                var anchor = GetItemAnchor(i, _direction);
                _itemStateList.Add(i, _pagerAdapter.InstantiateItem(i, anchor[0], anchor[1]));
            }

            if (_direction == PageScrollDirection.Horizontal)
            {
                snapScrollRect.verticalPages = 0;
                snapScrollRect.horizontalPages = pageNum;
                snapScrollRect.SetScrollPosition(_defaultPagePosition, 0, false);
            }
            else
            {
                snapScrollRect.verticalPages = pageNum;
                snapScrollRect.horizontalPages = 0;
                snapScrollRect.SetScrollPosition(0, _defaultPagePosition, false);
            }
        }
    }
}