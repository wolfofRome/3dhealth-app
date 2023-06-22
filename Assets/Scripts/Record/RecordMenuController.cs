
using Assets.Scripts.Common;
using Assets.Scripts.Network.Response;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Assets.Scripts.ThreeDView;
using Assets.Scripts.Network;
using Assets.Scripts.Common.Animators;
using Assets.Scripts.Calendar;

namespace Assets.Scripts.Record
{
    public class RecordMenuController : SlideMenuController
    {
        [SerializeField]
        private List<ExerciseItem> _exerciseItemList = default;

        [SerializeField]
        private Button _threeDViewButton = default;

        private RectTransform _threeDViewButtonRectTransform;
        private RectTransform threeDViewButtonRectTransform
        {
            get
            {
                return _threeDViewButtonRectTransform = _threeDViewButtonRectTransform ?? _threeDViewButton.GetComponent<RectTransform>();
            }
        }

        [SerializeField]
        private Button _memoButton = default;

        [SerializeField]
        private Image _pencilImage = default;

        [SerializeField]
        private InputField _memoInputField = default;

        [SerializeField]
        private ThreeDViewLoadParam _loadParam = default;

        [SerializeField]
        private List<SlideMenuController> _invertItemList = default;

        [SerializeField]
        private CalendarDataSource _dataSource = default;

        [SerializeField]
        private float _memoOpenHeight = default;
        private float memoOpenHeight
        {
            get
            {
                return _memoOpenHeight - (_threeDViewButton.IsActive() ? threeDViewButtonRectTransform.rect.height : 0);
            }
        }

        [SerializeField]
        private float _memoCloseHeight = default;
        private float memoCloseHeight
        {
            get
            {
                return _memoCloseHeight;
            }
        }

        [SerializeField]
        private float _memoOpenDuration = 0.3f;

        [SerializeField]
        private List<RectTransformAnimator> _coordinateAnimator = default;

        private Dictionary<ExerciseId, ExerciseItem> _exerciseItemMap;
        private Dictionary<ExerciseId, ExerciseItem> exerciseItemMap {
            get {
                return _exerciseItemMap = _exerciseItemMap ?? _exerciseItemList.ToDictionary(x => x.id);
            }
        }
        
        private string _date;
        public string date {
            get {
                return _date;
            }
            set {
                if (_date != value)
                {
                    SaveExercise();

                    CalendarInfo info;
                    if (value != null && _dataSource.infoMap.TryGetValue(value, out info))
                    {
                        this.info = info;
                    }
                    else
                    {
                        this.info = null;
                    }
                }
                _date = value;
            }
        }

        private CalendarInfo _info;
        private CalendarInfo info {
            get {
                return _info;
            }
            set {
                _info = value;

                InitChildItems();
                
                if (value != null)
                {
                    if (value.ExerciseList != null)
                    {
                        foreach (var item in exerciseItemMap)
                        {
                            item.Value.toggle.isOn = value.ExerciseList.Any(x => x.ExerciseId == item.Key);
                        }
                    }
                    _memoInputField.text = string.IsNullOrEmpty(value.Memo) ? "" : value.Memo;
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

                _scanButtonArea.SetActive(_contents != null);

                if (_contents != null)
                {
                    _loadParam.argment.resourceId = _contents.ResourceId;
                }
            }
        }

        [SerializeField]
        private GameObject _scanButtonArea = default;
        public GameObject scanButtonArea
        {
            get
            {
                return _scanButtonArea;
            }
        }

        protected override void Start()
        {
            base.Start();

            _memoButton.onClick.AddListener( () =>
            {
                iTween.ValueTo(gameObject,
                    iTween.Hash(
                        "from", memoCloseHeight,
                        "to", memoOpenHeight,
                        "time", _memoOpenDuration,
                        "delay", 0f,
                        "onupdate", "OnUpdateAnimation",
                        "onupdatetarget", gameObject,
                        "oncomplete", "OnFinishAnimation",
                        "oncompletetarget", gameObject));

                _memoButton.gameObject.SetActive(false);                
                _memoInputField.ActivateInputField();
            });

            _memoInputField.onValueChanged.AddListener(text =>
            {
                if (string.IsNullOrEmpty(text))
                {
                    _pencilImage.enabled = true;
                }else
                {
                    _pencilImage.enabled = false;
                }
            });

            _memoInputField.onEndEdit.AddListener(text =>
            {
                iTween.ValueTo(gameObject,
                    iTween.Hash(
                        "from", memoOpenHeight,
                        "to", memoCloseHeight,
                        "time", _memoOpenDuration,
                        "delay", 0f,
                        "onupdate", "OnUpdateAnimation",
                        "onupdatetarget", gameObject,
                        "oncomplete", "OnFinishAnimation",
                        "oncompletetarget", gameObject));
                _memoButton.gameObject.SetActive(true);
                SaveMemo(text);
            });

            _threeDViewButton.onClick.AddListener(() =>
            {
                SceneLoader.Instance.LoadSceneWithParams(_loadParam);
            });
        }

        /// <summary>
        /// iTweeenアニメーションの更新イベント.
        /// </summary>
        /// <param name="value"></param>
        public void OnUpdateAnimation(float value)
        {
            var memoParentLayout = _memoInputField.transform.parent.GetComponent<LayoutElement>();
            memoParentLayout.preferredHeight = value;
        }

        /// <summary>
        /// iTweeenアニメーションの完了イベント.
        /// </summary>
        public void OnFinishAnimation()
        {
        }

        /// <summary>
        /// メニューのオープン.
        /// </summary>
        public override void Show()
        {
            base.Show();
            foreach (var item in _invertItemList)
            {
                item.Hide();
            }
            foreach (var item in _coordinateAnimator)
            {
                item.StartCollapseAnimation();
            }
        }

        /// <summary>
        /// メニューのクローズ.
        /// </summary>
        public override void Hide()
        {
            base.Hide();
            foreach (var item in _invertItemList)
            {
                item.Show();
            }
            foreach (var item in _coordinateAnimator)
            {
                item.StartExpandAnimation();
            }
            SaveExercise();
        }

        /// <summary>
        /// パネル全体の初期化.
        /// </summary>
        private void InitChildItems()
        {
            foreach (var item in exerciseItemMap)
            {
                item.Value.toggle.isOn = false;
            }
            _memoInputField.text = "";
            _memoButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// エクササイズの選択状態の保存.
        /// </summary>
        private void SaveExercise()
        {
            if (date != null )
            {
                var activeExerciseIdList = exerciseItemMap.Where(x => x.Value.toggle.isOn).
                                                           Select(x => x.Value.id).
                                                           ToArray();
                if (isExerciseUpdated(activeExerciseIdList))
                {
                    StartCoroutine(ApiHelper.Instance.UpdateCalendarInfo(activeExerciseIdList, date, response =>
                    {
                        if (response.ErrorCode != null)
                        {
                            DebugUtil.LogError("UpdateCalendarInfo -> " + response.ErrorCode + " : " + response.ErrorMessage);
                            return;
                        }
                        _dataSource.MargeBaseCalendarInfo(response.UpdateInfo);
                    }));
                }
            }
        }

        /// <summary>
        /// エクササイズアイコンのON/OFF状態の更新.
        /// </summary>
        /// <param name="activeExerciseIdList"></param>
        /// <returns></returns>
        private bool isExerciseUpdated(ExerciseId[] activeExerciseIdList)
        {
            if (info == null || info.ExerciseList == null)
            {
                return activeExerciseIdList.Length > 0;
            }
            else
            {
                foreach (var item in exerciseItemMap)
                {
                    var exercise = item.Value;
                    var orgValue = exercise.toggle.isOn;
                    var newValue = info.ExerciseList.Any(x => x.ExerciseId == exercise.id);
                    if (orgValue != newValue)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// メモの保存.
        /// </summary>
        /// <param name="memo"></param>
        private void SaveMemo(string memo)
        {
            if (date != null)
            {
                StartCoroutine(ApiHelper.Instance.UpdateCalendarInfo(memo, date, response =>
                {
                    if (response.ErrorCode != null)
                    {
                        DebugUtil.LogError("UpdateCalendarInfo -> " + response.ErrorCode + " : " + response.ErrorMessage);
                        return;
                    }
                    _dataSource.MargeBaseCalendarInfo(response.UpdateInfo);
                }));
            }
        }
    }
}