using UnityEngine;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Animators;
using UnityEngine.UI;
using System;
using Assets.Scripts.Common.Util;
using Assets.Scripts.Network.Response;
using System.Collections;
using Assets.Scripts.ThreeDView;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using Assets.Scripts.FullDisplaySupport;

namespace Assets.Scripts.NavMenu
{
    public class NavMenuController : SlideMenuController , IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        private float beginDraggingX;

        [SerializeField]
        private CanvasFadeAnimator _backgroundImage = default;

        [SerializeField]
        private Image _profileImage = default;

        [SerializeField]
        private Sprite _defaultImage = default;

        [SerializeField]
        private SceneLoadParam _threeDLoadParam = default;

        [SerializeField]
        private SceneLoadParam _calendarParam = default;
        
        [SerializeField]
        private Button _profileButton = default;

        [SerializeField]
        private Button _resavationButton = default;

        [SerializeField]
        private Button _qrcodeButton = default;

        [SerializeField]
        private Button _logoutButton = default;

        private NavMenuLoadParam.Argment _argment;
        private NavMenuLoadParam.Argment argment
        {
            get
            {
                return _argment = _argment ?? SceneLoader.Instance.GetArgment<NavMenuLoadParam.Argment>(gameObject.scene.name);
            }
        }

        private AsyncOperation _sceneLoadAsyncOperation;

        protected override void Start()
        {
            base.Start();

            var buttonList = GetComponentsInChildren<Button>();
            foreach (var b in buttonList)
            {
                b.onClick.AddListener(OnClickItem);
            }

#if UNITY_IOS
            //iPhoneXのノッチ対応
            if (SafeArea.IsFullDisplay() || SafeArea.HasSafeArea())
            {
                Transform childTransform = gameObject.transform;

                foreach (Transform child in childTransform)
                {
                    if (child.transform.name == "Header")
                    {
                        child.GetComponent<LayoutElement>().preferredHeight = SafeArea.HeaderHeight;
                    }
                    if (child.transform.name == "CloseButton")
                    {
                        child.GetComponent<RectTransform>().anchoredPosition = new Vector2(child.GetComponent<RectTransform>().anchoredPosition.x, child.GetComponent<RectTransform>().anchoredPosition.y - SafeArea.HeaderHeight);
                    }
                    if (child.transform.name == "Footer")
                    {
                        child.GetComponent<RectTransform>().anchoredPosition = new Vector2(child.GetComponent<RectTransform>().anchoredPosition.x, child.GetComponent<RectTransform>().anchoredPosition.y + SafeArea.FooterHeight);
                    }
                }
            }
#endif

            // 1フレーム後にアニメーションを開始.
            StartCoroutine(this.DelayFrame(1, () =>
            {
                _profileButton.interactable = !DataManager.Instance.isTrialAccount;
                _qrcodeButton.interactable = !DataManager.Instance.isTrialAccount;
                _resavationButton.interactable = !DataManager.Instance.isTrialAccount;
                Show();
            }));
        }

        /// <summary>
        /// 3Dモードボタンのクリックイベント.
        /// </summary>
        public void OnClickThreeDModeButton()
        {
            _sceneLoadAsyncOperation = SceneLoader.Instance.LoadSceneAsyncWithParams(_threeDLoadParam);
        }

        /// <summary>
        /// カレンダーボタンのクリックイベント.
        /// </summary>
        public void OnClickCalnderModeButton()
        {
            _sceneLoadAsyncOperation = SceneLoader.Instance.LoadSceneAsyncWithParams(_calendarParam);
        }

        /// <summary>
        /// メニューアイテム（3Dモード、カレンダー以外の）クリックイベント.
        /// </summary>
        public void OnClickItem()
        {
            if (argment != null)
            {
                argment.onMenuItemClicked.Invoke();
            }
            Hide();
        }

        /// <summary>
        /// メニューのオープン.
        /// </summary>
        public override void Show()
        {
            base.Show();
            if (!_backgroundImage.gameObject.activeSelf)
            {
                _backgroundImage.gameObject.SetActive(true);
                _backgroundImage.FadeIn();
            }

            // プロフィール画像の読み込み.
            var dm = DataManager.Instance;
            var content = dm.GetLatestContents();
            if (content != null)
            {
                StartCoroutine(LoadThumbnail(dm.CachePath.GetThumbnailFilePath(content.ResourceId)));
            }

            // 体験版の場合はログアウトボタンを非表示にする.
            _logoutButton.gameObject.SetActive(!dm.isTrialAccount);

            if (argment != null)
            {
                argment.onMenuOpened.Invoke();
            }
        }

        /// <summary>
        /// メニューのクローズ.
        /// </summary>
        public override void Hide()
        {
            base.Hide();

            slideAnimator.onFinishAnimation.AddListener(OnFinishSlideAnimation);
            _backgroundImage.onFinishAnimation.AddListener(OnFinishFadeAnimation);
            _backgroundImage.FadeOut();
        }

        /// <summary>
        /// サムネイル画像の読込.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IEnumerator LoadThumbnail(string path)
        {
            if (!File.Exists(path))
            {
                yield return 0;
            }

            var image = FileIO.ReadBytes(path);
            if (image != null)
            {

                yield return 0;
                Texture2D texture = new Texture2D(0, 0);
                texture.LoadImage(image);
                yield return 0;
                TextureScale.Bilinear(texture, _defaultImage.texture.width, _defaultImage.texture.height);
                _profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            yield return 0;
        }

        /// <summary>
        /// 背景のフェードアニメーション完了イベント.
        /// </summary>
        /// <param name="go"></param>
        public void OnFinishFadeAnimation(GameObject go)
        {
            _backgroundImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// メニューのスライドアニメーション完了イベント.
        /// </summary>
        /// <param name="go"></param>
        public void OnFinishSlideAnimation(GameObject go)
        {
            slideAnimator.onFinishAnimation.RemoveListener(OnFinishSlideAnimation);
            if (argment != null)
            {
                argment.onMenuClosed.Invoke();
            }

            if (_sceneLoadAsyncOperation != null)
            {
                _sceneLoadAsyncOperation.allowSceneActivation = true;
                _sceneLoadAsyncOperation = null;
            }
        }

        /// <summary>
        /// メニューのドラッグ終了イベント.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if ((eventData.position.x - beginDraggingX) < -50)
            {
                this.Hide();
            } else
            {
                this.Show();
            }
        }

        /// <summary>
        /// メニューのドラッグイベント.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            if ((eventData.position.x - beginDraggingX) < 0)
            {
                transform.position = new Vector3((eventData.position.x - beginDraggingX), this.transform.position.y, 0f);
            }
        }
        
        /// <summary>
        /// メニューのタッチ開始イベント.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            beginDraggingX = eventData.position.x;
        }
    }
}