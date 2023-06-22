using Assets.Scripts.Common.Config;
using Assets.Scripts.Common.Graphics.UI.ViewPager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView.Posture
{
    public class PostureViewPager : ViewPager
    {
        [SerializeField]
        private ToggleGroup _toggleGroup = default;

        [SerializeField]
        private List<Toggle> _toggleList = default;
        
        private int activeToggleIndex {
            get {
                return _toggleList.IndexOf(_toggleGroup.ActiveToggles().FirstOrDefault());
            }
            set {
                if (_toggleList.Count > value)
                {
                    _toggleList[value].isOn = true;
                }
            }
        }

        private Dictionary<PostureVerifyPoint, PostureVerifyer.Result> _results;

        protected override void Start()
        {
            base.Start();
            foreach (var toggle in _toggleList)
            {
                toggle.onValueChanged.AddListener(onValueChanged);
            }
            adapter.NotifyDataSetChanged();
        }

        protected override void OnPositionChanged(int horizontalPagePos, int verticalPagePos)
        {
            base.OnPositionChanged(horizontalPagePos, verticalPagePos);

            var pagePos = (direction == PageScrollDirection.Horizontal ? horizontalPagePos : verticalPagePos);
            UpdatePagePosition(pagePos);
        }
        
        private void onValueChanged(bool isOn)
        {
            if (isOn)
            {
                UpdatePagePosition(activeToggleIndex);
            }
        }

        private void UpdatePagePosition(int pagePosition)
        {
            if (currentPagePosition != pagePosition)
            {
                currentPagePosition = pagePosition;
            }

            if (activeToggleIndex != pagePosition)
            {
                activeToggleIndex = pagePosition;
            }
        }
    }
}
