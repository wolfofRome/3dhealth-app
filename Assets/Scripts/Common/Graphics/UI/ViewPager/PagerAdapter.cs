using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common.Graphics.UI.ViewPager
{
    public class PagerAdapter : BasePagerAdapter
    {
        [SerializeField]
        private List<BasePageFrame> _pageItemList = default;

        protected override void Awake()
        {
        }

        protected override void Start()
        {
        }

        protected override void Update()
        {
        }

        public override void ClearAllItems()
        {
            foreach (var item in _pageItemList)
            {
                Destroy(item.gameObject);
            }
            _pageItemList.Clear();
        }

        public override int AddItem(BasePageFrame item)
        {
            _pageItemList.Add(item);
            return _pageItemList.Count;
        }

        public override int GetCount()
        {
            return _pageItemList.Count;
        }

        public override List<BasePageFrame> GetItemList()
        {
            return _pageItemList;
        }

        public override GameObject InstantiateItem(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject ret = null;
            var frame = GetItem(pagePosition);
            if (frame != null)
            {
                ret = frame.Instantiate(pagePosition, anchorMin, anchorMax);
            }
            return ret;
        }

        public override void BindItem(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            var frame = GetItem(pagePosition);
            if (frame != null)
            {
                frame.Bind(pagePosition, anchorMin, anchorMax);
            }
        }

        public override void UnBindItem(int pagePosition)
        {
            var frame = GetItem(pagePosition);
            if (frame != null)
            {
                frame.UnBind(pagePosition);
            }
        }

        public override void OnGotFocusItem(int pagePosition)
        {
            var frame = GetItem(pagePosition);
            if (frame != null)
            {
                frame.OnGotFocus();
            }
        }

        public override void OnLostFocusItem(int pagePosition)
        {
            var frame = GetItem(pagePosition);
            if (frame != null)
            {
                frame.OnLostFocus();
            }
        }

        public override BasePageFrame GetItem(int pagePosition)
        {
            if (pagePosition >= _pageItemList.Count)
            {
                DebugUtil.LogWarning("is not exist target item.  list count -> " + _pageItemList.Count + ",  position -> " + pagePosition);
                return null;
            }
            return _pageItemList[pagePosition];
        }
    }

    public abstract class BasePagerAdapter : MonoBehaviour
    {
        private UnityEvent _onNotifyDataSetChanged = new UnityEvent();
        public UnityEvent OnNotifyDataSetChanged {
            get {
                return _onNotifyDataSetChanged;
            }
        }

        protected virtual void Awake(){}

        protected virtual void Start() {}

        protected virtual void Update() {}

        public abstract int GetCount();

        public abstract List<BasePageFrame> GetItemList();

        public abstract void ClearAllItems();

        public abstract int AddItem(BasePageFrame item);

        public abstract BasePageFrame GetItem(int pagePosition);

        public abstract GameObject InstantiateItem(int pagePosition, Vector2 anchorMin, Vector2 anchorMax);

        public abstract void BindItem(int pagePosition, Vector2 anchorMin, Vector2 anchorMax);

        public abstract void UnBindItem(int pagePosition);

        public abstract void OnGotFocusItem(int pagePosition);

        public abstract void OnLostFocusItem(int pagePosition);

        public virtual void NotifyDataSetChanged()
        {
            OnNotifyDataSetChanged.Invoke();
        }
    }
}