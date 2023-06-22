using Assets.Scripts.Common;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Response;
using Assets.Scripts.PointPurchase;
using Assets.Scripts.TalentList;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.TalentDetail
{
    public class PurchaseMenu : SlideMenuController
    {
        [Serializable]
        public class TalentPurchaseEvent : UnityEvent<TalentoContents> { }
        public TalentPurchaseEvent OnPurchaseCompleted;

        [SerializeField]
        private Image _talentImage = default;
        private Image talentImage {
            get {
                return _talentImage;
            }
        }

        [SerializeField]
        private Text _talentName = default;
        private Text talentName
        {
            get
            {
                return _talentName;
            }
        }

        [SerializeField]
        private Text _talentJob = default;
        private Text talentJob
        {
            get
            {
                return _talentJob;
            }
        }

        [SerializeField]
        private Text _purchaseMessage = default;
        private Text purchaseMessage
        {
            get
            {
                return _purchaseMessage;
            }
        }

        [SerializeField]
        private Text _pointBalance = default;
        private Text pointBalance {
            get {
                return _pointBalance;
            }
        }

        [SerializeField]
        private PointConsumptionButton _purchaseButton = default;
        private PointConsumptionButton purchaseButton {
            get {
                return _purchaseButton;
            }
        }

        [SerializeField]
        private PointPurchaseDataSource _pointDataSource = default;
        private PointPurchaseDataSource pointDataSource {
            get {
                return _pointDataSource;
            }
        }

        [SerializeField]
        private FadeMenuController _purchaseMenu = default;
        private FadeMenuController purchaseMenu
        {
            get
            {
                return _purchaseMenu;
            }
        }

        [SerializeField]
        private List<GameObject> _purchasedActivateObjectList = default;
        private List<GameObject> purchasedActivateObjectList
        {
            get
            {
                return _purchasedActivateObjectList;
            }
        }

        private RectTransform _rootRectTransform;
        private RectTransform rootRectTransform {
            get {
                return _rootRectTransform = _rootRectTransform ?? transform.root.GetComponent<RectTransform>();
            }
        }

        protected override void Start()
        {
            base.Start();
            var args = SceneLoader.Instance.GetArgment<TalentDetailLoadParam.Argment>(gameObject.scene.name);

            talentName.text = args.talentInfo.CommonName;
            talentJob.text = "/" + args.talentInfo.Job;

            purchaseMessage.text = args.talentInfo.Point.ToString(AppConst.PointFormat) + "で3Dスキャンを購入できます";
            pointBalance.text = "ポイント残高：" + pointDataSource.pointBalance.ToString(AppConst.PointFormat);
            talentImage.sprite = args.talentInfo.bannerSprite;

            // ポップアップのメッセージ設定.
            purchaseButton.popUpConfirmLoadParam.argment.message = "";
            purchaseButton.popUpConfirmLoadParam.argment.message = args.talentInfo.Point.ToString(AppConst.PointFormat) + "を使用して\n3Dスキャンを購入しますか？";
            purchaseButton.onClickPointConsumption.AddListener(() =>
            {
                // 有名人データの購入処理.
                StartCoroutine(ApiHelper.Instance.PurchaseTalent(args.talentInfo.Point, args.talentInfo.TalentResourceId, res =>
                {
                    if (res.ErrorCode != null)
                    {
                        DebugUtil.LogError("PurchaseTalent Error -> " + res.ErrorCode + " : " + res.ErrorMessage);
                        return;
                    }
                    args.talentInfo.purchaseStatus = PurchaseStatus.Purchased;
                    pointDataSource.AddPoint(-args.talentInfo.Point);
                    purchaseMenu.gameObject.SetActive(false);

                    purchasedActivateObjectList.ForEach(obj => { obj.SetActive(true); });
                    Hide();

                    OnPurchaseCompleted.Invoke(args.talentInfo);
                }));
            });
        }
    }
}