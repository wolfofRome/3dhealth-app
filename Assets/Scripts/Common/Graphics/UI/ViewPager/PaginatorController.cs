using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.Graphics.UI.ViewPager
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class PaginatorController : BasePaginatorController
    {
        [SerializeField]
        private Image _prefabPagenatorImage = default;
        [SerializeField]
        private Color _normalColor = Color.white;
        [SerializeField]
        private Color _selectedColor = Color.red;

        private List<Image> _pagenatorImageList = new List<Image>();

        protected override void OnNotifyDataSetChanged()
        {
            foreach (var image in _pagenatorImageList)
            {
                Destroy(image.gameObject);
            }
            _pagenatorImageList.Clear();

            var curPos = currentPagePosition;
            var num = maxPageNum;
            for (int i=0; i<num; i++)
            {
                var image = Instantiate(_prefabPagenatorImage);
                image.color = (i == curPos ? _selectedColor : _normalColor);
                image.transform.SetParent(gameObject.transform, false);
                _pagenatorImageList.Add(image);
            }
        }

        protected override void OnPageChanged(int pagePosition)
        {
            var num = _pagenatorImageList.Count;
            for (int i = 0; i < num; i++)
            {
                _pagenatorImageList[i].color = (i == pagePosition ? _selectedColor : _normalColor);
            }
        }
    }

    public abstract class BasePaginatorController : MonoBehaviour
    {
        [SerializeField]
        private ViewPager _viewPager = default;
        [SerializeField]
        private BasePagerAdapter _pagerAdapter = default;

        protected virtual void Awake()
        {
            _viewPager.onPageChanged.AddListener(OnPageChanged);
            _pagerAdapter.OnNotifyDataSetChanged.AddListener(OnNotifyDataSetChanged);
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }
        
        protected int maxPageNum {
            get {
                return _pagerAdapter.GetCount();
            }
        }

        protected int currentPagePosition {
            get {
                return _viewPager.currentPagePosition;
            }
        }

        protected abstract void OnNotifyDataSetChanged();
        protected abstract void OnPageChanged(int pagePosition);
    }
}
