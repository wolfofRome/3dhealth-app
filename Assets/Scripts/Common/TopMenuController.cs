using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Assets.Scripts.ThreeDView;

namespace Assets.Scripts.Common
{
    public class TopMenuController : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _onClickOpenButton = default;

        [SerializeField]
        private UnityEvent _onClickCloseButton = default;
        
        [SerializeField]
        private Button _modeMenuOpenButton = default;

        [SerializeField]
        private Button _modeMenuCloseButton = default;

        [SerializeField]
        private Text _title = default;

        [SerializeField]
        private float _fadeDuration = 0.2f;
        
        private Text _dummyTitle;
        private Text dummyTitle {
            get {
                if (_dummyTitle == null)
                {
                    _dummyTitle = Instantiate(_title);
                    _dummyTitle.transform.SetParent(_title.transform.parent, false);
                }
                return _dummyTitle;
            }
        }

        void Start()
        {
            _modeMenuOpenButton.onClick.AddListener(OpenModeMenu);
            _modeMenuCloseButton.onClick.AddListener(CloseModeMenu);
        }
        
        void Update()
        {
        }

        public void OpenModeMenu()
        {
            _modeMenuOpenButton.gameObject.SetActive(false);
            _modeMenuCloseButton.gameObject.SetActive(true);
            _onClickOpenButton.Invoke();
        }

        public void CloseModeMenu()
        {
            _modeMenuOpenButton.gameObject.SetActive(true);
            _modeMenuCloseButton.gameObject.SetActive(false);
            _onClickCloseButton.Invoke();
        }

        public void OnModeChanged(Mode mode)
        {
            SetTitleWithAnimation(mode.GetTitle());
        }

        public void SetTitleWithAnimation(string title)
        {
            dummyTitle.CrossFadeAlpha(1, 0, false);
            _title.CrossFadeAlpha(0, 0, false);

            dummyTitle.text = _title.text;
            _title.text = title;

            dummyTitle.CrossFadeAlpha(0, _fadeDuration, false);
            _title.CrossFadeAlpha(1, _fadeDuration, false);
        }
    }
}