using Assets.Scripts.Common;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Network.Response;
using Assets.Scripts.TalentDetail;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TalentList
{

    public class TalentBanner : MonoBehaviour
    {
        [SerializeField]
        private Image _backgroundImage = default;
        private Image backgroundImage {
            get {
                return _backgroundImage;
            }
        }

        [SerializeField]
        private Color _backgroundImageColor = Color.white;
        private Color backgroundImageColor {
            get {
                return _backgroundImageColor;
            }
        }
        
        [SerializeField]
        private Image _objImage = default;
        private Image objImage {
            get {
                return _objImage;
            }
        }

        [SerializeField]
        private Text _talentName = default;
        private Text talentName {
            get {
                return _talentName;
            }
        }

        [SerializeField]
        private Text _job = default;
        private Text job {
            get {
                return _job;
            }
        }

        [SerializeField]
        private Text _date = default;
        private Text date {
            get {
                return _date;
            }
        }


        [SerializeField]
        private Text _comment = default;
        private Text comment
        {
            get
            {
                return _comment;
            }
        }

        [SerializeField]
        private Text _purchaseStatusText = default;
        private Text purchaseStatusText {
            get {
                return _purchaseStatusText;
            }
        }

        [SerializeField]
        private Image _purchaseStatusTextBg = default;
        private Image purchaseStatusTextBg
        {
            get
            {
                return _purchaseStatusTextBg;
            }
        }

        [SerializeField]
        private Image _purchaseStatusTextFrame = default;
        private Image purchaseStatusTextFrame
        {
            get
            {
                return _purchaseStatusTextFrame;
            }
        }
        
        [SerializeField]
        private Color _purchasedTextColor = Color.white;
        private Color purchasedTextColor
        {
            get
            {
                return _purchasedTextColor;
            }
        }

        [SerializeField]
        private Color _unpurchasedTextColor = Color.cyan;
        private Color unpurchasedTextColor
        {
            get
            {
                return _unpurchasedTextColor;
            }
        }

        [SerializeField]
        private Color _purchasedTextBgColor = Color.clear;
        private Color purchasedTextBgColor
        {
            get
            {
                return _purchasedTextBgColor;
            }
        }

        [SerializeField]
        private Color _unpurchasedTextBgColor = Color.black;
        private Color unpurchasedTextBgColor
        {
            get
            {
                return _unpurchasedTextBgColor;
            }
        }

        [SerializeField]
        private Image _lockImage = default;
        private Image lockImage {
            get {
                return _lockImage;
            }
        }

        [SerializeField]
        private Button _button = default;
        private Button button {
            get {
                return _button;
            }
        }

        [SerializeField]
        private Image _disablePanel = default;
        private Image disablePanel {
            get {
                return _disablePanel;
            }
        }

        [SerializeField]
        private TalentDetailLoadParam _detailLoadParam = default;
        private TalentDetailLoadParam detailLoadParam {
            get {
                return _detailLoadParam;
            }
        }

        private TalentoContents _contents;
        public TalentoContents contents
        {
            get
            {
                return _contents;
            }
            set
            {
                talentName.text = value.CommonName;
                job.text = "/" + value.Job;
                comment.text = value.Comment;
                date.text = value.CreateTimeAsDateTime.ToString("yyyy/MM/dd");

                if (value.purchaseStatus == PurchaseStatus.Purchased)
                {
                    purchaseStatusText.text = "購入済み";
                    purchaseStatusText.color = purchasedTextColor;
                    purchaseStatusTextFrame.color = purchasedTextColor;
                    purchaseStatusTextBg.color = purchasedTextBgColor;
                    lockImage.gameObject.SetActive(false);
                }
                else
                {
                    purchaseStatusText.text = value.Point.ToString(AppConst.PointFormat);
                    purchaseStatusText.color = unpurchasedTextColor;
                    purchaseStatusTextFrame.color = unpurchasedTextColor;
                    purchaseStatusTextBg.color = unpurchasedTextBgColor;
                    lockImage.gameObject.SetActive(true);
                }

                // バナー画像のダウンロード＋設定.
                StartCoroutine(value.GetBannerImage(image => {
                    backgroundImage.color = backgroundImageColor;
                    backgroundImage.sprite = image;
                }));

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    detailLoadParam.argment.talentInfo = contents;
                    SceneLoader.Instance.LoadSceneWithParams(detailLoadParam);
                });

                _contents = value;
            }
        }

        private void Start()
        {
        }
    }
}
