using Assets.Scripts.Common;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Calendar
{
    public class CalendarDataSource : ScriptableObject
    {
        public const string KeyFormat = ApiHelper.DateFormat;

        [Serializable]
        public class DataSourceUpdateEvent : UnityEvent<string[], CalendarDataSource> { }

        [SerializeField]
        private DataSourceUpdateEvent _onUpdate = new DataSourceUpdateEvent();
        public DataSourceUpdateEvent onUpdate {
            get {
                return _onUpdate;
            }
        }

        public Dictionary<string, CalendarInfo> _infoMap = new Dictionary<string, CalendarInfo>();
        public Dictionary<string, CalendarInfo> infoMap {
            get {
                return _infoMap;
            }
        }

        private DateTime _dateFrom;
        public DateTime dateFrom {
            get {
                return _dateFrom;
            }
            private set {
                _dateFrom = value;
            }
        }

        private DateTime _dateTo;
        public DateTime dateTo {
            get {
                return _dateTo;
            }
            private set {
                _dateTo = value;
            }
        }
        
        /// <summary>
        /// カレンダーの更新.
        /// </summary>
        /// <param name="baseDate"></param>
        /// <returns></returns>
        public IEnumerator UpdateDataSource(DateTime baseDate)
        {
            if ((dateFrom - dateTo).Equals(TimeSpan.Zero))
            {
                // 初回取得
                var loadNum = CalendarConfig.OnceLoadNum / 2;
                var from = baseDate.AddMonths(-loadNum);
                var to = baseDate.AddMonths(loadNum);

                yield return ApiHelper.Instance.GetCalendarInfo(from, to, response =>
                {
                    if (response.ErrorCode != null)
                    {
                        DebugUtil.LogError("GetCalendarInfo -> " + response.ErrorCode + " : " + response.ErrorMessage);
                        return;
                    }
                    InitCalendarInfoList(response.CalendarInfoList, from, to);
                });
            }
            else
            {
                // カレンダーのデータを分割してロード.
                // 取得済みのデータの範囲が、指定されたbaseDateの前後3ヶ月以内となった場合、
                // 12ヶ月分のデータを取得して既存のデータにマージ.
                var loadingThreshold = CalendarConfig.OnceLoadNum / 4;
                var fromDiff = baseDate.MonthDiff(dateFrom);
                var toDiff = baseDate.MonthDiff(dateTo);

                if (fromDiff < loadingThreshold || toDiff < loadingThreshold)
                {
                    DateTime from, to;
                    if (fromDiff < loadingThreshold)
                    {
                        from = dateFrom.AddMonths(-CalendarConfig.OnceLoadNum);
                        to = dateFrom;
                    }
                    else
                    {
                        from = dateTo;
                        to = dateTo.AddMonths(CalendarConfig.OnceLoadNum);
                    }

                    yield return ApiHelper.Instance.GetCalendarInfo(from, to, response =>
                    {
                        if (response.ErrorCode != null)
                        {
                            DebugUtil.LogError("GetCalendarInfo -> " + response.ErrorCode + " : " + response.ErrorMessage);
                            return;
                        }
                        MargeCalendarInfoRange(response.CalendarInfoList, from, to);
                    });
                }
            }
        }

        /// <summary>
        /// カレンダーの初期化.
        /// </summary>
        /// <param name="infoList"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void InitCalendarInfoList(CalendarInfo[] infoList, DateTime from, DateTime to)
        {
            _infoMap = new Dictionary<string, CalendarInfo>();
            foreach (var info in infoList)
            {
                _infoMap[info.Date] = info;
            }
            dateFrom = from;
            dateTo = to;
            NotifyUpdateEvent(infoList);
        }

        /// <summary>
        /// カレンダーのマージ.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void MargeCalendarInfo(CalendarInfo info, DateTime? from = null, DateTime? to = null)
        {
            _infoMap[info.Date] = info;
            UpdatePeriod(from, to);
            NotifyUpdateEvent(info);
        }

        /// <summary>
        /// カレンダーのマージ.
        /// </summary>
        /// <param name="newInfoList"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void MargeCalendarInfoRange(CalendarInfo[] newInfoList, DateTime? from = null, DateTime? to = null)
        {
            foreach (var info in newInfoList)
            {
                MargeCalendarInfo(info);
            }
            UpdatePeriod(from, to);
            NotifyUpdateEvent(newInfoList);
        }

        /// <summary>
        /// カレンダーのマージ.
        /// </summary>
        /// <param name="newInfo"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void MargeBaseCalendarInfo(BaseCalendarInfo newInfo, DateTime? from = null, DateTime? to = null)
        {
            if (newInfo == null || newInfo.Date == null)
            {
                return;
            }

            CalendarInfo org;
            if (_infoMap.TryGetValue(newInfo.Date, out org))
            {
                org.Memo = newInfo.Memo;
                org.ExerciseList = newInfo.ExerciseList;
            }
            else
            {
                _infoMap[newInfo.Date] = new CalendarInfo(newInfo);
            }
            UpdatePeriod(from, to);
            NotifyUpdateEvent(newInfo);
        }

        /// <summary>
        /// カレンダーのマージ.
        /// </summary>
        /// <param name="infoList"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void MargeBaseCalendarInfoRange(BaseCalendarInfo[] infoList, DateTime? from = null, DateTime? to = null)
        {
            foreach (var info in infoList)
            {
                MargeBaseCalendarInfo(info);
            }
            UpdatePeriod(from, to);
            NotifyUpdateEvent(infoList);
        }

        /// <summary>
        /// カレンダーの期間更新.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void UpdatePeriod(DateTime? from = null, DateTime? to = null)
        {
            if (from != null && from < dateFrom ) 
            {
                dateFrom = (DateTime)from;
            }
            if (to != null && dateTo < to)
            {
                dateTo = (DateTime)to;
            }
        }

        /// <summary>
        /// カレンダー更新イベントの通知.
        /// </summary>
        /// <param name="info"></param>
        private void NotifyUpdateEvent(BaseCalendarInfo info)
        {
            onUpdate.Invoke(new string[] { info.Date }, this);
        }

        /// <summary>
        /// カレンダー更新イベントの通知.
        /// </summary>
        /// <param name="infoList"></param>
        private void NotifyUpdateEvent(BaseCalendarInfo[] infoList)
        {
            onUpdate.Invoke(infoList.Select(x => x.Date).ToArray(), this);
        }
    }
}
