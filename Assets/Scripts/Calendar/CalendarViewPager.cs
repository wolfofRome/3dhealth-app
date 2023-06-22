using Assets.Scripts.Common;
using Assets.Scripts.Common.Graphics.UI.ViewPager;
using Assets.Scripts.Network;
using Assets.Scripts.WebView;
using Assets.Scripts.Record;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Calendar
{
    [RequireComponent(typeof(ToggleGroup))]
    public class CalendarViewPager : LoopableViewPager
    {
        [SerializeField]
        private Text _yearPickerText = default;
        [SerializeField]
        private Text _monthPickerText = default;


        [SerializeField]
        private CalendarDataSource _dataSource = default;

        [SerializeField]
        private RecordMenuController _recordMenu = default;

        [SerializeField]
        private WebViewLoadParam _appointmentParam = default;

        private ToggleGroup _toggleGroup;
        public ToggleGroup toggleGroup
        {
            get
            {
                return _toggleGroup = _toggleGroup ?? GetComponent<ToggleGroup>();
            }
        }

        public override int deltaPagePosition
        {
            protected set
            {
                baseDate = baseDate.AddMonths(value - deltaPagePosition);
                base.deltaPagePosition = value;
            }
        }

        private DateTime _baseDate;
        public DateTime baseDate
        {
            get
            {
                return _baseDate;
            }
            set
            {
                _baseDate = value;
                _yearPickerText.text = string.Format("{0}年", value.Year);
                _monthPickerText.text = string.Format("{0}月", value.Month);
                _appointmentParam.WebViewArgument.OptionalParam = "selected=" + value.ToString(ApiHelper.DateFormat);

                var itemList = adapter.GetItemList();
                var first = baseDate.AddMonths(-itemList.Count / 2);
                var count = itemList.Count;
                // 前後のページのデータを更新.
                for (var i = 0; i < count; i++)
                {
                    var item = (MonthPageFrame)itemList[i];
                    item.date = first.AddMonths(i);
                    item.dataSource = _dataSource;
                    item.toggleGroup = toggleGroup;
                    item.recordMenu = _recordMenu;
                }
                StartCoroutine(_dataSource.UpdateDataSource(value));
            }
        }

        private Vector2 _calenderRectDefaultPosition;
        private Vector2 _calenderRectDefaultSizeDelta;

        protected override void Start()
        {
            base.Start();
            baseDate = DateTime.Today;
            adapter.NotifyDataSetChanged();
        }
        public void SetToDay()
        {
            adapter.NotifyDataSetChanged();
            baseDate = DateTime.Today;
        }
    }
}
