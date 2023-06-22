using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Network.Response;
using Assets.Scripts.TalentList;
using Assets.Scripts.Common;

namespace Assets.Scripts.TalentDetail
{
    public class TalentBodyCompositionDataPanel : BodyCompositionDataPanel
    {
        private const string UnpurchasedText = "****";

        private PurchaseStatus _talentPurchaseStatus = PurchaseStatus.Unpurchased;

        protected override void Start()
        {
            base.Start();

            var args = SceneLoader.Instance.GetArgment<TalentDetailLoadParam.Argment>(gameObject.scene.name);
            _talentPurchaseStatus = args.talentInfo.purchaseStatus;
        }

        /// <summary>
        /// 体組成データの表示更新.
        /// </summary>
        protected override void ApplyBodyCompositionData()
        {
            base.ApplyBodyCompositionData();

            if (_talentPurchaseStatus != PurchaseStatus.Purchased)
            {
                SetUnpurchasedText(weight);
                SetUnpurchasedText(fatPercentage);
                SetUnpurchasedText(basalMetabolism);
                SetUnpurchasedText(trunkMuscle);
                SetUnpurchasedText(trunkFat);
                SetUnpurchasedText(leftArmMuscle);
                SetUnpurchasedText(leftArmFat);
                SetUnpurchasedText(rightArmMuscle);
                SetUnpurchasedText(rightArmFat);
                SetUnpurchasedText(leftLegMuscle);
                SetUnpurchasedText(leftLegFat);
                SetUnpurchasedText(rightLegMuscle);
                SetUnpurchasedText(rightLegFat);
            }
        }

        /// <summary>
        /// 未購入時のテキスト更新処理.
        /// </summary>
        /// <param name="text"></param>
        private void SetUnpurchasedText(Text text)
        {
            text.text = UnpurchasedText;
        }

        /// <summary>
        /// 購入完了イベント受信処理.
        /// </summary>
        /// <param name="content"></param>
        public void OnPurchaseCompleted(TalentoContents content)
        {
            _talentPurchaseStatus = content.purchaseStatus;
            ApplyBodyCompositionData();
        }
    }
}