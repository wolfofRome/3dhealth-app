using Assets.Scripts.Graphics.UI;
using Assets.Scripts.Network.Response;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Calendar
{ 
    [RequireComponent(typeof(Toggle))]
    [RequireComponent(typeof(ToggleGraphic))]
    public class Day : MonoBehaviour
    {
        [Serializable]
        public class ToggleValueChangeEvent : UnityEvent<Day, bool> { }

        [SerializeField]
        private ToggleValueChangeEvent _onToggleValueChanged = default;
        public ToggleValueChangeEvent onToggleValueChanged {
            get {
                return _onToggleValueChanged;
            }
        }

        [SerializeField]
        private Text _dateText = default;

        [SerializeField]
        private Image _todayBackgroundImage = default;

        [SerializeField]
        private List<Image> _exerciseImageList = default;

        [SerializeField]
        private Text _ellipsisText = default;

        [SerializeField]
        private Image _scanImage = default;

        [SerializeField]
        private Image _memoImage = default;

        [SerializeField]
        private Text _appointmentText = default;

        [SerializeField]
        private CalendarConfig _config = default;


        [SerializeField]
        protected Color _onColor = default;

        [SerializeField]
        protected Color _offColor = default;

        [SerializeField]
        protected Color _todayColor = default;

        private ToggleGraphic _toggleGraphic;
        protected ToggleGraphic toggleGraphic
        {
            get
            {
                return _toggleGraphic = _toggleGraphic ?? GetComponent<ToggleGraphic>();
            }
        }

        private DateTime? _date;
        public DateTime? date {
            get {
                return _date;
            }
            set {
                _date = value;
                if (value != null)
                {
                    var dateTime = ((DateTime)value);
                    _dateText.text = dateTime.Day.ToString();
                    if (dateTime.Date.Equals(DateTime.Now.Date))
                    {
                        _todayBackgroundImage.gameObject.SetActive(true);
                        toggleGraphic.onColor = _todayColor;
                        toggleGraphic.offColor = _todayColor;
                    }
                    else
                    {
                        _todayBackgroundImage.gameObject.SetActive(false);
                        toggleGraphic.onColor = _onColor;
                        toggleGraphic.offColor = _offColor;
                    }
                }
                else
                {
                    _dateText.text = "";
                    _todayBackgroundImage.gameObject.SetActive(false);
                    toggleGraphic.onColor = _onColor;
                    toggleGraphic.offColor = _offColor;
                }
            }
        }

        private Toggle _toggle;
        public Toggle toggle {
            get {
                return _toggle = _toggle ?? GetComponent<Toggle>();
            }
        }

        private CalendarInfo _info;
        public CalendarInfo info {
            get {
                return _info;
            }
            set {
                _info = value;

                InitChildItems();

                // 予約テキストは未来の項目のみ表示.
                if (value != null)
                {
                    if (value.Appointment != null && value.Appointment.StartTimeAsDateTime > DateTime.Now)
                    {
                        _appointmentText.gameObject.SetActive(true);
                        _appointmentText.text = info.Appointment.StartTimeAsDateTime.ToString("3D予約\nH:mm");
                    }
                    else
                    {
                        // メモアイコンの表示設定.
                        _memoImage.gameObject.SetActive(!string.IsNullOrEmpty(value.Memo));

                        // 運動アイコンの表示設定.
                        if (value.ExerciseList != null)
                        {
                            var imageNum = _exerciseImageList.Count;
                            var exerciseNum = value.ExerciseList.Length;

                            var spriteSettingCount = Mathf.Min(imageNum, exerciseNum);
                            for (var i = 0; i < spriteSettingCount; i++)
                            {
                                _exerciseImageList[i].sprite = _config.exerciseImageMap[value.ExerciseList[i].ExerciseId].sprite;
                                _exerciseImageList[i].gameObject.SetActive(true);
                            }

                            var ellipsisNum = exerciseNum - imageNum;
                            if (ellipsisNum > 0)
                            {
                                _ellipsisText.gameObject.SetActive(true);
                                _ellipsisText.text = "+" + ellipsisNum.ToString();
                            }
                        }
                        // スキャンデータアイコンの表示設定.
                        _scanImage.gameObject.SetActive(contents != null);
                    }
                }
            }
        }

        private Contents _contents;
        public Contents contents {
            get {
                return _contents;
            }
            set {
                _contents = value;
                _scanImage.gameObject.SetActive(contents != null && !_appointmentText.gameObject.activeSelf);
            }
        }

        void Start()
        {
            toggle.onValueChanged.AddListener( isOn =>
            {
                onToggleValueChanged.Invoke(this, isOn);
            });
        }

        private void InitChildItems()
        {
            _appointmentText.gameObject.SetActive(false);
            _memoImage.gameObject.SetActive(false);
            _exerciseImageList.ForEach(image =>
            {
                image.gameObject.SetActive(false);
            });
            _scanImage.gameObject.SetActive(false);
            _ellipsisText.gameObject.SetActive(false);
        }
    }
}
