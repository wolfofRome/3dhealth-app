using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.Graphics.UI.ListView
{
    public class GridViewController : MonoBehaviour
    {
        public enum ScrollDirection
        {
            Horizontal = 0,
            Vertical = 1
        }

        [SerializeField]
        private ScrollDirection _scrollDirection = ScrollDirection.Vertical;
        
        [SerializeField]
        private RectOffset _padding = default;
        protected RectOffset padding {
            get {
                return _padding;
            }
            set {
                _padding = value;
                _content.padding = value;
            }
        }

        [SerializeField]
        private int _columnNum = 3;

        [SerializeField]
        private int _rowNum = 3;

        [SerializeField]
        private GridLayoutGroup _content = default;

        [SerializeField]
        private BaseAdapter _adapter = default;
        
        private ScrollRect _scrollRect;
        protected ScrollRect scrollRect
        {
            get
            {
                return _scrollRect = _scrollRect ?? GetComponent<ScrollRect>();
            }
        }

        private RectTransform _contentRectTransform;
        private RectTransform contentRectTransform {
            get {
                return _contentRectTransform = _contentRectTransform ?? _content.GetComponent<RectTransform>();
            }
        }

        private Dictionary<int, GameObject> _childItemPool = new Dictionary<int, GameObject>();

        private float _cellSize;

        private float _spaceSize;

        private int _prevIndex;

        protected virtual void Start()
        {
            var root = transform.root.GetComponent<RectTransform>();
            var width = root.sizeDelta.x;
            var height = root.sizeDelta.y;

            _content.padding = _padding;
            _content.cellSize = new Vector2((width - _padding.horizontal) / _columnNum, (height - _padding.vertical) / _rowNum);

            if (_scrollDirection == ScrollDirection.Horizontal)
            {
                _content.startCorner = GridLayoutGroup.Corner.UpperLeft;
                _content.startAxis = GridLayoutGroup.Axis.Vertical;
                _content.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                _content.constraintCount = _rowNum;

                _cellSize = _content.cellSize.x;
                _spaceSize = _content.spacing.x;
            }
            else
            {
                _content.startCorner = GridLayoutGroup.Corner.UpperLeft;
                _content.startAxis = GridLayoutGroup.Axis.Horizontal;
                _content.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                _content.constraintCount = _columnNum;

                _cellSize = _content.cellSize.y;
                _spaceSize =  _content.spacing.y;
            }
            scrollRect.onValueChanged.AddListener(OnScroll);
            _adapter.OnNotifyDataSetChanged.AddListener(OnNotifyDataSetChanged);
        }

        protected virtual void Update()
        {
        }

        protected void OnScroll(Vector2 pos)
        {
            var index = GetLeftTopIndex();
            if (index != _prevIndex)
            {
                UpdateListItem();
            }
            _prevIndex = index;
        }

        protected virtual void OnNotifyDataSetChanged()
        {
            foreach (var item in _childItemPool)
            {
                Destroy(item.Value);
            }
            _childItemPool.Clear();

            ResizeContent();
            UpdateListItem();
        }

        private void ResizeContent()
        {
            var childCount = _adapter.GetCount();
            var scrollDirectionNum = Mathf.CeilToInt((float)childCount / _content.constraintCount);
            var contentSize = _cellSize * scrollDirectionNum + _spaceSize * (scrollDirectionNum - 1);

            var newSize = contentRectTransform.sizeDelta;
            if (_scrollDirection == ScrollDirection.Horizontal)
            {
                newSize.x = contentSize + _padding.horizontal;
            }
            else
            {
                newSize.y = contentSize + _padding.vertical;
            }
            contentRectTransform.sizeDelta = newSize;
        }

        public void UpdateListItem()
        {
            // 画面外のItem前後2列(行)分先読みする.
            var bufferingNum = (_scrollDirection == ScrollDirection.Horizontal ? _rowNum : _columnNum) * 2;
            var ltIndex = GetLeftTopIndex();
            var first = Math.Max(ltIndex - bufferingNum, 0);
            var last = Math.Min(ltIndex + (_columnNum * _rowNum) + bufferingNum, _adapter.GetCount());
            
            for (var i = first; i < last; i++)
            {
                if (!_childItemPool.ContainsKey(i) || _childItemPool[i] == null)
                {
                    var child = _adapter.InstantiateItem(i);
                    child.transform.SetParent(_content.transform, false);
                    _childItemPool.Add(i, child);
                }
                _adapter.Bind(_childItemPool[i], i);
            }
        }
        
        private int GetLeftTopIndex()
        {
            int index;
            var pos = contentRectTransform.anchoredPosition;
            if (_scrollDirection == ScrollDirection.Horizontal)
            {
                index = (int)(pos.x / _cellSize) * _rowNum;
            }
            else
            {
                index = (int)(pos.y / _cellSize) * _columnNum;
            }
            return index >= 0 ? index : 0;
        }
    }
}