using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Common.Config;

namespace Assets.Scripts.History
{
    public class HistoryController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private Text _title = default;
        [SerializeField]
        private Text _scanDate = default;
        [SerializeField]
        private WMG_Axis_Graph _graph = default;
        [SerializeField]
        private WMG_Series _series = default;
        [SerializeField]
        private List<Toggle> _graphTermToggle = default;
        [SerializeField]
        private Image _flareImage = default;

        private class HistoryItem
        {
            public HistoryItem(DateTime date, double value)
            {
                this.date = date;
                this.value = value;
            }

            public DateTime date { get; set; }
            public double value { get; set; }
        }
        private List<HistoryItem> _history = new List<HistoryItem>();

        private DateTime _historyFromDate = DateTime.Today.Date;
        private DateTime _historyToDate = DateTime.Today.Date;

        private MeasurementItem _selectedItem = MeasurementItem.Weight;

        private enum TermType
        {
            Week1 = 0,
            Month1 = 1,
            Month3 = 2,
            Month6 = 3,
            Year1 = 4
        }
        private TermType _selectedTermType = TermType.Week1;
        
        private bool _updateGraph = false;

		private DateTime _graphStartDate = DateTime.Today.Date;
        
        // グラフのゲームオブジェクト
        private GameObject _graphObject;
        private CanvasGroup _graphCanvasGroup;

        // グラフドラッグ中フラグ
        private bool _isGraphDragging;

        // グラフ期間変更中フラグ
        private bool _isGraphTermChanging;

        // グラフドラッグ開始位置
        private Vector3 _graphObjectDragBeginPosition;

        // ドラッグ開始のタップポイント
        private Vector3 _graphDragBeginTouchPosition;

        // ドラッグ可能最大距離
        private float _maxDragMoveValue = Screen.width / 3;

        private void Start()
        {
            int historyGraphTerm = UserData.HistoryGraphTerm;
			_selectedTermType = (TermType)historyGraphTerm;
            _graphTermToggle[historyGraphTerm].isOn = true;
            var args = SceneLoader.Instance.GetArgment<HistoryLoadParam.Argment>(gameObject.scene.name);
            Assert.IsNotNull(args, "HistoryLoadParam.Argment is null");

            _selectedItem = args.dispItem;
            _title.text = _selectedItem.GetTitle();
            _scanDate.text = args.selectedDate.ToString("yyyy年M月d日(ddd)", new CultureInfo("ja-JP"));

            _graphObject = _graph.gameObject;
            _graphCanvasGroup = _graphObject.GetComponent<CanvasGroup>();
            _isGraphDragging = false;
            _isGraphTermChanging = false;

        }

        private void Update()
        {
            if (_updateGraph)
            {
                UpdateGraph();
            }
        }

        public void OnValueChangedTermWeek1(bool isChecked)
        {
            OnValueChangedTerm(TermType.Week1, isChecked);
        }

        public void OnValueChangedTermMonth1(bool isChecked)
        {
            OnValueChangedTerm(TermType.Month1, isChecked);
        }

        public void OnValueChangedTermMonth3(bool isChecked)
        {
            OnValueChangedTerm(TermType.Month3, isChecked);
        }

        public void OnValueChangedTermMonth6(bool isChecked)
        {
            OnValueChangedTerm(TermType.Month6, isChecked);
        }

        public void OnValueChangedTermYear1(bool isChecked)
        {
            OnValueChangedTerm(TermType.Year1, isChecked);
        }

        private void OnValueChangedTerm(TermType termType, bool isChecked)
        {
			if (!isChecked)
			{
				return;
			}

            _selectedTermType = termType;
			UserData.HistoryGraphTerm = (int)_selectedTermType;

            _updateGraph = true;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (_isGraphDragging || _isGraphTermChanging)
            {
                // ドラッグ中 or 期間変更中
                return;
            }

            _isGraphDragging = true;
            _graphObjectDragBeginPosition = _graphObject.transform.position;
            _graphDragBeginTouchPosition = data.position;
        }

        public void OnDrag(PointerEventData data)
        {
            if (!_isGraphDragging || _isGraphTermChanging)
            {
                // ドラッグ停止 or 期間変更中
                return;
            }

            float moveValue = _graphDragBeginTouchPosition.x - data.position.x;

            if (Math.Abs(moveValue) < _maxDragMoveValue)
            {
                // ドラッグ中はドラッグポイントにグラフを追従させる
                _graphObject.transform.position = new Vector3(
                    _graphObjectDragBeginPosition.x - moveValue,
                    _graphObjectDragBeginPosition.y,
                    _graphObjectDragBeginPosition.z
                );

                return;
            }

            // 期間変更判定へ
            DateTime startDate;

            if (moveValue < 0)
            {
                // 前の期間
                startDate = _graphStartDate.AddDays(-(GetTermDays() + 1));

                // 履歴の最も古い期間の表示終了日
                DateTime fromStartDate = _historyFromDate.AddDays(GetTermDays());

                // グラフの表示期間が履歴の最も古いデータデータより過去を含む場合、履歴の最も古いデータ日を表示開始日とする
                startDate = startDate < fromStartDate ? fromStartDate : startDate;
            }
            else
            {
                // 次の期間
                startDate = _graphStartDate.AddDays(GetTermDays() + 1);

                // グラフの表示期間が本日より未来を含む場合、本日を表示終了日とする
                startDate = startDate > _historyToDate ? _historyToDate : startDate;
            }

            if (startDate != _graphStartDate)
            {
                // グラフ表示期間変更
                _graphStartDate = startDate;

                // 期間変更アニメーションへ
                _isGraphDragging = false;
                _isGraphTermChanging = true;

                iTween.ValueTo(
                    _graphObject,
                    iTween.Hash(
                        "from", 1f,
                        "to", 0f,
                        "time", 0.5f,
                        "delay", 0f,
                        "onupdate", "OnUpdateGraphFadeAnimation",
                        "onupdatetarget", gameObject,
                        "oncomplete", "OnCompleteGraphCurrentTermFadeOutAnimation",
                        "oncompletetarget", gameObject
                    )
                );
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (!_isGraphDragging)
            {
                return;
            }

            _isGraphDragging = false;

            if (!_isGraphTermChanging)
            {
                iTween.MoveTo(_graphObject, _graphObjectDragBeginPosition, 0.5f);
            }
        }

        private void OnUpdateGraphFadeAnimation(float value)
        {
            _graphCanvasGroup.alpha = value;
        }

        private void OnCompleteGraphCurrentTermFadeOutAnimation()
        {
            _graphObject.transform.position = _graphObjectDragBeginPosition;

            _updateGraph = true;

            iTween.ValueTo(
                _graphObject,
                iTween.Hash(
                    "from", 0f,
                    "to", 1f,
                    "time", 0.5f,
                    "delay", 0f,
                    "onupdate", "OnUpdateGraphFadeAnimation",
                    "onupdatetarget", gameObject,
                    "oncomplete", "OnCompleteGraphNextTermFadeInAnimation",
                    "oncompletetarget", gameObject
                )
            );
        }

        private void OnCompleteGraphNextTermFadeInAnimation()
        {
            _isGraphTermChanging = false;
        }

        private int GetTermDays()
		{
			switch (_selectedTermType)
			{
			case TermType.Week1:
				return 6;
			case TermType.Month1:
				return 30;
			case TermType.Month3:
				return 90;
			case TermType.Month6:
				return 180;
			case TermType.Year1:
				return 360;
			default:
				throw new Exception();
			}
		}

        private List<HistoryItem> SelectTermPanelRowItemData()
        {
            var list = new List<HistoryItem>();

            int rowDataListCount = _history.Count;

			int termDays = GetTermDays();

			DateTime maxDate = _graphStartDate.AddDays(1);
			DateTime minDate = _graphStartDate.AddDays(-termDays);

            for (int i = 0; i < rowDataListCount; i++)
            {
				DateTime targetDate = _history[i].date.Date;

                if (targetDate >= minDate &&
					targetDate < maxDate)
                {
                    list.Add(_history[i]);
                }
            }

            return list;
        }

        private void UpdateGraph()
        {
			_updateGraph = false;

            _flareImage.gameObject.SetActive(false);

            if (_history.Count == 0)
			{
				return;
			}
            
            _graph.theTooltip.tooltipLabeler = customTooltipLabeler;
            _graph.xAxis.hideTicks = true;
            _graph.yAxis.hideTicks = true;
            _graph.xAxis.axisLabels.Clear();
            _graph.yAxis.axisLabels.Clear();
            _series.pointValues.Clear();

            _series.seriesName = _title.text;

			int termDays = GetTermDays();

			// 点データの作成
			List<Vector2> pointList = new List<Vector2>();
			List<HistoryItem> list = SelectTermPanelRowItemData();

			list.Sort((a, b) => a.date.CompareTo(b.date));

			int listCount = list.Count;

			float yAxisMaxValue = float.MinValue;
			float yAxisMinValue = float.MaxValue;

			for (int i = 0; i < listCount; i++)
			{
				TimeSpan timeSpan = _graphStartDate - list[i].date.Date;

				int yAxisValue = termDays - timeSpan.Days;
				pointList.Add(new Vector2(yAxisValue, (float)list[i].value));
                yAxisMaxValue = Math.Max(yAxisMaxValue, (float)list[i].value);
				yAxisMinValue = Math.Min(yAxisMinValue, (float)list[i].value);
			}
            // X軸ラベル設定
            int xAxisNumTicks = 2;
			for (int i = 6; i > 2; i--)
			{
				if (termDays % i == 0)
				{
					xAxisNumTicks = i + 1;
					break;
				}
			}

            _graph.xAxis.AxisMinValue = 0;
            _graph.xAxis.AxisMaxValue = termDays;
            _graph.xAxis.AxisNumTicks = xAxisNumTicks;

			int xAxisLabelValue = termDays / (xAxisNumTicks - 1);

			for (int i = 1; i < xAxisNumTicks; i++)
            {
				int days = ((xAxisNumTicks - i) * xAxisLabelValue);
                _graph.xAxis.axisLabels.Add(_graphStartDate.AddDays(-days).ToString("MM/dd"));
			}

            _graph.xAxis.axisLabels.Add(_graphStartDate.ToString("MM/dd"));

			if (list.Count == 0)
			{
				return;
			}

			// Y軸ラベル設定(データの上下20%幅)
			yAxisMaxValue = yAxisMaxValue + (yAxisMaxValue * 0.2f);
			yAxisMinValue = yAxisMinValue - (yAxisMinValue * 0.2f);
            _graph.yAxis.AxisMaxValue = yAxisMaxValue;
            _graph.yAxis.AxisMinValue = yAxisMinValue;
            _graph.yAxis.AxisNumTicks = 5;

			float yAxisLabelValue = (yAxisMaxValue - yAxisMinValue) / (_graph.yAxis.AxisNumTicks - 1);
			string yAxisLabelToStringParam = yAxisLabelValue >= 0.1 ? "0.0" : "0.00";
            _graph.yAxis.axisLabels.Add(yAxisMinValue.ToString(yAxisLabelToStringParam));
            _graph.yAxis.axisLabels.Add((yAxisMinValue + (yAxisLabelValue * 1)).ToString(yAxisLabelToStringParam));
            _graph.yAxis.axisLabels.Add((yAxisMinValue + (yAxisLabelValue * 2)).ToString(yAxisLabelToStringParam));
            _graph.yAxis.axisLabels.Add((yAxisMinValue + (yAxisLabelValue * 3)).ToString(yAxisLabelToStringParam));
            _graph.yAxis.axisLabels.Add(yAxisMaxValue.ToString(yAxisLabelToStringParam));

            // 点のデータ設定
            for (int i = 0; i < pointList.Count; i++)
			{
                _series.pointValues.Add(pointList[i]);
			}

            StartCoroutine(this.DelayFrame(1, () => {
                if (_graph.NodesParent.Count > 0)
                {
                    _flareImage.gameObject.SetActive(true);
                    _flareImage.transform.position = _graph.NodesParent[_graph.NodesParent.Count - 1].transform.position;
                }
            }));
        }

        public void OnLoadCompleted(HistoryLoader loader)
        {
            switch (_selectedItem.GetDataType())
            {
                case DataType.BodyComposition:
                    _history = ConvertToHistoryData(loader.bodyCompositionDataLoader.BodyCompositionList, src =>
                    {
                        return new HistoryItem(src.CreateTimeAsDateTime, src.GetValue<double>(_selectedItem.ToString()));
                    });
                    break;
                case DataType.Size:
                    _history = ConvertToHistoryData(loader.measurementDataLoader.MeasurementList, src =>
                    {
                        return new HistoryItem(src.CreateTimeAsDateTime, src.GetValue<int>(_selectedItem.ToString()) * AppConst.MeasurementDataScale);
                    });
                    break;
                default:
                    break;
            }
            _updateGraph = true;
        }

        private List<HistoryItem> ConvertToHistoryData<T>(List<T> list, Func<T, HistoryItem> convert) where T : BaseDataRow
        {
            var isSetDate = new List<string>();
            var history = new List<HistoryItem>();

            _historyFromDate = DateTime.MaxValue;
            _historyToDate = DateTime.MinValue;

            int dataListCount = list.Count;
            for (int i = 0; i < dataListCount; i++)
            {
                var item = convert(list[i]);
                
                if (Mathf.Approximately((float)item.value, 0))
                {
                    continue;
                }

                string dateString = item.date.ToString("yyyy/MM/dd");
                if (isSetDate.Contains(dateString))
                {
                    continue;
                }
                isSetDate.Add(dateString);
                
                history.Add(item);

                if (_historyFromDate > item.date.Date)
                {
                    _historyFromDate = item.date.Date;
                }

                if (_historyToDate < item.date.Date)
                {
                    _historyToDate = item.date.Date;
                }
            }

            // グラフ表示期間を履歴の一番新しいデータに設定
            // TODO:グラフ表示期間を本日に設定
            _historyToDate = DateTime.Today.Date;
            _graphStartDate = _historyToDate;

            return history;
        }

        private string customTooltipLabeler(WMG_Series aSeries, WMG_Node aNode)
        {
            Vector2 nodeData = aSeries.getNodeValue(aNode);

			DateTime minDate = _graphStartDate.AddDays(-GetTermDays());

            DateTime dt = minDate.AddDays(nodeData.x);

            return dt.ToString("yyyy/MM/dd") + " : " + nodeData.y.ToString("f1");
		}

		public void OnClickPrevButton()
		{
			_graphStartDate = _graphStartDate.AddDays(-(GetTermDays() + 1));

			//OnValueChangedMenu(_selectedDataType);
		}

		public void OnClickNextButton()
		{
			_graphStartDate = _graphStartDate.AddDays(GetTermDays() + 1);

			//OnValueChangedMenu((int)_selectedDataType);
		}
    }
}