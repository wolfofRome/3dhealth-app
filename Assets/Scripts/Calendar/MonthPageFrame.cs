using UnityEngine;
using Assets.Scripts.Common.Graphics.UI.ViewPager;
using System.Collections.Generic;
using System;
using System.Collections;
using Assets.Scripts.Network;
using Assets.Scripts.Common;
using Assets.Scripts.Network.Response;
using System.Linq;
using Assets.Scripts.Record;
using UnityEngine.UI;

namespace Assets.Scripts.Calendar
{
    public class MonthPageFrame : PageFrame
    {
        [SerializeField]
        private Week _prefabWeek = default;

        private DateTime _date;
        public DateTime date {
            get {
                return _date;
            }
            set {
                _date = value;
            }
        }

        private DateTime _bindedDate;
        private DateTime bindedDate {
            get {
                return _bindedDate;
            }
            set {
                _bindedDate = value;
            }
        }

        private RecordMenuController _recordMenu;
        public RecordMenuController recordMenu {
            get {
                return _recordMenu;
            }
            set {
                _recordMenu = value;
            }
        }

        private CalendarDataSource _dataSource;
        public CalendarDataSource dataSource {
            get {
                return _dataSource;
            }
            set {
                _dataSource = value;
            }
        }

        private ToggleGroup _toggleGroup;
        public ToggleGroup toggleGroup {
            get {
                return _toggleGroup;
            }
            set {
                _toggleGroup = value;
            }
        }

        private List<Week> _weekList;

        private Dictionary<string, Day> _availableDayMap = new Dictionary<string, Day>();

        private bool _isRunningWaitProcess = false;

        public override GameObject Instantiate(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            base.Instantiate(pagePosition, anchorMin, anchorMax);

            if (_weekList == null)
            {
                _weekList = new List<Week>();
                for (var i = 0; i < CalendarConfig.MaxWeekOfMonth; i++)
                {
                    var week = Instantiate(_prefabWeek);
                    week.transform.SetParent(transform, false);
                    _weekList.Add(week);
                }
            }

            return gameObject;
        }

        public override void Bind(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            base.Bind(pagePosition, anchorMin, anchorMax);

            if (date.Date != bindedDate.Date)
            {
                var monthDays = DateTime.DaysInMonth(date.Year, date.Month);
                var firstDate = date.BeginOfMonth();

                var dayOfWeekList = Enum.GetValues(typeof(DayOfWeek));
                var dayCount = 0;

                _availableDayMap.Clear();

                // 日付の設定.
                foreach (var w in _weekList)
                {
                    var start = ((int)(firstDate.DayOfWeek) + dayCount) % dayOfWeekList.Length;
                    var end = Mathf.Clamp(dayOfWeekList.Length, 0, monthDays - dayCount);
                    foreach (int dw in dayOfWeekList)
                    {
                        var dayObject = w.days[(DayOfWeek)dw];
                        if (start <= dw && dw < end)
                        {
                            var dateTime = firstDate.AddDays(dayCount++);
                            dayObject.date = dateTime;
                            dayObject.toggle.interactable = true;
                            dayObject.toggle.group = toggleGroup;
                            _availableDayMap[dateTime.ToString(ApiHelper.DateFormat)] = dayObject;
                        }
                        else
                        {
                            dayObject.toggle.interactable = false;
                            dayObject.toggle.group = null;
                            dayObject.date = null;
                        }
                        dayObject.info = null;
                        dayObject.onToggleValueChanged.RemoveListener(OnToggleValueChanged);
                    }
                }

                // APIで取得したデータの設定.
                StartCoroutine(WaitDataLoadProcess(() =>
                {
                    ApplyDataSource();
                }));
            }

            bindedDate = date;
        }

        public override void OnGotFocus()
        {
            base.OnGotFocus();
            dataSource.onUpdate.AddListener(OnUpdateDataSource);

            // 日付選択中にページ遷移した場合は次の月の1日を選択状態にする.
            if (toggleGroup.AnyTogglesOn())
            {
                var toggle = toggleGroup.ActiveToggles().FirstOrDefault();
                var day = toggle.gameObject.GetComponent<Day>();
                if (date.Month != ((DateTime)day.date).Month)
                {
                    var key = date.BeginOfMonth().ToString(ApiHelper.DateFormat);
                    _availableDayMap[key].toggle.isOn = true;
                }
            }
        }

        public override void OnLostFocus()
        {
            base.OnLostFocus();
            dataSource.onUpdate.RemoveListener(OnUpdateDataSource);
        }

        private void OnUpdateDataSource(string[] dateList, CalendarDataSource source)
        {
            ApplyDataSource();
        }

        private void ApplyDataSource()
        {
            foreach (var item in _availableDayMap)
            {
                var dayObject = item.Value;

                CalendarInfo info;
                dayObject.info = dataSource.infoMap.TryGetValue(item.Key, out info) ? info : null;

                if (dayObject.date != null)
                {
                    dayObject.contents = DataManager.Instance.ContentsList.
                        Where(x => x.CreateTimeAsDateTime.Date == ((DateTime)dayObject.date).Date).
                        FirstOrDefault();
                }
                dayObject.onToggleValueChanged.AddListener(OnToggleValueChanged);
            }
        }

        IEnumerator WaitDataLoadProcess(Action action)
        {
            if (_isRunningWaitProcess)
            {
                yield break;
            }
            _isRunningWaitProcess = true;

            // 必要なデータのロード完了まで待機.
            while (dataSource.infoMap == null)
            {
                yield return null;
            }

            _isRunningWaitProcess = false;

            action();
        }

        public void OnToggleValueChanged(Day day, bool isOn)
        {
            if (isOn)
            {
                var prevDate = recordMenu.date;
                recordMenu.date = ((DateTime)day.date).ToString(ApiHelper.DateFormat);
                recordMenu.contents = day.contents;


                if (!string.IsNullOrEmpty(prevDate))
                {
                    if (DateTime.ParseExact(prevDate, ApiHelper.DateFormat, null).Month == ((DateTime)day.date).Month)
                    {
                        recordMenu.Show();
                    }
                }
                else
                {
                    recordMenu.Show();
                }
            }
            else if (!toggleGroup.AnyTogglesOn())
            {
                recordMenu.Hide();
            }
        }
    }
}
