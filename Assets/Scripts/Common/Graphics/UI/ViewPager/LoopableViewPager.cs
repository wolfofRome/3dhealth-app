using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common.Graphics.UI.ViewPager
{
    public class LoopableViewPager : ViewPager
    {
        [Serializable]
        public class LoopableViewPagerEvent : UnityEvent<int> { }

        [SerializeField]
        private LoopableViewPagerEvent _onDeltaPagePositionChanged = new LoopableViewPagerEvent();
        public LoopableViewPagerEvent onDeltaPagePositionChanged {
            get {
                return _onDeltaPagePositionChanged;
            }
        }

        private int _deltaPagePosition;
        public virtual int deltaPagePosition {
            get {
                return _deltaPagePosition;
            }
            protected set {
                _deltaPagePosition = value;
                onDeltaPagePositionChanged.Invoke(deltaPagePosition);
            }
        }

        private int? _prevPagePosition = null;

        protected override void OnPositionChanged(int horizontalPagePos, int verticalPagePos)
        {
            base.OnPositionChanged(horizontalPagePos, verticalPagePos);

            var pagePosition = (direction == PageScrollDirection.Horizontal ? horizontalPagePos : verticalPagePos);
            
            if (_prevPagePosition != null)
            {
                var scrollDelta = pagePosition - defaultPagePosition;
                if (scrollDelta > 0)
                {
                    StartCoroutine(this.DelayFrame(1, () =>
                    {
                        SetAsLastSibling(0);
                        SetPagePosition(defaultPagePosition, false);
                        deltaPagePosition += scrollDelta;
                        
                        BindBufferingItems(defaultPagePosition);
                    }));
                }
                else if (scrollDelta < 0)
                {
                    StartCoroutine(this.DelayFrame(1, () =>
                    {
                        SetAsFirstSibling(adapter.GetCount() - 1);
                        SetPagePosition(defaultPagePosition, false);
                        deltaPagePosition += scrollDelta;
                        
                        BindBufferingItems(defaultPagePosition);
                        
                    }));
                }
            }
            else
            {
                // 前後のページを先読みする.
                BindBufferingItems(pagePosition);

                adapter.OnGotFocusItem(pagePosition);

                if (_prevPagePosition != null)
                {
                    adapter.OnLostFocusItem((int)_prevPagePosition);
                }
            }
            _prevPagePosition = pagePosition;
        }
        
        private void Loop(int itemPosition)
        {
        }

        private void SetAsLastSibling(int itemPosition)
        {
            var item = adapter.GetItem(itemPosition);
            adapter.GetItemList().RemoveAt(itemPosition);
            adapter.GetItemList().Add(item);
            item.transform.SetAsLastSibling();
        }

        private void SetAsFirstSibling(int itemPosition)
        {
            var item = adapter.GetItem(itemPosition);
            adapter.GetItemList().RemoveAt(itemPosition);
            adapter.GetItemList().Insert(0, item);
            item.transform.SetAsFirstSibling();
        }
    }
}